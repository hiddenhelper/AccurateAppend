using System;
using System.ComponentModel.DataAnnotations;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.Order.Csv.Messages
{
    /// <summary>
    /// Command to instruct file analysis for a new <see cref="Sales.Cart"/> to be performed on a specified file.
    /// </summary>
    [Serializable()]
    public class AnalyzeRawCsvFileCommand : ICommand
    {
        /// <summary>
        /// The identifier of the cart the file to analyze is for.
        /// </summary>
        public Guid CartId { get; set; }

        /// <summary>
        /// The customer provided name of the file to analyze.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [MinLength(1)]
        [MaxLength(255)]
        public String ClientFileName { get; set; }

        /// <summary>
        /// The system generated name of the file to analyze.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [MinLength(36)]
        public String SystemFileName { get; set; }

        /// <summary>
        /// Gets the unique request identifier for the command.
        /// </summary>
        public Guid RequestId { get; set; }
    }
}