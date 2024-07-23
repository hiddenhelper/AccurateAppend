using System;
using System.Collections.Generic;

namespace AccurateAppend.Websites.Admin.Areas.Clients.SearchClients.Models
{
    public class SearchResultModel
    {
        public SearchResultModel()
        {
            this.Users = new List<UserSearchResult>();
            this.Leads = new List<DomainModel.ReadModel.LeadView>();
        }

        public IList<UserSearchResult> Users { get; }

        public IList<DomainModel.ReadModel.LeadView> Leads { get; }
    }

    public class UserSearchResult
    {
       public Guid ApplicationId { get; set; }

        public Guid UserId { get; set; }

        public String UserName { get; set; }

        public String BusinessName { get; set; }

        public String Email { get; set; }

        public String FirstName { get; set; }

        public String LastName { get; set; }

        public DateTime LastActivityDate { get; set; }
        public String DetailUrl { get; set; }
        public String NewDealUrl { get; set; }
    }
}