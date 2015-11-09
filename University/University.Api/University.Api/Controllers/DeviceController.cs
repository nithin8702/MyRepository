using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using University.Api.Controllers.Log;
using University.Api.Controllers.Serialize;
using University.Api.Extensions;
using University.Common.Models;
using University.Common.Models.Security;
using University.Constants;
using University.Context;
using University.Security.Models;
using University.Security.Models.ViewModel;

namespace University.Api.Controllers
{
    public class DeviceController : UnSecuredController
    {

        public HttpResponseMessage Put(ApiViewModel apiViewModel)
        {
            //_logger.Info("Device HttpPUt - Called");
            CurrentUser currentUser = null;
            UniversityContext dbContext = null;
            int apiDataLogId = 0;
            Tenant tenant = null;
            string data = string.Empty;
            try
            {
                if (apiViewModel.HasValue())
                {
                    tenant = CurrentTenant;
                    if (tenant != null)
                    {
                        Token = apiViewModel.Token;
                        currentUser = ApiUser;
                        if (currentUser.HasValue())
                        {
                            data = JsonConvert.SerializeObject(apiViewModel.custom);
                            apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Put, "Device", data);
                            ApplicationUser_vm serializedApplicationUser = JsonConvert
                                .DeserializeObject<ApplicationUser_vm>(apiViewModel.custom.ToString());
                            if (serializedApplicationUser != null)
                            {
                                dbContext = new UniversityContext();
                                ApplicationUser user = dbContext.ApplicationUsers.SingleOrDefault(x => x.ApplicationUserId == currentUser.UserId
                                    && x.StatusCode == StatusCodeConstants.ACTIVE
                                   && x.TenantId == tenant.TenantId);
                                if (user != null)
                                {
                                    user.DeviceType = serializedApplicationUser.DeviceType;
                                    //user.DeviceToken = serializedApplicationUser.DeviceToken;
                                    user.RegistrationId = serializedApplicationUser.RegistrationId;
                                    user.DeviceUDDI = serializedApplicationUser.DeviceUDDI;
                                    user.LastModifiedBy = currentUser.UserId;
                                    user.LastModifiedOn = DateTime.Now;
                                }
                                dbContext.SaveChanges();
                                return Serializer.ReturnContent(HttpConstants.Updated
                                    , this.Configuration.Services.GetContentNegotiator()
                                    , this.Configuration.Formatters, this.Request);
                            }
                            else
                            {
                                _logger.Warn(HttpConstants.InvalidInput);
                                return Serializer.ReturnContent(HttpConstants.InvalidCurrentUser, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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
