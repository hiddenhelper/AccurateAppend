using System;

namespace AccurateAppend.Websites.Admin.Areas.Tickets.ListTickets.Models
{
    /// <summary>
    /// Used to instruct the view (that is allowing it to remain 'dumb') on the variables it needs for operation.
    /// </summary>
    public class PageSettings
    {
        /// <summary>
        /// The absolute uri value for the ZenDesk Support site for AA.
        /// </summary>
        public String ZenDesk { get; set; }

        /// <summary>
        /// The absolute or relative uri value used to query ZenDesk ticket information for use in the view.
        /// </summary>
        public String Data { get; set; }
    }
}