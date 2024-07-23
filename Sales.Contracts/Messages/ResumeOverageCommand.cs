﻿using System;
using NServiceBus;

namespace AccurateAppend.Sales.Contracts.Messages
{
    /// <summary>
    /// Command to resume a subscription based service account that lacks saga data.
    /// Nominally just used while we sort out the kinks in the new automation
    /// perhaps we may keep it around to reopen sagas when a subscription bill
    /// is canceled/expired.
    /// </summary>
    [Serializable()]
    public class ResumeOverageCommand : ICommand
    {
        /// <summary>
        /// Gets or sets the identifier of the subscription to resume.
        /// </summary>
        public Int32 SubscriptionId { get; set; }
    }
}