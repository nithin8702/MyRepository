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
    public class ErrorController : Controller
    {
        //
        // GET: /Error/
        public ActionResult Index()
        {
            UniversityContext dbContext = null;
            List<ApiErrorLog> lstApiErrorLog = new List<ApiErrorLog>();
            try
            {
                dbContext = new UniversityContext();
                lstApiErrorLog = dbContext.ApiErrorLogs.Where(x => x.StatusCode == StatusCodeConstants.ACTIVE).OrderByDescending(x=>x.ApiErrorLogId).ToList();
            }
            catch (Exception)
            {
                
                throw;
            }
            return View(lstApiErrorLog);
        }
	}
}