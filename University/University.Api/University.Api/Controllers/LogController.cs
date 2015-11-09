using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using University.Common.Models.Log;
using University.Constants;
using University.Context;

namespace University.Api.Controllers
{
    public class LogController : Controller
    {
        //
        // GET: /Log/
        public ActionResult Index()
        {
            UniversityContext dbContext = null;
            List<ApiDataLog> lstApiDataLog = new List<ApiDataLog>();
            try
            {
                dbContext = new UniversityContext();
                lstApiDataLog = dbContext.ApiDataLogs.Where(x => x.StatusCode == StatusCodeConstants.ACTIVE).OrderByDescending(x => x.ApiDataLogId).ToList();
            }
            catch (Exception)
            {

                throw;
            }
            return View(lstApiDataLog);
        }
	}
}