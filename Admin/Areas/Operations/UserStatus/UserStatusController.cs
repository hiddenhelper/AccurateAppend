using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;
using AccurateAppend.Core;
using AccurateAppend.Data;
using AccurateAppend.Security;
using AccurateAppend.Websites.Admin.Controllers;
using DomainModel.ActionResults;

namespace AccurateAppend.Websites.Admin.Areas.Operations.UserStatus
{
    [Authorize()]
    public class UserStatusController : ContextBoundController
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="UserStatusController"/> class.
        /// </summary>
        /// <param name="context">The <see cref="ISessionContext"/> DAL component.</param>
        public UserStatusController(ISessionContext context) : base(context)
        {
        }

        #endregion

        #region Nested Types

        protected class UserStatusDto
        {
            public String UserName { get; set; }

            public String Status { get; set; }

            public DateTime? LastActivity { get; set; }

            public string LastActivityDescription
            {
                get
                {
                    return this.LastActivity == null
                        ? ""
                        : this.LastActivity.Value.DescribeDifference(DateTime.UtcNow);
                }
            }
        }

        #endregion

        #region Action Methods

        [OutputCache(Duration = 1*60, VaryByParam = "none", Location = OutputCacheLocation.Client)]
        public virtual async Task<ActionResult> ActivitySummary(CancellationToken cancellation)
        {
            var activeFloor = DateTime.UtcNow.AddMinutes(-15);
            var awayFloor = DateTime.UtcNow.AddMinutes(-30);
            var superUserFloor = awayFloor.AddHours(-1);
            var activityCutOff = DateTime.Now.AddDays(-14);
            var output = new List<UserStatusDto>();

            using (this.Context.CreateScope(ScopeOptions.ReadOnly))
            {
                var activity = this.Context.SetOf<ActivityEntry>();

                var users = this.Context.SetOf<User>()
                    .Where(u => !u.IsLockedOut)
                    .Where(u => u.Application.Id == ApplicationExtensions.AdminId);

                var access = this.Context.SetOf<Access>().Join(users, a => a.Id, u => u.Id, (a, u) => a);
                
                var allUserActivity = access.GroupJoin(activity, u => u.Id, a => a.PerformedBy.Id,
                    (u, a) =>
                        new
                        {
                            u.Rights,
                            u.UserName,
                            LastActivity = a.OrderByDescending(i => i.EventDate)
                                    .Select(i => (DateTime?) i.EventDate)
                                    .FirstOrDefault()
                        }).Where(a => a.LastActivity > activityCutOff);

                await allUserActivity.OrderBy(u => u.UserName).ForEachAsync(entry =>
                {
                    if (entry.LastActivity == null)
                    {
                        output.Add(new UserStatusDto
                        {
                            UserName = entry.UserName,
                            Status = "Offline"
                        });
                        return;
                    }

                    if (entry.LastActivity >= activeFloor ||
                        (entry.LastActivity >= superUserFloor && entry.Rights.Any(r => r.Name == "super user")))
                    {
                        output.Add(new UserStatusDto
                        {
                            LastActivity = entry.LastActivity,
                            UserName = entry.UserName,
                            Status = "Online"
                        });
                        return;
                    }

                    if (entry.LastActivity < activeFloor && entry.LastActivity >= awayFloor)
                    {
                        output.Add(new UserStatusDto
                        {
                            LastActivity = entry.LastActivity,
                            UserName = entry.UserName,
                            Status = "Away"
                        });
                        return;
                    }

                    if (entry.LastActivity < awayFloor)
                    {
                        output.Add(new UserStatusDto
                        {
                            LastActivity = entry.LastActivity,
                            UserName = entry.UserName,
                            Status = "Offline"
                        });
                    }
                }, cancellation);

                var jsonNetResult = new JsonNetResult
                {
                   Data = output
                };

                return jsonNetResult;
            }
        }

        public ActionResult Index()
        {
            return View();
        }

        #endregion
    }
}
