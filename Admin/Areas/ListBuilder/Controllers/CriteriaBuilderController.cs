using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AccurateAppend.ListBuilder.Models;
using Newtonsoft.Json;

namespace AccurateAppend.Websites.Admin.Areas.ListBuilder.Controllers
{
    [Authorize()]
    public class CriteriaBuilderController : Controller
    {
        public ActionResult Start()
        {
            return this.RedirectToAction(nameof(this.Index), new {id = Guid.NewGuid()});
        }

        public ActionResult Index(Guid id)
        {
            var listCriteria = new ListCriteria();
            listCriteria.RequestId = id;

            return View(listCriteria);
        }

        /// <summary>
        /// Loads UI using Json from serilaized <seealso cref="ListCriteria"/> object.
        /// </summary>
        public ActionResult LoadFromFile()
        {
            return View();
        }

        /// <summary>
        /// Loads UI using Json from serilaized <seealso cref="ListCriteria"/> object.
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        [HttpPost()]
        public ActionResult LoadFromFile(IEnumerable<HttpPostedFileBase> files)
        {
            if (files == null) return Content("");
            var file = files.First();
            if (file == null) return Content("");
            var reader = new StreamReader(file.InputStream);
            var json = reader.ReadToEnd();
            var list = JsonConvert.DeserializeObject<ListCriteria>(json);
            return View("Index", JsonConvert.DeserializeObject<ListCriteria>(json));
        }
    }
}