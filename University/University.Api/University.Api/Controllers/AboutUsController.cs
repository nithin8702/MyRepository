using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using University.Api.Controllers.Log;
using University.Api.Controllers.Serialize;
using University.Api.Extensions;
using University.Common.Models;
using University.Common.Models.Security;
using University.Constants;
using University.Context;

namespace University.Api.Controllers
{
    public class AboutUsController : UnSecuredController
    {
        public HttpResponseMessage Get([FromUri]CurrentUser currentUser)
        {
            //_logger.Info("AboutUs HttpGet - Called");
            int apiDataLogId = 0;
            UniversityContext dbcontext = null;
            AboutUs data = null;
            Tenant tenant = null;
            try
            {
                tenant = CurrentTenant;
                if (tenant!=null)
                {
                    if (currentUser.HasValue())
                    {
                        dbcontext = new UniversityContext();
                        data = dbcontext.AboutUs
                            .SingleOrDefault(x => x.StatusCode == StatusCodeConstants.ACTIVE
                                && x.TenantId == currentUser.TenantId
                                );
                        //_logger.Info("data return : " + data.AboutUsId);
                    }   
                }                
                else
                    _logger.Warn("not valid tenant");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                ErrorLog.LogCustomError(currentUser, ex, apiDataLogId);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
            return Serializer.ReturnContent(data, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
        }
    }
}
