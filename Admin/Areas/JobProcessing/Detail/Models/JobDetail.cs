using System;
using AccurateAppend.Core;
using DomainModel.JsonNET;
using Newtonsoft.Json;
using JobStatus = AccurateAppend.Core.Definitions.JobStatus;

namespace AccurateAppend.Websites.Admin.Areas.JobProcessing.Detail.Models
{
    [Serializable()]
    public class JobDetail
    {
        public Int32 JobId { get; set; }

        public Guid UserId { get; set; }
        
        public Int32? DealId { get; set; }

        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime DateSubmitted { get; set; }
        
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime DateUpdated { get; set; }

        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime DateComplete { get; set; }
        
        public Int32 Priority { get; set; }
        
        public Int32 InputFileSize { get; set; }
        
        public Int32 TotalRecords { get; set; }
        
        public Int32 ProcessedRecords { get; set; }
        
        public Int32 MatchRecords { get; set; }
        
        public Int32 SystemErrors { get; set; }

        [JsonConverter(typeof(JsonEnumConvertor))]
        public JobStatus Status { get; set; }

        public Boolean SensitiveData { get; set; }

        public String Product { get; set; }
               
        public String CustomerFileName { get; set; }
        
        public String Source { get; set; }
        
        public String UserName { get; set; }
        
        public String Message { get; set; }

        public String PublicFtpFolder { get; set; }

        public Double MatchRate
        {
            get
            {
                return ((ProcessedRecords > 0 && MatchRecords > 0)
                    ? (Convert.ToDouble(MatchRecords) / Convert.ToDouble(ProcessedRecords))
                    : 0);
            }
        }

        /// <summary>
        /// Contains plain language description of when job was submitted
        /// </summary>
        public String SubmittedDescription { get; set; }

        /// <summary>
        /// Returns plain language descrpition of Status
        /// </summary>
        public string StatusDescription
        {
            get
            {
                String description;
                switch (Status)
                {
                    case JobStatus.Complete:
                        description = "Complete";
                        break;
                    case JobStatus.InProcess:
                    description = "Processing";
                        break;
                    case JobStatus.WaitingToBeSliced:
                    case JobStatus.BeingSliced:
                        description = "Queuing";
                        break;
                    case JobStatus.InQueue:
                        description = "Queued";
                        break;
                    case JobStatus.Failed:
                        description = "Failed (" + SystemErrors + " errors)";
                        break;
                    
                    // verfied email
                    case JobStatus.WaitingToVerify:
                    case JobStatus.WaitingForVerify:
                        description = "Verify Email(1)";
                        break;
                    case JobStatus.WaitingToVerify2:
                        case JobStatus.WaitingForVerify2:
                        description = "Verify Email(2)";
                        break;
                    case JobStatus.WaitingToVerify3:
                    case JobStatus.WaitingForVerify3:
                        description = "Verify Email(3)";
                        break;
                    case JobStatus.WaitingToVerify4:
                    case JobStatus.WaitingForVerify4:
                        description = "Verify Email(4)";
                        break;
                    case JobStatus.WaitingToVerify5:
                        case JobStatus.WaitingForVerify5:
                        description = "Verify Email(5)";
                        break;
                    case JobStatus.WaitingToVerifyInputEmails:
                    case JobStatus.WaitingForVerifyInputEmails:
                        description = "Verify Input Email";
                        break;
                    case JobStatus.EmailVerifyComplete:
                        description = "Verify Email Complete";
                        break;
                    case JobStatus.EmailVerifyError:
                        description = "Verify Email Error";
                        break;
                    case JobStatus.NeedsReview:
                        description = "Review";
                        break;
                    case JobStatus.HasBeenReviewed:
                        description = "Has Been Reviewed";
                        break;
                   default:
                        description = this.Status.GetDescription();
                        break;
                }

                return description;
            }
        }
        /// <summary>
        /// Returns plain language description of file size
        /// </summary>
        public String FileSizeDescription => IntegerExtensions.FormatBytes(this.InputFileSize);

        /// <summary>
        /// Returns number of records processed per minute
        /// </summary>
        /// <remarks>
        /// Populated when object is created in JobQueueRespository
        /// Calculated using the past few minutes of processing
        /// Returns 0 if job is not currently processing
        /// </remarks>
        public Int32 ProcessingRate { get; set; }

        public string AltEmail { get; set; }

        public string InputFileName { get; set; }

        public Boolean IsAssociated
        {
            get { return this.DealId != null; }
        }

        public Boolean IsPaused { get; set; }
    }
}