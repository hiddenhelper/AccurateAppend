<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<AccurateAppend.Websites.Admin.ViewModels.Authentication.LogonModel>" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Login</title>
    <link href="<%= Url.Content("~/Content/bootstrap/bootstrap.css") %>" rel="stylesheet" type="text/css" />
    <script src="//code.jquery.com/jquery-1.11.1.min.js" type="text/javascript"> </script>
</head>
<body style="background: url(/images/bg-triangles.jpg) fixed 50% / cover;">

    <div class="row" style="padding: 30px;">
        <div class="col-md-4 col-xs-12">
            <div class="panel panel-default">
                <div class="panel-heading">
                    <h3 class="panel-title">Sign In</h3>
                </div>
                <div class="panel-body">
                    <% using (Html.BeginForm("LogOn", "Authentication")) { %>
                    <%: Html.AntiForgeryToken() %>
                    <%: Html.ValidationSummary(true) %>
                    <% = Html.HiddenFor(m => m.ReturnUrl) %>
                    <fieldset>
                        <div class="form-group">
                            <%: Html.TextBoxFor(m => m.UserName, new { @class = "form-control", @type="email", @placeholder="Email Address" }) %>
                            <%: Html.ValidationMessageFor(m => m.UserName) %>
                        </div>
                        <div class="form-group">
                            <%: Html.PasswordFor(m => m.Password, new { @class = "form-control", @type="password", @placeholder="Password" }) %>
                            <%: Html.ValidationMessageFor(m => m.Password) %>
                        </div>
                      <div class="form-group">
                        <div style="margin-top: 20px;" class="g-recaptcha" data-sitekey="6LeW4EkUAAAAAPX3-Zd61mquDOX_P26WRlgCvtAf" data-callback="imNotARobot" data-expired-callback="tooSlow"></div>
                      </div>
                        <!-- Change this to a button or input when using this as a form -->
                        <input type="submit" id="submitBtn" class="btn btn-sm btn-success" disabled="disabled" value="Login" />
                    </fieldset>
                    <%: Html.Hidden("RememberMe", false) %>
                    <%: Html.HiddenFor(m => m.Offset, new {id="offset"}) %>
                    <%} %>
                </div>
            </div>
        </div>

    </div>
<script type="text/javascript">
    var imNotARobot = function () {
        console.info("reCaptcha was verified");
      $("#submitBtn").prop('disabled', false);
    };
    var tooSlow = function () {
      console.info("reCaptcha has expired");
      $("#submitBtn").prop('disabled', true);
    };
</script>
    <script src="https://www.google.com/recaptcha/api.js"></script>

    <script type="text/javascript">
        function get_time_zone_offset() {
            var current_date = new Date();
            var gmt_offset = current_date.getTimezoneOffset() / 60;
            return gmt_offset * -1;
        }
        $('#offset').val(get_time_zone_offset());
    </script>
</body>
</html>
