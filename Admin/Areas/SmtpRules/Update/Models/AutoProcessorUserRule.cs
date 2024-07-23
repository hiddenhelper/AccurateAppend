using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using DomainModel.JsonNET;
using Newtonsoft.Json;

namespace AccurateAppend.Websites.Admin.Areas.JobProcessing.UpdateSmtpRule.Models
{
    [DebuggerDisplay("Term: {Terms} on Subject:{Subject}, FileName:{FileName}, Body:{Body}")]
    public class AutoProcessorUserRule
    {
        public Int32 Rid { get; set; }

        [Required()]
        public String Terms { get; set; }

        public Guid ManifestId { get; set; }

        [JsonConverter(typeof(JsonBoolBitConverter))]
        public Boolean Default { get; set; }

        [JsonConverter(typeof(JsonBoolBitConverter))]
        public Boolean Subject { get; set; }

        [JsonConverter(typeof(JsonBoolBitConverter))]
        public Boolean FileName { get; set; }

        [JsonConverter(typeof(JsonBoolBitConverter))]
        public Boolean Body { get; set; }
        
        public Byte RunOrder { get; set; }

        public String Description { get; set; }
    }
}
