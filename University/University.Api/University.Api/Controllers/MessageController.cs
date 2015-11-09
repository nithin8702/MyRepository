using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace University.Api.Controllers
{
    public class MessageController : UnSecuredController
    {
        //
        // GET: /Message/

        [HttpPost]
        public HttpResponseMessage Compose()
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

    }
}
