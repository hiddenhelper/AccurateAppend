using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using AccurateAppend.Core;
using AccurateAppend.Core.Collections.Generic;
using AccurateAppend.Security;

namespace AccurateAppend.Websites.Admin.Areas.Operations.Message.Models
{
    public class MessageDetail
    {
        public MessageDetail(Security.Message message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            Contract.EndContractBlock();

            this.SendTo = new Collection<String>();
            this.BccTo = new Collection<String>();
            this.Attachments = new Collection<Attachment>();

            this.Id = message.Id.Value;
            this.SendFrom = message.SendFrom;
            this.Status = message.Status.GetDescription();
            this.Subject = message.Subject;
            this.CreatedDate = message.CreatedDate.ToLocalTime();
            this.ModifiedDate = message.ModifiedDate.ToLocalTime();
            message.SendTo.ForEach(a => this.SendTo.Add(a.Address));
            message.BccTo.ForEach(a => this.BccTo.Add(a.Address));

            this.Attachments.AddRange( message.Attachments.Select(a =>
                new Attachment
                {
                    ContentType = a.ContentType,
                    Exists = (DateTime.UtcNow - message.ModifiedDate).Days <= 7,
                    SendFileName = a.SendFileName,
                    FileName = a.FileName
                }));

            this.CanResend = message.Status == MessageStatus.Posion || message.Status == MessageStatus.Sent;
            if (this.Attachments.Any(a => !a.Exists)) this.CanResend = false;

            this.CanClear = message.Status == MessageStatus.Posion;

        }

        public Int32 Id { get; private set; }
        public String SendFrom { get; private set; }
        public ICollection<String> SendTo { get; private set; }
        public ICollection<String> BccTo { get; private set; }
        public ICollection<Attachment> Attachments { get; private set; }

        public String Status { get; private set; }

        public String Subject { get; private set; }

        public DateTime CreatedDate { get; private set; }

        public DateTime ModifiedDate { get; private set; }
        public Boolean CanResend { get; private set; }
        public Boolean CanClear { get; set; }
    }
}