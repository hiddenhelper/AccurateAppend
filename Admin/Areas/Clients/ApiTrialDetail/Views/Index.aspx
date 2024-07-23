<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/bootstrap3.Master" Inherits="ViewPage<int>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  Api Trial Detail
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  
  <div class="row" style="padding: 0 0 20px 20px;">
    <div class="col-md-6">
      <div class="panel panel-default">
        <div class="panel-heading">API Trial Details</div>
        <div class="panel-body">
          <div id="globalMessage" style="display: none;"></div>
          <form class="form-horizontal">
            <div class="form-group">
              <label for="" class="col-sm-4 control-label">Email</label>
              <div class="col-sm-7">
                <div class="input-group">
                  <input type="text" class="form-control" aria-label="" id="DefaultEmail">
                  <div class="input-group-btn">
                    <a href="#" class="btn btn-default" id="viewSourceLead">View Source Lead</a>
                  </div>
                </div>
              </div>
            </div>
            <div class="form-group">
              <label for="" class="col-sm-4 control-label">Trial Key</label>
              <div class="col-sm-7">
                <div class="input-group">
                  <input type="text" class="form-control" aria-label="" id="AccessId">
                  <div class="input-group-btn">
                    <button type="button" class="btn btn-default" id="copyAccessIdToClipboard">Copy</button>
                  </div>
                </div>
              </div>
            </div>
            <div class="form-group">
              <label for="" class="col-sm-4 control-label">Start Date</label>
              <div class="col-sm-7">
                <input class="form-control" id="DateCreated" disabled="disabled"/>
              </div>
            </div>
            <div class="form-group">
              <label for="" class="col-sm-4 control-label">Expiration Date</label>
              <div class="col-sm-7">
                <input class="form-control" id="ExpirationDate" disabled="disabled"/>
              </div>
            </div>
            <div class="form-group">
              <label for="" class="col-sm-4 control-label">Total Lifetime Calls</label>
              <div class="col-sm-7">
                <input class="form-control" id="MaximumCalls" disabled="disabled"/>
              </div>
            </div>
            <div class="form-group">
              <label for="" class="col-sm-4 control-label">Is Enabled</label>
              <div class="col-sm-7">
                <div class="form-group">
                  <div class=" col-sm-7">
                    <input type="checkbox" checked data-toggle="toggle" data-size="small" id="IsEnabled">
                  </div>
                </div>
              </div>
            </div>
          </form>
          <p style="font-weight: bold;">API Trial Calls</p>
          <div class="alert alert-info" style="display: none;" id="MethodCallCountsGridMessage">No calls found</div>
          <div id="MethodCallCountsGrid"></div>
          <p style="font-weight: bold; margin-top: 10px;">API Trial Matches</p>
          <div class="alert alert-info" style="display: none;" id="OperationMatchCountsGridMessage">No matches found</div>
          <div id="OperationMatchCountsGrid"></div>
        </div>
      </div>
    </div>
  </div>

  <div class="modal fade" tabindex="-1" role="dialog" id="extendTrialModal">
    <div class="modal-dialog" role="document">
      <div class="modal-content">
        <div class="modal-header">
          <button type="button" class="close" data-dismiss="modal" aria-label="Close">
            <span aria-hidden="true">&times;</span>
          </button>
          <h4 class="modal-title">Extend Trial</h4>
        </div>
        <div class="modal-body">
          <div class="form-group">
            <label for="" class="col-sm-5 control-label">Maximum Number of Calls</label>
            <div class="col-sm-7">
              <input class="form-control" id="maxCalls" value="500"/>
            </div>
          </div>
        </div>
        <div class="modal-footer">
          <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
          <button type="button" class="btn btn-primary" id="extendTrialButton">Extend Trial</button>
        </div>
      </div>
    </div>
  </div>
  
  <input type="hidden" id="readUrl" value="<%= Url.Action("Read", "ApiTrialDetail", new {Area = "Clients", id = Model}) %>" />

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="DocumentStart" runat="server">
  
  <link href="//gitcdn.github.io/bootstrap-toggle/2.2.2/css/bootstrap-toggle.min.css" rel="stylesheet">
  <script src="//gitcdn.github.io/bootstrap-toggle/2.2.2/js/bootstrap-toggle.min.js"></script>
  <script src="<%= Url.Content("~/Areas/Clients/ApiTrialDetail/Scripts/ApiTrialDetail.js") %>" type="text/javascript"></script>
  
</asp:Content>