<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/bootstrap3.Master" Inherits="ViewPage<ChangeBillingAddressModel>" %>
<%@ Import Namespace="DomainModel.Html" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.Billing.EditCreditCard.Models" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.Billing.Shared.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Add Credit Card
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="row" style="padding: 0 0 20px 20px;">
      <div class="col-md-5">
        <% this.Html.RenderPartial("~/Views/Shared/PartyDetail2.ascx", this.Model.UserId); %>
      </div>
      </div>
  <div class="row" style="padding: 0 0 20px 20px;">
      <div class="col-md-5">
          <div class="panel panel-default">
                <div class="panel-heading">
                    Address Details - <%: this.Model.UserName %>
                </div>
                <div class="panel-body">

                    <% if (this.TempData["message"] != null)
                       { %>
                        <div id="notice" class="alert alert-warning" style="margin: 20px 0 20px 0;">
                            <%= this.TempData["message"] %>
                        </div>
                    <% } %>

                    <% using (this.Html.BeginForm("ChangeBillingAddress", "EditCreditCard", FormMethod.Post, new {@class = "form-horizontal"}))
                       { %>

                        <div class="form-group">
                            <label class="col-sm-3 control-label"><%= this.Html.LabelFor(m => m.Address.BusinessName) %></label>
                            <div class="col-sm-8">
                                <%= this.Html.TextBoxFor(m => m.Address.BusinessName, new {@class = "form-control", maxlength = BillingAddressModel.DataSize.BusinessName}) %>
                                <%= this.Html.ValidationMessageFor(m => m.Address.BusinessName, "", new { @style="color: red;" }) %>
                            </div>
                        </div>
                        <div class="form-group">
                            <label class="col-sm-3 control-label"><%= this.Html.LabelFor(m => m.Address.FirstName) %></label>
                            <div class="col-sm-8">
                                <%= this.Html.TextBoxFor(m => m.Address.FirstName, new {@class = "form-control", maxlength = BillingAddressModel.DataSize.FirstName}) %>
                                <%= this.Html.ValidationMessageFor(m => m.Address.FirstName, "", new { @style="color: red;" }) %>
                            </div>
                        </div>
                        <div class="form-group">
                            <label class="col-sm-3 control-label"><%= this.Html.LabelFor(m => m.Address.LastName) %></label>
                            <div class="col-sm-8">
                                <%= this.Html.TextBoxFor(m => m.Address.LastName, new {@class = "form-control", maxlength = BillingAddressModel.DataSize.LastName}) %>
                                <%= this.Html.ValidationMessageFor(m => m.Address.LastName, "", new { @style="color: red;" }) %>
                            </div>
                        </div>
                        <div class="form-group">
                            <label class="col-sm-3 control-label"><%= this.Html.LabelFor(m => m.Address.PhoneNumber) %></label>
                            <div class="col-sm-8">
                                <%= this.Html.TextBoxFor(m => m.Address.PhoneNumber, new {@class = "form-control", type = "tel", maxlength = BillingAddressModel.DataSize.PhoneNumber}) %>
                                <%= this.Html.ValidationMessageFor(m => m.Address.PhoneNumber, "", new { @style="color: red;" }) %>
                            </div>
                        </div>
                        <div class="form-group">
                            <label class="col-sm-3 control-label"><%= this.Html.LabelFor(m => m.Address.Address) %></label>
                            <div class="col-sm-8">
                                <%= this.Html.TextBoxFor(m => m.Address.Address, new {@class = "form-control", maxlength = BillingAddressModel.DataSize.Address}) %>
                                <%= this.Html.ValidationMessageFor(m => m.Address.Address, "", new { @style="color: red;" }) %>
                            </div>
                        </div>
                        <div class="form-group">
                            <label class="col-sm-3 control-label"><%= this.Html.LabelFor(m => m.Address.City) %></label>
                            <div class="col-sm-8">
                                <%= this.Html.TextBoxFor(m => m.Address.City, new {@class = "form-control", maxlength = BillingAddressModel.DataSize.City}) %>
                                <%= this.Html.ValidationMessageFor(m => m.Address.City, "", new { @style="color: red;" }) %>
                            </div>
                        </div>
                        <div class="form-group">
                            <label class="col-sm-3 control-label"><%= this.Html.LabelFor(m => m.Address.State) %></label>
                            <div class="col-sm-8">
                                <%= this.Html.TextBoxFor(m => m.Address.State, new {@class = "form-control", maxlength = BillingAddressModel.DataSize.State, id = "State"}) %>
                                <%= this.Html.DropDownList("StateHelper", NorthAmericanTerritories.StateSelectList(),
                                        new {@class = "form-control", OnClick="javascript:$(\"#State\").val($(\"#StateHelper\").val())"}
                                        )
    %>
                                <%= this.Html.ValidationMessageFor(m => m.Address.State, "", new { @style="color: red;" }) %>
                            </div>
                        </div>
                        <div class="form-group">
                            <label class="col-sm-3 control-label"><%= this.Html.LabelFor(m => m.Address.Zip) %></label>
                            <div class="col-sm-8">
                                <%= this.Html.TextBoxFor(m => m.Address.Zip, new {@class = "form-control", maxlength = BillingAddressModel.DataSize.Zip}) %>
                                <%= this.Html.ValidationMessageFor(m => m.Address.Zip, "", new { @style="color: red;" }) %>
                            </div>
                        </div>
                        <div class="form-group">
                            <label class="col-sm-3 control-label"><%= this.Html.LabelFor(m => m.Address.Country) %></label>
                            <div class="col-sm-8">
                                <%= this.Html.DropDownListFor(a => a.Address.Country, Countries.CountrySelectList, new {@class = "form-control", }) %>
                                <%= this.Html.ValidationMessageFor(m => m.Address.Country, "", new { @style="color: red;" }) %>
                            </div>
                        </div>
                        <input type="submit" value="Create" class="btn btn-primary col-sm-offset-3"/>
                        <%= this.Html.HiddenFor(m => m.UserId) %>
                        <%= this.Html.HiddenFor(m => m.UserName) %>
                        <%= this.Html.HiddenFor(m => m.CardId) %>
                    <% } %>
                </div>
            </div>
        </div>

    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="DocumentStart" runat="server">
</asp:Content>