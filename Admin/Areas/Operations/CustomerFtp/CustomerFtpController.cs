using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Data;
using AccurateAppend.JobProcessing;
using AccurateAppend.Websites.Admin.Controllers;
using DomainModel.ActionResults;
using DomainModel.Messages;
using NServiceBus;
using RestSharp;

namespace AccurateAppend.Websites.Admin.Areas.Operations.Controllers
{
    [Authorize()]
    public class CustomerFtpController : ContextBoundController
    {
        private readonly ISessionContext context;
        private readonly IMessageSession bus;

        public CustomerFtpController(ISessionContext context, IMessageSession bus) : base(context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (bus == null) throw new ArgumentNullException(nameof(bus));

            this.context = context;
            this.bus = bus;
        }

        [HttpGet()]
        public virtual ActionResult Index(Guid userId)
        {
            return this.View(new FtpLogonModel() {UserId = userId});
        }

        [HttpGet()]
        public virtual async Task<ActionResult> Query(Guid userId, CancellationToken cancellation)
        {
            var ftp = await this.context
                .SetOf<FtpBatchAccount>()
                .Where(f => f.Logon.Id == userId)
                .Select(f => new FtpLogonModel
                {
                    Id = f.Id,
                    UserId = userId,
                    UserName = f.UserName,
                    Password = f.Password,
                    IsActive = f.Status == FtpAccountStatus.Active
                })
                .FirstOrDefaultAsync(cancellation);
            if (ftp == null)
            {
                ftp = new FtpLogonModel
                {
                    Id = null,
                    UserId = userId,
                    UserName = String.Empty,
                    Password = String.Empty,
                    IsActive= false
                };
            }

            return new JsonNetResult() {Data = ftp};
        }

        [HttpGet()]
        public virtual async Task<ActionResult> Generate(CancellationToken cancellation)
        {
            var client = new RestClient("https://www.passwordrandom.com/query?command=password");
            var request = new RestRequest(Method.GET);
            var response = await client.ExecuteTaskAsync(request, cancellation);
            return this.Content(response.Content);
        }

        [HttpPost()]
        public virtual async Task<ActionResult> Index(FtpLogonModel model)
        {
            if (!this.ModelState.IsValid) return this.View(model);
            
            var message = new ConfigureFtpAccountCommand { UserId = model.UserId, UserName = model.UserName, Password = model.Password, Enabled = model.IsActive };
            await this.bus.Send(message);

            return this.View(model);
        }
    }

    [Serializable()]
    public class FtpLogonModel
    {
        public Guid UserId { get; set; }

        public Int32? Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MinLength(1)]
        [MaxLength(50)]
        [DataType(DataType.Text)]
        public String UserName { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MinLength(1)]
        [MaxLength(50)]
        [DataType(DataType.Password)]
        public String Password { get; set; }

        public Boolean IsActive { get; set; }
    }
}