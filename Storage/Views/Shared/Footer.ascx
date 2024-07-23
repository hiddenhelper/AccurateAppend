<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<footer>
    <div class="container">
        <div class="holder">
            <div class="box">
                <ul>
                    <li><a href="http://www.accurateappend.com/contact" target="_blank">Contact</a></li>
                    <li><a href="http://www.accurateappend.com" target="_blank">Accurate Append</a></li>
                </ul>
            </div>
            <!-- box -->
            <div style="float: right; margin-right: 20px;">
                <span style="font-size: 1.3em;">888-496-4258</span>
            </div>
        </div>
        <!-- holder -->
        <div class="frame">
            <strong class="logo"><a href="/">Accurate Append</a></strong>
            <a href="#" class="logo-bbb">
                <img src="/Content/images/logo-06.png" width="86" height="32" alt="#"></a>
            <p class="copy">&copy; <%: DateTime.Now.Year %> Accurate Append. All Rights Reserved.</p>
        </div>
        <!-- frame -->
    </div>
    <!-- container -->
</footer>
<!-- footer -->

