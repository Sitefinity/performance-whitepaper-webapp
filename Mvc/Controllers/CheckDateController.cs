using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;

namespace SitefinityWebApp.Mvc.Controllers
{
    [ControllerToolboxItem(Name = "CheckDate", Title = "CheckDate", SectionName = "CheckDate", CssClass = "sfMvcIcn")]
    public class CheckDateController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}