using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using University.Api.Controllers.Log;
using University.Api.Controllers.Serialize;
using University.Api.Extensions;
using University.Api.Utilities;
using University.Bussiness.Models;
using University.Bussiness.Models.ViewModel;
using University.Common.Models;
using University.Common.Models.Enums;
using University.Common.Models.Security;
using University.Constants;
using University.Context;
using University.Security.Models.ViewModel;
using University.Utilities;

namespace University.Api.Controllers
{
    public class FileUploadController : UnSecuredController
    {
        public HttpResponseMessage Get()
        {
            //_logger.Info("FileUpload HttpPost - Called");
            string name = "university";
            return Serializer.ReturnContent(name, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);

        }
        public HttpResponseMessage Post()
        {
            //_logger.Info("FileUpload HttpPost - Called");
            CurrentUser currentUser = null;
            int apiDataLogId = 0;
            Tenant tenant = null;
            string data = string.Empty;
            string subPath = "~/UploadedFiles/";
            string fileName = string.Empty;
            UniversityContext Context = new UniversityContext();
            string fileSavePath = "";
            string fileReturnPath = "";
            try
            {
                var tenantToken = HttpContext.Current.Request.Form["TenantToken"];
                var tenanttmp = Context.TenantTokens.Include("Tenant").SingleOrDefault(x => x.Token == tenantToken && x.StatusCode == StatusCodeConstants.ACTIVE);
                if (tenanttmp != null)
                {
                    tenant = tenanttmp.Tenant;
                    if (HttpContext.Current.Request.Files.AllKeys.Count() > 0)
                    {
                        // Get the uploaded image from the Files collection
                        var httpPostedFile = HttpContext.Current.Request.Files["UploadedImage"];
                        if (httpPostedFile != null)
                        {
                            subPath += tenant.TenantId.ToString() + "/" + Guid.NewGuid().ToString("D");
                            bool exists = System.IO.Directory.Exists(HttpContext.Current.Server.MapPath(subPath));
                            if (!exists)
                                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(subPath));
                            fileName = httpPostedFile.FileName;
                            fileSavePath = Path.Combine(HttpContext.Current.Server.MapPath(subPath), fileName);
                            httpPostedFile.SaveAs(fileSavePath);
                            fileReturnPath = HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.AbsolutePath, "") + subPath.Remove(0, 1) + "/" + fileName;
                        }
                    }
                }
                else
                {
                    _logger.Warn(HttpConstants.InvalidTenant);
                    return Serializer.ReturnContent(HttpConstants.InvalidTenant, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                ErrorLog.LogCustomError(currentUser, ex, apiDataLogId);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
            return Serializer.ReturnContent(fileReturnPath, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
        }
    }
}
