using System;
using System.ComponentModel.DataAnnotations;

namespace AccurateAppend.Websites.Clients.Areas.NationBuilder.DataQualityAssessment.Models
{
    [Serializable()]
    public class ConsultationRequestModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please supply your name")]
        [Display(Name = "Name")]
        public String Name { get; set; }

        [Display(Name = "Phone")]
        [DataType(DataType.PhoneNumber)]
        public String Phone { get; set; }

        [Display(Name = "Comments")]
        public String Comments { get; set; }

        public Guid CartId { get; set; }
    }
}