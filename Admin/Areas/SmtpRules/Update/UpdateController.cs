using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Accounting;
using AccurateAppend.Core.ComponentModel;
using AccurateAppend.Data;
using AccurateAppend.JobProcessing;
using AccurateAppend.Websites.Admin.Areas.JobProcessing.UpdateSmtpRule.Models;
using AccurateAppend.Websites.Admin.Controllers;
using DomainModel.ActionResults;
using DomainModel.JsonNET;

namespace AccurateAppend.Websites.Admin.Areas.SmtpRules.Update
{
    [Authorize()]
    public class UpdateController : ContextBoundController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateController"/> class.
        /// </summary>
        /// <param name="context">The <see cref="ISessionContext"/> to use for this controller instance.</param>
        public UpdateController(ISessionContext context) : base(context)
        {
        }

        /// <summary>
        /// Updates a <see cref="SmtpRuleSet"/> for a given <see cref="Client"/>
        /// </summary>
        /// <param name="ruleSetModel"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public virtual async Task<ActionResult> Index([ModelBinderEx(typeof(FormCollectionJsonBinder), "json")]
            AutoProcessorUserRuleSetModel ruleSetModel, CancellationToken cancellation)
        {
            if (ruleSetModel == null || !ruleSetModel.Rules.Any())
                return new JsonNetResult { Data = new { HttpStatusCode = HttpStatusCode.BadRequest, Message = "Rules collection is empty." } };

            // Validate rules
            var errors = ruleSetModel.Validate().ToArray();
            if (errors.Any())
            {
                return new JsonNetResult
                {
                    Data = new
                    {
                        HttpStatusCode = HttpStatusCode.BadRequest,
                        Message = "Rule set failed validation. Rules need a description, terms and Subject, Body, or File name.",
                        Errors = errors.Select(e => e.Message)
                    }
                };
            }

            // Update existing rules, insert new rule
            using (var uow = this.Context.CreateScope(ScopeOptions.AutoCommit))
            {
                var ids = ruleSetModel.Rules.Select(r => r.Rid).ToArray();

                var data = await this.Context
                    .SetOf<SmtpAutoprocessorRule>()
                    .Where(r => ids.Contains(r.Id))
                    .ToListAsync(cancellation);
                var ruleset = new SmtpRuleSet(data); // this is what allows a specific rule to be set as default

                var client = await this.Context
                    .SetOf<Client>()
                    .FirstAsync(c => c.Logon.Id == ruleSetModel.UserId, cancellation);

                foreach (var input in ruleSetModel.Rules)
                {
                    var rule = data.FirstOrDefault(r => r.Id == input.Rid);
                    if (rule == null)
                    {
                        var manifest = await this.Context
                            .SetOf<ManifestCache>()
                            .FirstAsync(m => m.Id == input.ManifestId, cancellation);

                        rule = new SmtpAutoprocessorRule(client, input.Terms, manifest);
                        this.Context.SetOf<SmtpAutoprocessorRule>().Add(rule);
                    }

                    if (input.Default) ruleset.MakeDefault(rule);
                    rule.Terms = input.Terms.Trim();
                    rule.Description = input.Description ?? String.Empty;
                    if (rule.Description.Length == 0 && rule.Terms == "*") rule.Description = "Default";
                    rule.ForBodyContent = input.Body;
                    rule.ForSubject = input.Subject;
                    rule.ForFilename = input.FileName;
                    rule.Order = input.RunOrder;
                }

                await uow.CommitAsync(cancellation);
            }

            return new JsonNetResult { Data = new { HttpStatusCode = HttpStatusCode.Accepted, Message = "Rules successfully updated." } };
        }

    }
}