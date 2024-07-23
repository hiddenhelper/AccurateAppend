using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;
using System.Web.Mvc;
using DomainModel.JsonNET;
using Newtonsoft.Json;

namespace DomainModel.ActionResults
{
    /// <remarks>
    /// http://james.newtonking.com/archive/2008/10/16/asp-net-mvc-and-json-net.aspx
    /// </remarks>
    public class JsonNetResult : ActionResult
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonNetResult"/> class with system defaults.
        /// </summary>
        /// <remarks>
        /// The system will automatically configure the use of the <see cref="JsonEnumConvertor"/> and <see cref="DateTimeConverterUtc"/>.
        /// </remarks>
        public JsonNetResult() : this(DateTimeKind.Utc)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonNetResult"/> class with system defaults.
        /// </summary>
        /// <remarks>
        /// The system will automatically configure the use of the <see cref="JsonEnumConvertor"/> and either the
        /// <see cref="DateTimeConverterUtc"/> or <see cref="DateTimeConverter"/> depending on <paramref name="dateKind"/>.
        /// An Unspecified kind will result in no date convertors being used.
        /// </remarks>
        public JsonNetResult(DateTimeKind dateKind)
        {
            if (dateKind == DateTimeKind.Unspecified) throw new ArgumentOutOfRangeException(nameof(dateKind), dateKind, $"Only {DateTimeKind.Local} or {DateTimeKind.Utc} is supported");
            Contract.EndContractBlock();

            var convertors = new List<JsonConverter> {new JsonEnumConvertor()};
            switch (dateKind)
            {
                case DateTimeKind.Utc:
                    convertors.Add(new DateTimeConverterUtc());
                    break;
                case DateTimeKind.Local:
                    convertors.Add(new DateTimeConverter());
                    break;
            }

            this.SerializerSettings = new JsonSerializerSettings
            {
                Converters = convertors,
                DefaultValueHandling = DefaultValueHandling.Include,
                NullValueHandling = NullValueHandling.Include
            };
            this.Formatting = Formatting.Indented;
        }

        #endregion

        #region Properties

        public Encoding ContentEncoding { get; set; }

        public string ContentType { get; set; }

        public object Data { get; set; }

        public JsonSerializerSettings SerializerSettings { get; set; }

        public Formatting Formatting { get; set; }

        public int Total { get; set; }

        #endregion

        #region Overrides

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var response = context.HttpContext.Response;

            response.ContentType = !string.IsNullOrEmpty(this.ContentType)
              ? this.ContentType
              : "application/json";

            if (this.ContentEncoding != null) response.ContentEncoding = this.ContentEncoding;

            if (this.Data != null)
            {
                var writer = new JsonTextWriter(response.Output) { Formatting = this.Formatting };
                var serializer = JsonSerializer.Create(this.SerializerSettings);
                serializer.Serialize(writer, this.Data);
                writer.Flush();
            }
        }

        #endregion
    }
}