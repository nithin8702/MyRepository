using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace University.Api.Controllers
{
    public class SubscribeController : UnSecuredController
    {
        //
        // GET: /Subscribe/

        [HttpPost]
        public HttpResponseMessage Post()
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

        /// <summary>
        /// unsubscribe
        /// - based on doctor id ?
        /// - based on class id ?
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public HttpResponseMessage Delete(int id)
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

            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
