using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using AccurateAppend.JobProcessing.Mapping;

namespace DomainModel.Enum
{
    public enum JobSource
    {
        [Display, Description("Nation Builder")]
        NationBuilder = IntegrationJobConfiguration.Source,
        [Display, Description("FTP Auto")]
        // ReSharper disable InconsistentNaming
        FTPAuto = FtpJobConfiguration.Source,
        // ReSharper restore InconsistentNaming
        [Display, Description("Email Auto")]
        EmailAuto = SmtpJobConfiguration.Source,
        [Display, Description("Administrator")]
        AdminWebsite = ManualJobConfiguration.Source,
        [Display, Description("Customer Upload")]
        PublicWebsite = ClientJobConfiguration.Source,
        [Display, Description("List Builder")]
        ListBuilder = ListbuilderJobConfiguration.Source
    }
}
