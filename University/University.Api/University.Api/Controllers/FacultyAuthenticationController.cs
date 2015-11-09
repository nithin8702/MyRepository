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
    public class FacultyAuthenticationController : UnSecuredController
    {
        public HttpResponseMessage Post(ApiViewModel apiViewModel)
        {
            //_logger.Info("FacultyAuthentication HttpPost - Called");
            CurrentUser currentUser = null;
            UniversityContext dbContext = null;
            ApplicationUser applicationUser = null;
            int apiDataLogId = 0;
            Tenant tenant = null;
            string data = string.Empty;
            try
            {
                if (apiViewModel.HasValue())
                {
                    tenant = CurrentTenant;
                    if (tenant.HasValue())
                    {
                        //_logger.Info("tk : "+apiViewModel.Token);
                        Token = apiViewModel.Token;
                        currentUser = ApiUser;
                        //_logger.Info("currentUser.UserId " + currentUser.UserId);
                        //_logger.Info("currentUser.TenantId " + currentUser.TenantId);
                        if (currentUser.HasValue())
                        {
                            //_logger.Info("apiViewModel.custom" + apiViewModel.custom);
                            data = JsonConvert.SerializeObject(apiViewModel.custom);
                            //_logger.Info("data" + data);
                            
                            //apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Post, "FacultyAuthentication", data);
                            if (currentUser.AccountType == AccountType.Admin)
                            {
                                //_logger.Info("starts");
                                AuthenticateUser_vm serializedApplicationUser = JsonConvert
                                   .DeserializeObject<AuthenticateUser_vm>(apiViewModel.custom.ToString());
                                if (serializedApplicationUser != null && !string.IsNullOrEmpty(serializedApplicationUser.AuthenticateToken))
                                {
                                    dbContext = new UniversityContext();
                                    //_logger.Info("token : " + serializedApplicationUser.AuthenticateToken);
                                    var drafterUser = dbContext.DraftedUsers.SingleOrDefault(x => x.TenantId == tenant.TenantId
                                        && x.Token == serializedApplicationUser.AuthenticateToken 
                                        && x.StatusCode==StatusCodeConstants.NEW);
                                    if (drafterUser!=null)
                                    {
                                        applicationUser = dbContext.ApplicationUsers
                                           .SingleOrDefault(x => x.ApplicationUserId == drafterUser.ApplicationUserId
                                               && x.AccountType == AccountType.Faculty
                                           && x.TenantId == tenant.TenantId && x.StatusCode == StatusCodeConstants.DRAFT);
                                        if (applicationUser != null)
                                        {
                                            applicationUser.StatusCode = StatusCodeConstants.ACTIVE;
                                            applicationUser.LastModifiedBy = currentUser.UserId;
                                            applicationUser.LastModifiedOn = DateTime.Now;
                                            dbContext.DraftedUsers.Remove(drafterUser);
                                            dbContext.SaveChanges();

                                            return Serializer.ReturnContent(HttpConstants.UserActivated
                                            , this.Configuration.Services.GetContentNegotiator()
                                            , this.Configuration.Formatters, this.Request);
                                        }
                                        else
                                        {
                                            _logger.Warn(HttpConstants.InvalidFaculty);
                                            return Serializer.ReturnContent(HttpConstants.InvalidFaculty, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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
                                    return Serializer.ReturnContent(HttpConstants.InvalidInput, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                                }
                            }
                            else
                            {
                                _logger.Warn(HttpConstants.NotAnAdminUser);
                                return Serializer.ReturnContent(HttpConstants.NotAnAdminUser, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                            }
                        }
                        else
                        {
                            _logger.Warn(HttpConstants.InvalidCurrentUser);
                            return Serializer.ReturnContent(HttpConstants.InvalidCurrentUser, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                        }
                    }
                    else
                    {
                        _logger.Warn(HttpConstants.InvalidTenant);
                        return Serializer.ReturnContent(HttpConstants.InvalidTenant, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                    }
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
