using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
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
    public class AdvertisementController : UnSecuredController
    {

        public HttpResponseMessage Get([FromUri]string screen = null)
        {
            //_logger.Info("Advertisement HttpGet - Called");
            UniversityContext dbcontext = null;
            List<Advertisement_vm> lstAdvertisement = null;
            Tenant tenant = null;
            int apiDataLogId = 0;
            CurrentUser currentUser = null;
            try
            {
                tenant = CurrentTenant;
                if (tenant.HasValue())
                {
                    dbcontext = new UniversityContext();
                    if (!string.IsNullOrEmpty(screen))
                    {
                        lstAdvertisement = (from ads in dbContext.Advertisements
                                       .Where(x => x.Screen == screen
                                           && x.StatusCode == StatusCodeConstants.ACTIVE
                                           && x.TenantId == tenant.TenantId)
                                            select new Advertisement_vm
                                            {
                                                AdvertisementId=ads.AdvertisementId,
                                                Screen = ads.Screen,
                                                Place = (Place)ads.Place,
                                                Frequency = (Frequency)ads.Frequency,
                                                Mode = (Mode)ads.Mode,
                                                ImagePath = ads.ImagePath,
                                                CustomField01 = ads.CustomField01,
                                                CustomField02 = ads.CustomField02,
                                                CustomField03 = ads.CustomField03,
                                                CustomField04 = ads.CustomField04,
                                                CustomField05 = ads.CustomField05,
                                            }).ToList();
                    }
                    else
                    {
                        lstAdvertisement = (from ads in dbContext.Advertisements
                                   .Where(x =>
                                       x.StatusCode == StatusCodeConstants.ACTIVE
                                       && x.TenantId == tenant.TenantId)
                                            select new Advertisement_vm
                                            {
                                                AdvertisementId = ads.AdvertisementId,
                                                Screen = ads.Screen,
                                                Place = (Place)ads.Place,
                                                Frequency = (Frequency)ads.Frequency,
                                                Mode = (Mode)ads.Mode,
                                                ImagePath = ads.ImagePath,
                                                CustomField01 = ads.CustomField01,
                                                CustomField02 = ads.CustomField02,
                                                CustomField03 = ads.CustomField03,
                                                CustomField04 = ads.CustomField04,
                                                CustomField05 = ads.CustomField05,
                                            }).ToList();
                    }

                    return Serializer.ReturnContent(lstAdvertisement, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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

        public HttpResponseMessage Post(ApiViewModel apiViewModel)
        {
            //_logger.Info("Advertisement HttpPost - Called");
            CurrentUser currentUser = null;
            UniversityContext dbContext = null;
            int apiDataLogId = 0;
            Tenant tenant = null;
            string data = string.Empty;
            Advertisement ads = null;
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
                            apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Post, "Advertisement", data);
                            if (currentUser.AccountType == AccountType.Admin)
                            {
                                Advertisement_vm serializedAdvertisement = JsonConvert
                                    .DeserializeObject<Advertisement_vm>(apiViewModel.custom.ToString());
                                if (serializedAdvertisement != null)
                                {
                                    dbContext = new UniversityContext();
                                    ads = new Advertisement
                                    {
                                        Screen = serializedAdvertisement.Screen,
                                        Place = (Place)serializedAdvertisement.Place,
                                        Frequency = (Frequency)serializedAdvertisement.Frequency,
                                        Mode = (Mode)serializedAdvertisement.Mode,
                                        ImagePath = serializedAdvertisement.ImagePath,
                                        CustomField01 = serializedAdvertisement.CustomField01,
                                        CustomField02 = serializedAdvertisement.CustomField02,
                                        CustomField03 = serializedAdvertisement.CustomField03,
                                        CustomField04 = serializedAdvertisement.CustomField04,
                                        CustomField05 = serializedAdvertisement.CustomField05,
                                        CreatedBy = currentUser.UserId,
                                        CreatedOn = DateTime.Now,
                                        TenantId = tenant.TenantId,
                                        StatusCode = StatusCodeConstants.ACTIVE,
                                        Language = Language.English
                                    };
                                    dbContext.Advertisements.Add(ads);
                                    dbContext.SaveChanges();
                                    return Serializer.ReturnContent(HttpConstants.Inserted
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

        public HttpResponseMessage Put(ApiViewModel apiViewModel)
        {
            //_logger.Info("Advertisement HttpPut - Called");
            CurrentUser currentUser = null;
            UniversityContext dbContext = null;
            int apiDataLogId = 0;
            Tenant tenant = null;
            string data = string.Empty;
            Advertisement cD = null;
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
                            apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Put, "Advertisement", data);
                            if (currentUser.AccountType != AccountType.Admin)
                            {
                                _logger.Warn(HttpConstants.NotAnAdminUser);
                                return Serializer.ReturnContent(HttpConstants.NotAnAdminUser, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                            }
                            Advertisement_vm serializedAdvertisement = JsonConvert
                                .DeserializeObject<Advertisement_vm>(apiViewModel.custom.ToString());
                            if (serializedAdvertisement != null)
                            {
                                dbContext = new UniversityContext();
                                cD = dbContext.Advertisements.SingleOrDefault(x => x.AdvertisementId == serializedAdvertisement.AdvertisementId
                                    && x.StatusCode == StatusCodeConstants.ACTIVE && x.TenantId == tenant.TenantId);
                                if (cD != null)
                                {
                                    cD.Screen = serializedAdvertisement.Screen;
                                    cD.Place = serializedAdvertisement.Place;
                                    cD.Frequency = serializedAdvertisement.Frequency;
                                    cD.Mode = serializedAdvertisement.Mode;
                                    cD.ImagePath = serializedAdvertisement.ImagePath;
                                    cD.CustomField01 = serializedAdvertisement.CustomField01;
                                    cD.CustomField02 = serializedAdvertisement.CustomField02;
                                    cD.CustomField03 = serializedAdvertisement.CustomField03;
                                    cD.CustomField04 = serializedAdvertisement.CustomField04;
                                    cD.CustomField05 = serializedAdvertisement.CustomField05;
                                    cD.LastModifiedBy = currentUser.UserId;
                                    cD.LastModifiedOn = DateTime.Now;
                                    dbContext.Entry<Advertisement>(cD).State = EntityState.Modified;
                                    dbContext.SaveChanges();

                                    return Serializer.ReturnContent(HttpConstants.Updated
                                    , this.Configuration.Services.GetContentNegotiator()
                                    , this.Configuration.Formatters, this.Request);
                                }
                                else
                                {
                                    _logger.Warn("Advertisement does not exists");
                                    return Serializer.ReturnContent("Advertisement does not exists", this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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
            catch (DbEntityValidationException ex)
            {
                foreach (var item1 in ex.EntityValidationErrors)
                {
                    foreach (var item in item1.ValidationErrors.ToList())
                    {
                        _logger.Error(item.PropertyName + " - " + item.ErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                ErrorLog.LogCustomError(currentUser, ex, apiDataLogId);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
            return Serializer.ReturnContent(HttpStatusCode.NotImplemented
                            , this.Configuration.Services.GetContentNegotiator()
                            , this.Configuration.Formatters, this.Request);
        }

        public HttpResponseMessage Delete(ApiViewModel apiViewModel)
        {
            //_logger.Info("Advertisement Delete - Called");
            CurrentUser currentUser = null;
            UniversityContext dbContext = null;
            int apiDataLogId = 0;
            Tenant tenant = null;
            string data = string.Empty;
            Advertisement cD = null;
            try
            {
                if (apiViewModel.HasValue())
                {
                    tenant = CurrentTenant;
                    if (tenant.HasValue())
                    {
                        Token = apiViewModel.Token;
                        currentUser = ApiUser;
                        if (currentUser.HasValue())
                        {
                            data = JsonConvert.SerializeObject(apiViewModel.custom);
                            apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Delete, "Advertisement", data);
                            if (currentUser.AccountType != AccountType.Admin)
                            {
                                _logger.Warn(HttpConstants.NotAnAdminUser);
                                return Serializer.ReturnContent(HttpConstants.NotAnAdminUser, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                            }
                            Advertisement_vm serializedCollege = JsonConvert
                                .DeserializeObject<Advertisement_vm>(apiViewModel.custom.ToString());
                            if (serializedCollege != null)
                            {
                                dbContext = new UniversityContext();
                                cD = dbContext.Advertisements.SingleOrDefault(x => x.AdvertisementId == serializedCollege.AdvertisementId
                                    && x.StatusCode == StatusCodeConstants.ACTIVE && x.TenantId == tenant.TenantId);
                                if (cD != null)
                                {
                                    cD.StatusCode = StatusCodeConstants.INACTIVE;
                                    cD.LastModifiedBy = currentUser.UserId;
                                    cD.LastModifiedOn = DateTime.Now;
                                    dbContext.SaveChanges();

                                    return Serializer.ReturnContent(HttpConstants.Deleted
                                    , this.Configuration.Services.GetContentNegotiator()
                                    , this.Configuration.Formatters, this.Request);
                                }
                                else
                                {
                                    _logger.Warn("Advertisement does not exists");
                                    return Serializer.ReturnContent("Advertisement does not exists", this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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
