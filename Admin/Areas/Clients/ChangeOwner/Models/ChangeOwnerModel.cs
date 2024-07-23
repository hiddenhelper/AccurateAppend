using System;
using System.ComponentModel.DataAnnotations;

namespace AccurateAppend.Websites.Admin.Areas.Clients.ChangeOwner.Models
{
    [Serializable()]
    public class ChangeOwnerModel
    {
        public Guid Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        public Guid OwnerId { get; set; }

        public String FirstName { get; set; }

        public String LastName { get; set; }
    }
}