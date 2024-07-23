using System.Web.Mvc;

namespace AccurateAppend.Websites.Admin.Filters
{
    public class HandleErrorWithAjaxFilter : HandleErrorAttribute
    {
        public bool ShowStackTraceIfNotDebug { get; set; }
        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                var content = this.ShowStackTraceIfNotDebug ||
                                filterContext.HttpContext.IsDebuggingEnabled ?
                                    filterContext.Exception.Message :
                                    string.Empty;
                filterContext.Result = new ContentResult
                {
                    ContentType = "text/plain",
                    Content = content
                };
                filterContext.HttpContext.Response.Status =
                    "500 " + filterContext.Exception.Message
                    .Replace("\r", " ")
                    .Replace("\n", " ");
                filterContext.ExceptionHandled = true;
                filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
            }
            else
            {
                base.OnException(filterContext);
            }
        }
    }
}
