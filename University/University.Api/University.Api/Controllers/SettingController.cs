using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using University.Api.Controllers.Log;
using University.Api.Controllers.Serialize;
using University.Api.Extensions;
using University.Common.Models;
using University.Common.Models.Enums;
using University.Common.Models.Security;
using University.Constants;
using University.Context;
using University.Security.Models;
using University.Security.Models.ViewModel;

namespace University.Api.Controllers
{
    public class SettingController : UnSecuredController
    {
        public HttpResponseMessage Get([FromUri]int applicationUserId)
        {
            //_logger.Info("Setting HttpGet - Called");
            UniversityContext dbcontext = null;
            Setting_vm lstSetting = null;
            Tenant tenant = null;
            int apiDataLogId = 0;
            CurrentUser currentUser = null;
            try
            {
                tenant = CurrentTenant;
                if (tenant.HasValue())
                {
                    dbcontext = new UniversityContext();
                    var data = dbcontext.Settings.SingleOrDefault(x =>
                                       x.ApplicationUserId == applicationUserId
                                       && x.StatusCode == StatusCodeConstants.ACTIVE
                                       && x.TenantId == tenant.TenantId);
                    if (data != null)
                    {
                        lstSetting = new Setting_vm
                        {
                            SettingId = data.SettingId,
                            IsNotify = data.IsNotify,
                            IsBookCorner = data.IsBookCorner,
                            IsBroadcast = data.IsBroadcast,
                            IsTrafficNews = data.IsTrafficNews,
                            IsMessage = data.IsMessage,
                            IsChat = data.IsChat,
                        };
                    }
                    return Serializer.ReturnContent(lstSetting, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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
                if (!currentUser.HasValue())
                {
                    currentUser = new CurrentUser { TenantId = tenant.TenantId };
                }
                ErrorLog.LogCustomError(currentUser, ex, apiDataLogId);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        //public HttpResponseMessage Post(ApiViewModel apiViewModel)
        //{
        //    _logger.Info("Setting HttpPost - Called");
        //    CurrentUser currentUser = null;
        //    UniversityContext dbContext = null;
        //    int apiDataLogId = 0;
        //    Tenant tenant = null;
        //    string data = string.Empty;
        //    try
        //    {
        //        if (apiViewModel.HasValue())
        //        {
        //            tenant = CurrentTenant;
        //            if (tenant != null)
        //            {
        //                Token = apiViewModel.Token;
        //                currentUser = ApiUser;
        //                if (currentUser.HasValue())
        //                {
        //                    data = JsonConvert.SerializeObject(apiViewModel.custom);
        //                    apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Post, "Setting", data);
        //                    Setting_vm serializedSetting_vm = JsonConvert
        //                        .DeserializeObject<Setting_vm>(apiViewModel.custom.ToString());
        //                    if (serializedSetting_vm != null)
        //                    {
        //                        dbContext = new UniversityContext();
        //                        Setting sG = new Setting
        //                        {
        //                            IsNotify = serializedSetting_vm.IsNotify,
        //                            IsBookCorner = serializedSetting_vm.IsBookCorner,
        //                            IsBroadcast = serializedSetting_vm.IsBroadcast,
        //                            IsTrafficNews = serializedSetting_vm.IsTrafficNews,
        //                            IsMessage = serializedSetting_vm.IsMessage,
        //                            CreatedBy = currentUser.UserId,
        //                            CreatedOn = DateTime.Now,
        //                            TenantId = currentUser.TenantId,
        //                            StatusCode = StatusCodeConstants.ACTIVE,
        //                            Language = Language.English
        //                        };
        //                        dbContext.Settings.Add(sG);
        //                        dbContext.SaveChanges();
        //                        return Serializer.ReturnContent(HttpConstants.Inserted
        //                            , this.Configuration.Services.GetContentNegotiator()
        //                            , this.Configuration.Formatters, this.Request);
        //                    }
        //                    else
        //                    {
        //                        _logger.Warn(HttpConstants.InvalidInput);
        //                        return Serializer.ReturnContent(HttpConstants.InvalidCurrentUser, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
        //                    }

        //                }
        //                else
        //                {
        //                    _logger.Warn(HttpConstants.InvalidCurrentUser);
        //                    return Serializer.ReturnContent(HttpConstants.InvalidCurrentUser, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
        //                }
        //            }
        //            else
        //            {
        //                _logger.Warn(HttpConstants.InvalidTenant);
        //                return Serializer.ReturnContent(HttpConstants.InvalidTenant, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
        //            }
        //        }
        //        else
        //        {
        //            _logger.Warn(HttpConstants.InvalidApiViewModel);
        //            return Serializer.ReturnContent(HttpConstants.InvalidApiViewModel, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Error(ex.Message);
        //        ErrorLog.LogCustomError(currentUser, ex, apiDataLogId);
        //        return new HttpResponseMessage(HttpStatusCode.InternalServerError);
        //    }
        //}

        public HttpResponseMessage Put(ApiViewModel apiViewModel)
        {
            //_logger.Info("Setting HttpPUt - Called");
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
                            apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Put, "Setting", data);
                            Setting_vm serializedSetting_vm = JsonConvert
                                .DeserializeObject<Setting_vm>(apiViewModel.custom.ToString());
                            if (serializedSetting_vm != null)
                            {
                                dbContext = new UniversityContext();
                                Setting gM = dbContext.Settings.SingleOrDefault(x => x.SettingId == serializedSetting_vm.SettingId && x.StatusCode == StatusCodeConstants.ACTIVE
                                   && x.TenantId == tenant.TenantId);
                                if (gM != null)
                                {
                                    gM.IsNotify = serializedSetting_vm.IsNotify;
                                    gM.IsBookCorner = serializedSetting_vm.IsBookCorner;
                                    gM.IsBroadcast = serializedSetting_vm.IsBroadcast;
                                    gM.IsTrafficNews = serializedSetting_vm.IsTrafficNews;
                                    gM.IsMessage = serializedSetting_vm.IsMessage;
                                    gM.IsChat = serializedSetting_vm.IsChat;
                                    gM.LastModifiedBy = currentUser.UserId;
                                    gM.LastModifiedOn = DateTime.Now;
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
