<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<DomainModel.ReadModel.Prospector.ColumnViewModel>" %>
<style scoped>
    td, th { white-space: nowrap; font-size: .9em }
</style>
<table class="table table-condensed" style="margin-bottom: 0;">
    <tr>
        <td style="border: none;"><label>Field Type</label></td>
        <td style="border: none;"><%= Model.FieldName.Replace("(Column "+ Model.ColumnIndex + ")","") %></td>
    </tr>
    <tr>
        <td><label>Total Records</label></td>
        <td><%= Model.TotalRecords.ToString("N0") %></td>
    </tr>
    <tr>
        <td><label>Filed Records</label></td>
        <td><%= Model.FilledRecords.ToString("N0") %></td>
    </tr>
    <tr>
        <td><label>Cardinality</label></td>
        <td><%= Model.Cardinality.ToString("N0") %></td>
    </tr>
    <tr>
        <td><label>Distinct Values</label></td>
        <td><%= Model.DistinctPct %>%</td>
    </tr>
    <tr>
        <td><label>Min Length</label></td>
        <td><%= Model.MinLength.ToString("N0") %></td>
    </tr>
    <tr>
        <td><label>Max Length</label></td>
        <td><%= Model.MaxLength.ToString("N0") %></td>
    </tr>
    <tr>
        <th>Most Common Values</th>
        <th>Instances</th>        
    </tr>
    <tr>
        <td>Blank</td>
        <td><%= Model.Commonest_Blank %></td>        
    </tr>
    <% if (Model.Commonest_1.Count > 0)
       { %>
        <tr>
            <td><%= Model.Commonest_1.FactType %></td>
            <td><%= Model.Commonest_1.Count.ToString("N0") %></td>        
        </tr>
    <% } %>
    <% if (Model.Commonest_2.Count > 0)
       { %>
        <tr>
            <td><%= Model.Commonest_2.FactType %></td>
            <td><%= Model.Commonest_2.Count.ToString("N0") %></td>        
        </tr>
    <% } %>
    <% if (Model.Commonest_3.Count > 0)
       { %>
        <tr>
            <td><%= Model.Commonest_3.FactType %></td>
            <td><%= Model.Commonest_3.Count.ToString("N0") %></td>        
        </tr>
    <% } %>
    <% if (Model.Commonest_4.Count > 0)
       { %>
        <tr>
            <td><%= Model.Commonest_4.FactType %></td>
            <td><%= Model.Commonest_4.Count.ToString("N0") %></td>        
        </tr>
    <% } %>
    <% if (Model.Commonest_5.Count > 0)
       { %>
        <tr>
            <td><%= Model.Commonest_5.FactType %></td>
            <td><%= Model.Commonest_5.Count.ToString("N0") %></td>        
        </tr>
    <% } %>
    
</table>
