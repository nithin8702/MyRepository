using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace University.Api.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return new FilePathResult("~/Index.php","text/html");
        }
    }
}
