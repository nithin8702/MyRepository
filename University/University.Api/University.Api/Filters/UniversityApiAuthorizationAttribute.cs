using System;
using System.Web;
using University.Api.Controllers;
using University.Common.Models;
using University.Common.Models.Security;
using University.Context;
using System.Linq;
using System.Collections.Specialized;
using Newtonsoft.Json;
using University.Constants;
using System.Net.Http;
using System.Net;
using System.Web.Http;

namespace University.Api.Filters
{
    public class UniversityApiAuthorizationAttribute : UnSecuredApiController
    {
        public override bool Match(object obj)
        {
            return base.Match(obj);
        }

        public override bool IsDefaultAttribute()
        {
            return base.IsDefaultAttribute();
        }

        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            //_logger.Info("OnAuthorization called");
            //_logger.Info("Request HostName: " + actionContext.Request.Headers.Host);
            bool isValid = false;
            string hostName = string.Empty;
            string controllerName = actionContext.ControllerContext.ControllerDescriptor.ControllerName;
            string verbName = actionContext.Request.Method.ToString();
            hostName = actionContext.Request.Headers.Host;
            //_logger.Info("cName" + controllerName);
            //_logger.Info("verbName" + verbName);
            if (controllerName == "Test" || controllerName == "Login" || controllerName == "SecurityQuestions"
                || controllerName == "SecurityAnswers" || controllerName == "Tenant" || controllerName == "StudentAuthentication"
                || controllerName == "UsersSecurity" || controllerName == "ForgotPassword" || controllerName == "User" || controllerName == "FileUpload" || controllerName == "FileDownload")
            {
                return;
            }
            isValid = AssignTenant(hostName);
            if (isValid && verbName.ToLower() == VerbConstants.Get.ToLower())
            {
                var queryCollection = HttpUtility.ParseQueryString(actionContext.Request.RequestUri.Query);
                //_logger.Info("UserId : " + queryCollection["UserId"]);
                if (controllerName == "Tenant")
                {
                    return;
                }
            }
            if (!isValid)
            {
                //_logger.Warn("Not Authorized");
                var unAuthorizeMessage = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                unAuthorizeMessage.Headers.Add("IsAuthorized", "False");
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden);
                return;
            }
        }

        private bool AssignTenant(string hostName)
        {
            bool isValid = false;
            UniversityContext dbContext = null;
            //_logger.Info("hn" + hostName);
            Tenant tenant = null;
            try
            {
                if (!string.IsNullOrEmpty(hostName))
                {
                    dbContext = new UniversityContext();
                    //_logger.Info("sql :" + dbContext.Database.Connection.ConnectionString);
                    var token = HttpContext.Current.Request.Headers.Get("Token");
                    if (token != null)
                    {
                        //_logger.Info("Tenant Token : " + token);
                        var tenantToken = dbContext.TenantTokens.Include("Tenant").SingleOrDefault(x => x.Token == token && x.StatusCode == StatusCodeConstants.ACTIVE);
                        if (tenantToken != null && tenantToken.Tenant != null)
                        {
                            tenant = tenantToken.Tenant;
                        }
                        else
                        {
                            //_logger.Info("tenantToken is empty.");
                        }
                    }
                    //if (tenant==null)
                    //{
                    //    _logger.Info("tenantToken is empty");
                    //    tenant = dbContext.Tenants.SingleOrDefault(x => x.HostName == hostName
                    //    && x.StatusCode == StatusCodeConstants.ACTIVE);
                    //}
                    if (tenant != null)
                    {
                        isValid = true;
                        //_logger.Info("Valid Tenant");
                        string data = JsonConvert.SerializeObject(tenant);
                        HttpContext.Current.Request.Headers.Add("tenant", data);
                    }
                    else
                        _logger.Info("Tenant Empty");
                }
                else
                    _logger.Warn("empty hostname");
                if (!isValid)
                {
                    //_logger.Info("not valid tenant");
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Er" + ex.Message);
            }
            return isValid;
        }
    }
}