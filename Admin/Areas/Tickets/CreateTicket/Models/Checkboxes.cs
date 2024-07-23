using System;
using System.ComponentModel.DataAnnotations;

namespace AccurateAppend.Websites.Admin.Areas.Tickets.CreateTicket.Models
{
    [Serializable()]
    public class CheckBoxes
    {
        [Required()]
        public String Value { get; set; }

        public String Text { get; set; }

        public Boolean Checked { get; set; }
    }
}