using System;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.Api.Trial.Messages
{
    /// <summary>
    /// Command to generate a API trial identity.
    /// </summary>
    [Serializable()]
    public class RequestTrialCommand : ICommand
    {
        public Guid ApplicationId { get; set; }

        public String Email {get; set; }

        public String Company { get; set; }

        public String Ip { get; set; }

        public String Referrer { get; set; }

        public String Phone { get; set; }

        public String FirstName { get; set; }

        public String LastName { get; set; }
    }
}