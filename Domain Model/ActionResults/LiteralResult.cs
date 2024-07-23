using System;
using System.Text;
using System.Web.Mvc;

namespace DomainModel.ActionResults
{
    public class LiteralResult : ActionResult
    {
        public String Title { get; set;}

        public Encoding ContentEncoding { get; set; }

        public String ContentType { get; set; }

        public Object Data { get; set; }

        public Boolean PreFormatted { get; set; }

        public LiteralResult() : this(true)
        {
        }

        public LiteralResult(Boolean preFormatted)
        {
            this.PreFormatted = preFormatted;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var response = context.HttpContext.Response;

            response.ContentType = !String.IsNullOrWhiteSpace(this.ContentType)
              ? this.ContentType
              : "text/html";

            if (this.ContentEncoding != null) response.ContentEncoding = this.ContentEncoding;

            if (this.Data != null)
            {
                var sb = new StringBuilder();

                sb.AppendFormat(@"<!DOCTYPE html>
                        <html>
                        <head runat='server'>
                            <meta name='viewport' content='width=device-width' />
                            <title>{0}</title>
                        </head>
                        <body>", this.Title);
                if (this.PreFormatted) sb.Append("<pre>");
                sb.Append(this.Data);
                if (this.PreFormatted) sb.Append("</pre>");
                sb.Append(@"</body>
                        </html>");

                response.Output.Write(sb.ToString());
            }
        }
    }
}