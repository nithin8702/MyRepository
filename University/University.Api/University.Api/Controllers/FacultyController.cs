using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace University.Api.Controllers
{
    public class FacultyController : UnSecuredController
    {
        //
        // GET: /Doctor/

        [HttpGet]
        [ActionName("Profile")]
        public HttpResponseMessage View()
        {
            try
            {
                //do job
                //dbContext.ApplicationUsers.Add(user);
            }
            catch (Exception)
            {
                //log exception.
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }

            return new HttpResponseMessage(HttpStatusCode.Created);
        }

        [HttpGet]
        public HttpResponseMessage ContactUs()
        {
            try
            {
                //do job
                //dbContext.ApplicationUsers.Add(user);
            }
            catch (Exception)
            {
                //log exception.
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }

            return new HttpResponseMessage(HttpStatusCode.Created);
        }

        [HttpGet]
        public HttpResponseMessage MyStudents()
        {
            try
            {
                //do job
                //dbContext.ApplicationUsers.Add(user);
            }
            catch (Exception)
            {
                //log exception.
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }

            return new HttpResponseMessage(HttpStatusCode.Created);
        }

    }
}
