<%@ Page Title="" Language="C#" Inherits="System.Web.Mvc.ViewPage<String>" %>
<html>
 <body>
 <form method="post" action="https://accept.authorize.net/customer/editPayment">
   <input type="hidden" name="token" value="<%: this.Model %>"/>
   <input type="submit" value="Manage my payment and shipping information"/>
  <input type="hidden" name="paymentProfileId" value="1921369413" />
</form>
  </body>
 </html>