<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/bootstrap3.Master" Inherits="ViewPage<Charge>" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.Sales.SubmitCharge.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Bill Order
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2 style="margin-top: 0;">Bill Order</h2>
    <div class="row" style="padding: 0 0 20px 20px;">
        <% using (this.Html.BeginForm("Index", "SubmitCharge", new {Area="Sales"}, FormMethod.Post))
           {
        %>
            <%: this.Html.Hidden("UserId", this.Model.Client) %>
            <%: this.Html.Hidden("OrderId", this.Model.Order) %>
     <div class="row">
      <div class="col-md-4">
       <% this.Html.RenderPartial("~/Areas/Sales/Shared/OrderDetail.ascx", this.Model.Order); %>
      </div>
      <div class="col-md-4">
       <% this.Html.RenderPartial("~/Views/Shared/PartyDetail2.ascx", this.Model.Client); %>
      </div>
     </div>
     <div class="row">
      <div class="col-md-12">
       <% this.Html.RenderPartial("~/Areas/Sales/Shared/OrderItems.ascx", this.Model.Order); %>
      </div>
     </div>
     <hr/>


            <div class="row">

                <div class="col-md-4">
                    <div class="panel panel-default">
                        <div class="panel-heading">Credit Card Details</div>
                        <div class="panel-body">
                          <input type="submit" value="Charge Customer's Card >" class="btn btn-primary" style="margin-bottom: 20px;" />
                         <% this.Html.RenderPartial("~/Areas/Sales/SubmitCharge/Views/Wallet.ascx", this.Model.Client); %>
                          
                                <table class="table">
                                    <tr>
                                        <th style="background-color: #ECECEC; vertical-align: middle; width: 125px;">
                                            Amount to charge: (leave blank to charge full amount)
                                        </th>
                                        <td>
                                            <%: this.Html.TextBox("amount", this.Model.MaxCharge, new {@class = "form-control", type="number", min = 0, step = 0.01, max = this.Model.MaxCharge}) %>
                                        </td>
                                    </tr>
                                </table>
                          <input type="submit" value="Charge Customer's Card >" class="btn btn-primary" />
                        </div>
                    </div>
                </div>
            </div>
        <% } %>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="DocumentStart" runat="server">
</asp:Content>