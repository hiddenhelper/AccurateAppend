using System;
using System.ComponentModel;
using Newtonsoft.Json;
using DateTimeConverter = DomainModel.JsonNET.DateTimeConverter;

namespace DomainModel.ReadModel
{
    public class LeadMetric
    {
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime Date { get; set; }

        [DefaultValue(0)]
        public Int32 Total { get; set; }

        [DefaultValue(0)]
        public Int32 Qualified { get; set; }

        [DefaultValue(0)]
        public Int32 NotQualified { get; set; }

        [DefaultValue(0)]
        public Int32 Unknown { get; set; }

        [DefaultValue(0)]
        public Int32 Converted { get; set; }

        public override String ToString()
        {
            return $"[LeadMetric: Date={this.Date}, Total={this.Total}, Qualified={this.Qualified}, NotQualified={this.NotQualified}, Unknown={this.Unknown}, Converted={this.Converted}]";
        }
    }
}
