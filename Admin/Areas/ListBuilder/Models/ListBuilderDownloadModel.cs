using System;
using AccurateAppend.Plugin.Storage;

namespace AccurateAppend.Websites.Admin.Areas.ListBuilder.Models
{
    [Obsolete("Do not use at this time-analysis on actual need occuring", true)]
    public class ListBuilderDownloadModel
    {
        public AzureBlobFile List { get; set; }

        public AzureBlobFile ListDefintion { get; set; }
    }
}