using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.Data;
using AccurateAppend.Websites.Clients.Areas.NationBuilder.DataQualityAssessment;
using AccurateAppend.Websites.Clients.Areas.NationBuilder.DisplayLists.Models;
using DomainModel.ActionResults;
using EventLogger;
using Integration.NationBuilder.Data;
using Integration.NationBuilder.Service;

namespace AccurateAppend.Websites.Clients.Areas.NationBuilder.DisplayLists
{
    /// <summary>
    /// Controller for providing NationBuilder list access. Part of the Order process
    /// but due to the complexity of this part of the process, lives in a dedicated controller.
    /// </summary>
    [Authorize()]
    public class Controller : System.Web.Mvc.Controller
    {
        #region Fields

        private readonly ISessionContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Controller" /> class.
        /// </summary>
        /// <param name="context">The <see cref="ISessionContext" /> component providing entity access.</param>
        public Controller(ISessionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            Contract.EndContractBlock();

            this.context = context;
        }

        #endregion

        #region Action Methods

        #region JSON

        /// <summary>
        /// Returns all nations for a user.
        /// </summary>
        public virtual async Task<ActionResult> GetNationsForUserJson(Boolean? includeInactive, CancellationToken cancellation)
        {
            var query = this.context.SetOf<Registration>().RegistrationsForInteractiveUser();
            if (!includeInactive.HasValue || !includeInactive.Value) query = query.Where(r => r.IsActive);

            var nations = await query.Select(a => new { a.NationName, a.DateRegistered, a.Id, a.IsActive })
                .ToArrayAsync(cancellation);
            var jsonNetResult = new JsonNetResult(DateTimeKind.Local)
            {
                Data = nations.Select(a =>
                    new
                    {
                        a.NationName,
                        DateRegistered = a.DateRegistered.ToUserLocal(),
                        a.Id,
                        a.IsActive,
                        ChangeAccessAction = a.IsActive
                            ? this.Url.Action("Deactivate", "ChangeAccess", new { area = "NationBuilder", id = a.Id })
                            : this.Url.Action("Reactivate", "ChangeAccess", new { area = "NationBuilder", id = a.Id })
                    })
            };
            return jsonNetResult;
        }

        /// <summary>
        ///     Returns first 1000 lists for a specific user in Json format
        /// </summary>
        /// <remarks>
        ///     At some point this should be refactored to use server paging
        ///     Used to filter using value from AutoComplete control on DisplayLists view
        /// </remarks>
        //[OutputCache(Duration = 1 * 30, VaryByParam = "*")]
        public virtual async Task<ActionResult> GetListsJson(String listname, Int32? listid, Int32 id)
        {
            using (this.context.CreateScope(ScopeOptions.NoTracking))
            {
                var registration = await this.context
                    .SetOf<Registration>()
                    .RegistrationsForInteractiveUser()
                    .FirstOrDefaultAsync(r => r.Id == id && r.IsActive);
                if (registration == null) return new JsonNetResult { Data = new { Data = Enumerable.Empty<ListData>() } };

                try
                {
                    var client = ClientFactory.CreateStandardApi(registration.NationName);

                    var data = new List<ListData>();
                    var currentPage = 1;

                    do
                    {
                        var result = await client.AllListsAsync(registration.LatestAccessToken, currentPage);
                        data.AddRange(result.Results);

                        currentPage = result.PageCount > currentPage ? result.CurrentPage + 1 : 0;
                    } while (currentPage > 0);

                    var jsonNetResult = new JsonNetResult(DateTimeKind.Utc)
                    {
                        Data = new
                        {
                            Data = String.IsNullOrEmpty(listname)
                                ? (listid == null ? data : data.Where(a => a.Id == listid))
                                : data.Where(a => a.Name == this.Server.UrlDecode(listname)),
                            Total = data.Count
                        }
                    };
                    return jsonNetResult;
                }
                catch (RateLimitException)
                {
                    //TODO: display notice to client
                    return new JsonNetResult
                    {
                        Data = new
                        {
                            HttpStatusCodeResult = (Int32)HttpStatusCode.Conflict,
                            ErrorMessage = $"Your Nation {registration.NationName} has no remaining calls for today left on the API."
                        }
                    };
                }
                catch (EndpointNotFoundException)
                {
                    return new JsonNetResult
                    {
                        Data = new
                        {
                            HttpStatusCodeResult = (Int32) HttpStatusCode.NotFound,
                            ErrorMessage = $"NationBuilder API reports your Nation {registration.NationName} no longer exists."
                        }
                    };
                }
                catch (TokenRevokedException)
                {
                    //TODO: redirect notice to client to renew action
                    return new JsonNetResult
                    {
                        Data = new
                        {
                            HttpStatusCodeResult = (Int32)HttpStatusCode.Unauthorized,
                            ErrorMessage = $"Our access to your Nation {registration.NationName} has been revoked",
                            RedirectTo = this.Url.Action("Index", "Renew", new { area = "NationBuilder", slug = registration.NationName})
                        }
                    };
                }
                catch (Exception ex)
                {
                    Logger.LogEvent(ex, TraceEventType.Warning, Severity.High, Application.Clients.ToString(), Request.UserHostAddress, $"NationBuilder API unhandled exception {this.User.Identity.Name}");
                    return new JsonNetResult
                    {
                        Data = new
                        {
                            HttpStatusCodeResult = (Int32)HttpStatusCode.InternalServerError,
                            ErrorMessage = "Unable to retrieve lists from your nation."
                        }
                    };
                }
            }
        }

        /// <summary>
        /// Determines if a token is valid by contacting the Nation Builder API.
        /// </summary>
        /// <remarks>Called by 'change' events bound to seelcts containing Nation name</remarks>
        public async Task<ActionResult> CheckRegistrationValidToken(String nationName, CancellationToken cancellation)
        {
            try
            {
                Registration registration;

                using (this.context.CreateScope(ScopeOptions.NoTracking))
                {
                    registration = await this.context
                        .SetOf<Registration>()
                        .RegistrationsForInteractiveUser()
                        .FirstOrDefaultAsync(r => r.NationName == nationName, cancellation);
                }

                // no access token in Registrations for that user
                if (registration == null)
                {
                    return new JsonNetResult
                    {
                        Data = new
                        {
                            HttpStatusCodeResult = (Int32)HttpStatusCode.BadRequest,
                            Message = $"{nationName} registration does not exist. Please contact support"
                        }
                    };
                }

                if (!registration.IsActive)
                {
                    return new JsonNetResult
                    {
                        Data = new
                        {
                            HttpStatusCodeResult = (Int32)HttpStatusCode.Redirect,
                            Message = $"{nationName} registration has been deactivated. Please contact support"
                        }
                    };
                }

                var client = ClientFactory.CreateStandardApi(registration.NationName);

                var status = await client.ConfirmAcessTokenIsValidAsync(registration.LatestAccessToken);

                // we have token, but API says no good
                if (status == TokenCheckStatus.RevokedOrExpired)
                {
                    return new JsonNetResult
                    {
                        Data = new
                        {
                            HttpStatusCodeResult = (Int32)HttpStatusCode.Redirect,
                            Message = $"{nationName} nation is not allowing access to our application. Please contact support"
                        }
                    };
                }

                // Closed up shop
                if (status == TokenCheckStatus.NationNotFound)
                {
                    return new JsonNetResult
                    {
                        Data = new
                        {
                            HttpStatusCodeResult = (Int32)HttpStatusCode.Gone,
                            Message = $"{nationName} nation does not exist. Please contact support"
                        }
                    };
                }

                // A-OK folks!
                return new JsonNetResult
                {
                    Data = new
                    {
                        HttpStatusCodeResult = (Int32)HttpStatusCode.OK,
                        Message = HttpStatusCode.OK.ToString()
                    }
                };
            }
            catch (Exception)
            {
                return new JsonNetResult
                {
                    Data = new
                    {
                        HttpStatusCodeResult = (Int32)HttpStatusCode.InternalServerError,
                        Message = $"Unable to contact {nationName} nation to confirm access."
                    }
                };
            }
        }

        /// <summary>
        /// Returns first 1000 list names for a specific user in Json format
        /// </summary>
        /// <remarks>Used to populate AutoComplete control on DisplayLists view</remarks>
        [OutputCache(Duration = 1 * 60, VaryByParam = "*", Location = OutputCacheLocation.Client)] 
        public virtual async Task<ActionResult> GetListNamesJson(Int32 id)
        {
            using (this.context.CreateScope(ScopeOptions.NoTracking))
            {
                var registration = await this.context.SetOf<Registration>()
                    .RegistrationsForInteractiveUser()
                    .FirstOrDefaultAsync(r => r.Id == id && r.IsActive);
                if (registration == null) return this.Json(new { Success = false, ErrorMessage = "Unable to retrieve lists." }, JsonRequestBehavior.AllowGet);

                try
                {
                    var client = ClientFactory.CreateStandardApi(registration.NationName);

                    var data = (await client.AllListsAsync(registration.LatestAccessToken, 1, 1000))
                        .Results
                        .Select(a => new  { name = a.Name,id = a.Id});
                    return this.Json(new { Success = true, Data = data, ErrorMessage = "" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    Logger.LogEvent(ex, TraceEventType.Warning, Severity.High, Application.Clients.ToString(), this.Request.UserHostAddress, $"NationBuilder API unhandled exception {this.User.Identity.Name}");

                    return this.Json(new { Success = false, ErrorMessage = "Unable to retrieve lists. NationBuilder failed to return a result" }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        #endregion

        /// <summary>
        /// Step 1 in the order process. Displays a user's NationBuilder lists for an active Nation.
        /// </summary>
        public virtual async Task<ActionResult> Index(Guid cartId, CancellationToken cancellation)
        {
            var model = new SelectListModel() { CartId = cartId };
            // Admin aliasing users can sidestep this maximum
            if (this.User.Identity.IsImpersonating()) model.MaximumListSize = Int32.MaxValue;

            using (this.context.CreateScope(ScopeOptions.NoTracking))
            {
                // redirect user to renew if they have not registered a nation
                var registrations = await this.context
                    .SetOf<Registration>()
                    .RegistrationsForInteractiveUser()
                    .Where(r => r.IsActive)
                    .Select(r => new {r.Id, r.Marketing.PersonCount, r.NationName, r.LatestAccessToken})
                    .ToArrayAsync(cancellation);

                if (!registrations.Any()) return this.RedirectToAction("Index", "Renew", new { area = "NationBuilder" });

                var userSettings = this.Request.Cookies[DataQualityAssessmentController.CookieName];
                var supress = !String.IsNullOrEmpty(userSettings?[DataQualityAssessmentController.SettingName]);

                // if user has registered and has equal to or more people and has not opted out then redirect to interstitial page
                if (registrations.Any(a => a.PersonCount >= DataQualityAssessmentController.ImpressiveListSize) && !supress) return this.RedirectToAction("Request", "DataQualityAssessment", new { area = "NationBuilder", cartId });

                // if user has one nation and one list then send them directly to the order page
                if (registrations.Length == 1)
                {
                    var registration = registrations.First();

                    try
                    {
                        var client = ClientFactory.CreateStandardApi(registration.NationName);

                        var data = (await client.AllListsAsync(registration.LatestAccessToken, 1, 100, cancellation)).Results;

                        // Filter out 0 length lists
                        data = data.Where(l => l.Count > 0).ToList();

                        if (!data.Any())
                        {
                            return this.RedirectToAction(nameof(NoLists));
                        }
                        if (data.Count == 1)
                        {
                            var list = data.First();
                            return this.RedirectToAction("SelectList", "Order",
                                new
                                {
                                    cartId,
                                    area = "NationBuilder",
                                    listid = list.Id,
                                    listName = list.Name,
                                    regId = registration.Id,
                                    recordCount = list.Count
                                });
                        }
                    }
                    catch (EndpointNotFoundException)
                    {
                        return this.RedirectToAction("NoLists", "DisplayLists", new { area = "NationBuilder" });
                    }
                    catch (TokenRevokedException)
                    {
                        return this.View(model);
                        //return this.RedirectToAction("Index", "Renew", new { area = "NationBuilder", slug = registration.NationName });
                    }
                }
            }

            return this.View(model);
        }

        /// <summary>
        /// Eject page for when the client doesn't have active Nation registrations.
        /// </summary>
        public virtual ActionResult NoLists()
        {
            return this.View();
        }

        #endregion
    }
}