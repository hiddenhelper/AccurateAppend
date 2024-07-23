using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace AccurateAppend.Websites.Admin.Areas.Operations.Message.Models
{
    public class MessageListViewModel
    {
        private readonly IEnumerable<MessageDetail> messages;
        private readonly DateTime startDate;
        private readonly DateTime endDate;
        private readonly String email;

        public MessageListViewModel(IEnumerable<Security.Message> messages, DateTime startDate, DateTime endDate, String email)
        {
            if (messages == null) throw new ArgumentNullException(nameof(messages));
            Contract.EndContractBlock();

            this.messages = messages.Select(m => new MessageDetail(m)).ToArray();
            this.startDate = startDate;
            this.endDate = endDate;
            this.email = email;
        }

        public virtual IEnumerable<MessageDetail> Messages => this.messages;

        public virtual DateTime StartDate => this.startDate;

        public virtual DateTime EndDate => this.endDate;

        public virtual String Email => this.email;
    }
}