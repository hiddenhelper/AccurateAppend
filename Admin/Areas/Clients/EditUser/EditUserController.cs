using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Mvc;
using AccurateAppend.Accounting;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Data;
using AccurateAppend.Websites.Admin.Controllers;
using EventLogger;

namespace AccurateAppend.Websites.Admin.Areas.Clients.EditUser
{
    /// <summary>
    /// Controller to edit details about a <see cref="Client"/>.
    /// </summary>
    [Authorize()]
    public class EditUserController : ContextBoundController
    {
        private readonly IRoleService rs;
        private readonly AccurateAppend.Accounting.DataAccess.DefaultContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditUserController"/> class.
        /// </summary>
        /// <param name="context">The <see cref="ISessionContext"/> to use for this controller instance.</param>
        /// <param name="rs">The role service manager.</param>
        public EditUserController(AccurateAppend.Accounting.DataAccess.DefaultContext context, IRoleService rs) : base(context)
        {
            if (rs == null) throw new ArgumentNullException(nameof(rs));
            Contract.EndContractBlock();

            this.rs = rs;
            this.context = context;
        }

        /// <summary>
        /// Update the client data.
        /// </summary>
        [AcceptVerbs(HttpVerbs.Post)]
        public virtual async Task<String> Index(Guid userid, string firstname, string lastname, string businessname, string address, string city, string state, string zip, string phone, string defaultproduct, string defaultcolumnmap, bool islockedout, bool batchUser, bool xmlUser, string email, bool storeData)
        {
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
                { 
                    using (var uow = this.Context.CreateScope(ScopeOptions.AutoCommit))
                    {
                        var client = await this.Context.SetOf<Client>().SingleAsync(c => c.Logon.Id == userid);

                        client.BusinessName = businessname;
                        client.FirstName = firstname;
                        client.LastName = lastname;
                        client.Address.Address = address;
                        client.Address.City = city;
                        client.Address.State = state;
                        client.Address.Zip = zip;
                        client.PrimaryPhone.Value = phone;
                        client.DefaultProduct = defaultproduct.Trim();
                        client.DefaultColumnMap = defaultcolumnmap.Trim();
                        client.Logon.IsLockedOut = islockedout;
                        client.AllowDataRetention = storeData;

                        if (batchUser)
                        {
                            this.rs.AddUserToRole(userid, AccountRoleService.BatchRole);
                        }
                        else
                        {
                            this.rs.RemoveUserFromRole(userid, AccountRoleService.BatchRole);
                        }

                        if (xmlUser)
                        {
                            this.rs.AddUserToRole(userid, AccountRoleService.XmlRole);
                        }
                        else
                        {
                            this.rs.RemoveUserFromRole(userid, AccountRoleService.XmlRole);
                        }

                        await uow.CommitAsync();

                        // update main account holder email if it has changed
                        if (!String.Equals(email.ToLower(), client.DefaultEmail.ToLower(), StringComparison.OrdinalIgnoreCase))
                        {
                            await this.context.Database.ExecuteSqlCommandAsync("exec [accounts].[UpdateUserEmail] @UserId=@p0, @NewEmail=@p1", userid, email);
                        }
                    }

                    transaction.Complete();  
                }
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached) Debugger.Break();
                Logger.LogEvent(ex, Severity.High, Application.AccurateAppend_Admin);

                throw;
            }

            return "";
        }
    }
}