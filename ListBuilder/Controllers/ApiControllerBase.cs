using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using DAL.DataAccess;

namespace AccurateAppend.ListBuilder.Controllers
{
    /// <summary>
    /// Contains the shared logic for the controller and establishes a required API.
    /// </summary>
    public class ApiControllerBase : Controller
    {
        /// <summary>
        /// Queries for County FIPS data by state.
        /// </summary>
        public virtual async Task<ActionResult> GetCounties(Int32[] state, CancellationToken cancellation)
        {
            if (!state.Any()) return this.Json(new String[0], JsonRequestBehavior.AllowGet);

            var recs = (await ZipCode.GetRecordsByStateAsync(state, cancellation))
                .Select(a => new { Fips = a.CountyFIPS, Name = a.County, a.State, a.StateFIPS })
                .Distinct()
                .OrderBy(a => a.Name);

            return this.Json(recs, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Queries for City FIPS data by state.
        /// </summary>
        public virtual async Task<ActionResult> GetCities(Int32[] state, CancellationToken cancellation)
        {
            if (!state.Any()) return this.Json(new String[0], JsonRequestBehavior.AllowGet);

            var recs = (await ZipCode.GetRecordsByStateAsync(state, cancellation))
                .Select(a => new { Name = a.City, a.State, a.StateFIPS })
                .Distinct()
                .OrderBy(a => a.Name);

            return this.Json(recs, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Queries for suported state geography.
        /// </summary>
        public virtual ActionResult GetStates()
        {
            var recs = StatesHelper.GetStates()
                .Select(a => new { a.Fips, a.StateFullName, a.Abbreviation })
                .Distinct()
                .OrderBy(a => a.Abbreviation);

            return this.Json(recs, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Queries for Zip Code FIPS data by state.
        /// </summary>
        public virtual async Task<ActionResult> GetZips(Int32[] state, CancellationToken cancellation)
        {
            if (!state.Any()) return this.Json(new string[0], JsonRequestBehavior.AllowGet);

            var recs = (await ZipCode.GetRecordsByStateAsync(state, cancellation))
                .Select(a => new { Name = a.Zip, a.State, a.StateFIPS })
                .Distinct()
                .OrderBy(a => a.State)
                .ThenBy(a => a.Name);

            return this.Json(recs, JsonRequestBehavior.AllowGet);
        }
    }
}
