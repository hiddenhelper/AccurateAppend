using System.Threading.Tasks;
using AccurateAppend.Core.Definitions;
using AccurateAppend.CustomerManagement.Contracts;
using AccurateAppend.Operations.Contracts;
using EventLogger;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.Public.LeadsApi.Messages
{
    /// <summary>
    /// Responds to the <see cref="LeadCreatedEvent"/> and sends and internal untracked email
    /// to the support team indicating the new lead creation.
    /// </summary>
    public class SendAlertForLeadCreatedEvent : IHandleMessages<LeadCreatedEvent>
    {
        /// <inheritdoc />
        public virtual async Task Handle(LeadCreatedEvent message, IMessageHandlerContext context)
        {
            using (new Correlation(message.PublicKey))
            {
                var command = new SendEmailCommand();
                command.IsHtmlContent = false;
                command.MessageKey = message.PublicKey;
                command.Subject = "New automated lead created";
                command.Body = $"https://admin.accurateappend.com/clients/LeadDetail/PublicKey?id={message.PublicKey}";
                command.SendFrom = "support@accurateappend.com";
                command.To.Add("support@accurateappend.com");
                command.Track = false;

                await context.Send(command);

                Logger.LogEvent($"Alert message sent for {message.Email} on lead {message.PublicKey}", Severity.None, Application.Clients);
            }
        }
    }
}