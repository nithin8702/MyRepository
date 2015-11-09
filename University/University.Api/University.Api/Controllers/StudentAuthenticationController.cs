using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using University.Api.Controllers.Log;
using University.Api.Controllers.Serialize;
using University.Api.Extensions;
using University.Bussiness.Models.ViewModel;
using University.Common.Models;
using University.Common.Models.Enums;
using University.Common.Models.Security;
using University.Constants;
using University.Context;
using University.Security.Models;
using University.Security.Models.ViewModel;

namespace University.Api.Controllers
{
    public class StudentAuthenticationController : UnSecuredController
    {
        public HttpResponseMessage Post(ApiViewModel apiViewModel)
        {
            //_logger.Info("StudentAuthentication HttpPost - Called");
            CurrentUser currentUser = null;
            UniversityContext dbContext = null;
            ApplicationUser applicationUser = null;
            int apiDataLogId = 0;
            //Tenant tenant = null;
            string data = string.Empty;
            try
            {
                if (apiViewModel.HasValue())
                {
                    //tenant = CurrentTenant;
                    //if (tenant.HasValue())
                    //{
                    //Token = apiViewModel.Token;
                    data = JsonConvert.SerializeObject(apiViewModel.custom);
                    apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Post, "StudentAuthentication", data);
                    AuthenticateUser_vm serializedApplicationUser = JsonConvert
                           .DeserializeObject<AuthenticateUser_vm>(apiViewModel.custom.ToString());
                    if (serializedApplicationUser != null && !string.IsNullOrEmpty(serializedApplicationUser.AuthenticateToken))
                    {
                        dbContext = new UniversityContext();
                        var drafterUser = dbContext.DraftedUsers.SingleOrDefault(x =>
                                    x.Token == serializedApplicationUser.AuthenticateToken
                                    && x.StatusCode == StatusCodeConstants.NEW);
                        if (drafterUser != null)
                        {
                            if (drafterUser.ExpiryDate < DateTime.Now)
                            {
                                _logger.Warn("Authentication Expired");
                                return Serializer.ReturnContent("Authentication Expired", this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                            }
                            else
                            {
                                applicationUser = dbContext.ApplicationUsers
                               .SingleOrDefault(x => x.ApplicationUserId == drafterUser.ApplicationUserId
                                   && x.AccountType == AccountType.Student
                               && x.TenantId == drafterUser.TenantId && x.StatusCode == StatusCodeConstants.DRAFT);
                                if (applicationUser != null)
                                {
                                    applicationUser.StatusCode = StatusCodeConstants.ACTIVE;
                                    applicationUser.LastModifiedBy = drafterUser.ApplicationUserId;
                                    applicationUser.LastModifiedOn = DateTime.Now;
                                    dbContext.DraftedUsers.Remove(drafterUser);
                                    dbContext.SaveChanges();

                                    return Serializer.ReturnContent(HttpConstants.UserActivated
                                    , this.Configuration.Services.GetContentNegotiator()
                                    , this.Configuration.Formatters, this.Request);
                                }
                                else
                                {
                                    _logger.Warn(HttpConstants.UserNotExists);
                                    return Serializer.ReturnContent(HttpConstants.UserNotExists, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                                }
                            }
                        }
                        else
                        {
                            _logger.Warn(HttpConstants.InvalidDrafterUser);
                            return Serializer.ReturnContent(HttpConstants.InvalidDrafterUser, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                        }
                    }
                    else
                    {
                        _logger.Warn(HttpConstants.InvalidInput);
                        return Serializer.ReturnContent(HttpConstants.InvalidCurrentUser, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                    }
                    //}
                    //else
                    //{
                    //    _logger.Warn(HttpConstants.InvalidTenant);
                    //    return Serializer.ReturnContent(HttpConstants.InvalidTenant, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                    //}
                }
                else
                {
                    _logger.Warn(HttpConstants.InvalidApiViewModel);
                    return Serializer.ReturnContent(HttpConstants.InvalidApiViewModel, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                ErrorLog.LogCustomError(currentUser, ex, apiDataLogId);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

    }
}
