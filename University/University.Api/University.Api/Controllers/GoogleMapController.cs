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
using University.Common.Models.ViewModel;
using University.Constants;
using University.Context;
using University.Security.Models.ViewModel;

namespace University.Api.Controllers
{
    public class GoogleMapController : UnSecuredController
    {
        public HttpResponseMessage Get()
        {
            //_logger.Info("GoogleMap HttpGet All - Called");
            UniversityContext dbcontext = null;
            List<GoogleMap_vm> lstGoogleMap = null;
            Tenant tenant = null;
            int apiDataLogId = 0;
            CurrentUser currentUser = null;
            try
            {
                tenant = CurrentTenant;
                if (tenant.HasValue())
                {
                    dbcontext = new UniversityContext();
                    lstGoogleMap = (from gM in dbContext.GoogleMaps
                                   .Where(x =>
                                       x.StatusCode == StatusCodeConstants.ACTIVE
                                       && x.TenantId == tenant.TenantId)
                                    select new GoogleMap_vm
                                    {
                                        GoogleMapId = gM.GoogleMapId,
                                        CollegeId = gM.CollegeId,
                                        CollegeName = gM.College.CollegeName,
                                        DepartmentId = gM.DepartmentId,
                                        DepartmentName = gM.Department.DepartmentName,
                                        Longitude = gM.Longitude,
                                        Latitude = gM.Latitude,
                                        CustomField01 = gM.CustomField01,
                                        CustomField02 = gM.CustomField02,
                                        CustomField03 = gM.CustomField03,
                                        CustomField04 = gM.CustomField04,
                                        CustomField05 = gM.CustomField05,
                                    }).ToList();
                    return Serializer.ReturnContent(lstGoogleMap, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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
            //_logger.Info("GoogleMaps HttpPost - Called");
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
                            apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Post, "GoogleMaps", data);
                            GoogleMap_vm serializedGoogleMap = JsonConvert
                                .DeserializeObject<GoogleMap_vm>(apiViewModel.custom.ToString());
                            if (serializedGoogleMap != null)
                            {
                                dbContext = new UniversityContext();
                                GoogleMap gM = new GoogleMap
                                {
                                    CollegeId = serializedGoogleMap.CollegeId,
                                    DepartmentId = serializedGoogleMap.DepartmentId,
                                    Latitude = serializedGoogleMap.Latitude,
                                    Longitude = serializedGoogleMap.Longitude,
                                    CustomField01 = serializedGoogleMap.CustomField01,
                                    CustomField02 = serializedGoogleMap.CustomField02,
                                    CustomField03 = serializedGoogleMap.CustomField03,
                                    CustomField04 = serializedGoogleMap.CustomField04,
                                    CustomField05 = serializedGoogleMap.CustomField05,
                                    CreatedBy = currentUser.UserId,
                                    CreatedOn = DateTime.Now,
                                    TenantId = currentUser.TenantId,
                                    StatusCode = StatusCodeConstants.ACTIVE,
                                    Language = Language.English
                                };
                                dbContext.GoogleMaps.Add(gM);
                                dbContext.SaveChanges();
                                return Serializer.ReturnContent(HttpConstants.Inserted
                                    , this.Configuration.Services.GetContentNegotiator()
                                    , this.Configuration.Formatters, this.Request);
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

        public HttpResponseMessage Put(ApiViewModel apiViewModel)
        {
            //_logger.Info("GoogleMaps HttpPUt - Called");
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
                            apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Put, "GoogleMaps", data);
                            GoogleMap_vm serializedGoogleMap = JsonConvert
                                .DeserializeObject<GoogleMap_vm>(apiViewModel.custom.ToString());
                            if (serializedGoogleMap != null)
                            {
                                dbContext = new UniversityContext();
                                GoogleMap gM = dbContext.GoogleMaps.SingleOrDefault(x => x.GoogleMapId == serializedGoogleMap.GoogleMapId && x.StatusCode == StatusCodeConstants.ACTIVE
                                   && x.TenantId == tenant.TenantId);
                                if (gM != null)
                                {
                                    gM.CollegeId = serializedGoogleMap.CollegeId;
                                    gM.DepartmentId = serializedGoogleMap.DepartmentId;
                                    gM.Latitude = serializedGoogleMap.Latitude;
                                    gM.Longitude = serializedGoogleMap.Longitude;
                                    gM.CustomField01 = serializedGoogleMap.CustomField01;
                                    gM.CustomField02 = serializedGoogleMap.CustomField02;
                                    gM.CustomField03 = serializedGoogleMap.CustomField03;
                                    gM.CustomField04 = serializedGoogleMap.CustomField04;
                                    gM.CustomField05 = serializedGoogleMap.CustomField05;
                                    gM.LastModifiedBy = currentUser.UserId;
                                    gM.LastModifiedOn = DateTime.Now;
                                }
                                //dbContext.GoogleMaps.Add(gM);
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

        public HttpResponseMessage Delete(ApiViewModel apiViewModel)
        {
            //_logger.Info("GoogleMap Delete - Called");
            CurrentUser currentUser = null;
            UniversityContext dbContext = null;
            int apiDataLogId = 0;
            Tenant tenant = null;
            string data = string.Empty;
            GoogleMap cD = null;
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
                            apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Delete, "GoogleMap", data);
                            GoogleMap serializedGoogleMap = JsonConvert
                                .DeserializeObject<GoogleMap>(apiViewModel.custom.ToString());
                            if (serializedGoogleMap != null)
                            {
                                dbContext = new UniversityContext();
                                cD = dbContext.GoogleMaps.SingleOrDefault(x => x.GoogleMapId == serializedGoogleMap.GoogleMapId
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
                                    _logger.Warn("GoogleMap does not exists");
                                    return Serializer.ReturnContent("GoogleMap does not exists", this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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
