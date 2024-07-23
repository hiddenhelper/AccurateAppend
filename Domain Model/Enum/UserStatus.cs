using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DomainModel.Enum
{
    public enum UserStatus
    {
        [Display, Description("Active")]
        Active = 1,
        [Display, Description("Inactive")]
        Inactive = 2
    }
}
