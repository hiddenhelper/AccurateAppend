<%@ Control Language="C#" Inherits="ViewUserControl<Card>" %>
<%@ Import Namespace="AccurateAppend.Core" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.Accounting.Models.ChargeCard" %>
<table class="table table-condensed">
    <tr>
        <th>
            Business Name
        </th>
        <td>
            <%: this.Model.BusinessName.ToTitleCase() %>
        </td>
    </tr>
    <tr>
        <th>
            Name
        </th>
        <td>
            <%: this.Model.ToDisplayName() %>
        </td>
    </tr>
    <tr>
        <th>
            Phone
        </th>
        <td>
            <%: this.Model.PhoneNumber %>
        </td>
    </tr>
    <tr>
        <th>
            Address
        </th>
        <td>
            <%= Html.Encode(this.Model.Address).Replace("\r\n", "<br />") %>
        </td>
    </tr>
    <tr>
        <th>
            City
        </th>
        <td>
            <%: this.Model.City %>
        </td>
    </tr>
    <tr>
        <th>
            State
        </th>
        <td>
            <%: this.Model.State %>
        </td>
    </tr>
    <tr>
        <th>
            Zip
        </th>
        <td>
            <%: this.Model.PostalCode %>
        </td>
    </tr>
    <tr>
        <th>
            Card Number
        </th>
        <td>
            <%: this.Model.Number %>
        </td>
    </tr>
    <tr>
        <th>
            Card Exp
        </th>
        <td>
            <%: this.Model.Expiration.Left(2) + "/" + this.Model.Expiration.Right(4) %>
        </td>
    </tr>
</table>