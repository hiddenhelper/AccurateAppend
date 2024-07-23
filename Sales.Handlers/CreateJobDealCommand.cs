using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using NServiceBus;

namespace AccurateAppend.Sales.Handlers
{
    /// <summary>
    /// Command used to construct/update a deal for a given public key, if not already completed.
    /// </summary>
    /// <remarks>
    /// Used internally to keep the complexity under wraps.
    /// </remarks>
    [Serializable()]
    public abstract class CreateJobDealCommand : ICommand, IValidatableObject
    {
        #region Properties

        /// <summary>
        /// Gets the identifier of the user that processed the job.
        /// </summary>
        /// <value>The identifier of the user that processed the job.</value>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets the shared public key for the deal creation command.
        /// </summary>
        /// <remarks>
        /// This relates the deal and job public key.
        /// </remarks>
        /// <value>The shared public key for the deal creation command.</value>
        public Guid PublicKey { get; set; }

        /// <summary>
        /// Gets the processing report to create the deal from.
        /// </summary>
        /// <value>The processing report to create the deal from.</value>
        public XElement ProcessingReport { get; set; }

        /// <summary>
        /// Gets the manifest definition from the completed job the deal should be created with.
        /// </summary>
        /// <remarks>
        /// If null, the created order will only contain items that are found in the processing report.
        /// </remarks>
        /// <value>The manifest definition from the completed job the deal should be created with.</value>
        public XElement Manifest { get; set; }

        #endregion

        #region IValidatableObject Members

        /// <inheritdoc />
        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.UserId == Guid.Empty) yield return new ValidationResult($"{nameof(this.UserId)} cannot be empty", new[] { nameof(this.UserId) });
            if (this.PublicKey == Guid.Empty) yield return new ValidationResult($"{nameof(this.PublicKey)} cannot be empty", new[] { nameof(this.PublicKey) });
            if (this.ProcessingReport == null) yield return new ValidationResult($"{nameof(this.ProcessingReport)} is required", new[] { nameof(this.ProcessingReport) });
        }

        #endregion
    }
}