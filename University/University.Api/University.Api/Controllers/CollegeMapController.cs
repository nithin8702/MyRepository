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
    public class CollegeMapController : UnSecuredController
    {

        public HttpResponseMessage Get()
        {
            //_logger.Info("CollegeMap HttpGet - Called");
            int apiDataLogId = 0;
            UniversityContext dbcontext = new UniversityContext();
            List<CollegeMap_vm> lstCollegeMap = null;
            CurrentUser currentUser = null;
            Tenant tenant = null;
            try
            {
                tenant = CurrentTenant;
                if (tenant.HasValue())
                {
                    lstCollegeMap = (from cMap in dbcontext.CollegeMaps
                            .Include("College")
                            .Where(x => x.StatusCode == StatusCodeConstants.ACTIVE
                                && x.TenantId == tenant.TenantId
                                )
                                     select new CollegeMap_vm
                                     {
                                         CollegeMapId=cMap.CollegeMapId,
                                         CollegeId = cMap.CollegeId,
                                         CollegeName = cMap.College.CollegeName,
                                         Department = cMap.Department,
                                         Place = cMap.Place,
                                         Longitude = cMap.Longitude,
                                         Latitude = cMap.Latitude,
                                         CustomField01 = cMap.CustomField01,
                                         CustomField02 = cMap.CustomField02,
                                         CustomField03 = cMap.CustomField03,
                                         CustomField04 = cMap.CustomField04,
                                         CustomField05 = cMap.CustomField05,
                                     }).ToList();
                    //_logger.Info("lstCollegeMap count : " + lstCollegeMap.Count);
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
            return Serializer.ReturnContent(lstCollegeMap, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
        }

        public HttpResponseMessage Post(ApiViewModel apiViewModel)
        {
            //_logger.Info("CollegeMap HttpPost - Called");
            CurrentUser currentUser = null;
            UniversityContext dbContext = null;
            int apiDataLogId = 0;
            Tenant tenant = null;
            string data = string.Empty;
            CollegeMap collegeMap = null;
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
                            apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Post, "CollegeMap", data);
                            if (currentUser.AccountType == AccountType.Admin)
                            {
                                CollegeMap serializedCollegeMap = JsonConvert
                                    .DeserializeObject<CollegeMap>(apiViewModel.custom.ToString());
                                if (serializedCollegeMap != null)
                                {
                                    dbContext = new UniversityContext();
                                    collegeMap = new CollegeMap
                                    {
                                        CollegeId = serializedCollegeMap.CollegeId,
                                        Department = serializedCollegeMap.Department,
                                        Place = serializedCollegeMap.Place,
                                        Longitude = serializedCollegeMap.Longitude,
                                        Latitude = serializedCollegeMap.Latitude,
                                        CustomField01 = serializedCollegeMap.CustomField01,
                                        CustomField02 = serializedCollegeMap.CustomField02,
                                        CustomField03 = serializedCollegeMap.CustomField03,
                                        CustomField04 = serializedCollegeMap.CustomField04,
                                        CustomField05 = serializedCollegeMap.CustomField05,
                                        CreatedBy = currentUser.UserId,
                                        CreatedOn = DateTime.Now,
                                        TenantId = tenant.TenantId,
                                        StatusCode = StatusCodeConstants.ACTIVE,
                                        Language = Language.English
                                    };
                                    dbContext.CollegeMaps.Add(collegeMap);
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
            //_logger.Info("CollegeMap HttpPUt - Called");
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
                            apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Put, "CollegeMap", data);
                            CollegeMap_vm serializedCollegeMap = JsonConvert
                                .DeserializeObject<CollegeMap_vm>(apiViewModel.custom.ToString());
                            if (serializedCollegeMap != null)
                            {
                                dbContext = new UniversityContext();
                                CollegeMap cM = dbContext.CollegeMaps.SingleOrDefault(x => x.CollegeMapId == serializedCollegeMap.CollegeMapId
                                    && x.StatusCode == StatusCodeConstants.ACTIVE
                                   && x.TenantId == tenant.TenantId);
                                if (cM != null)
                                {
                                    cM.CollegeId = serializedCollegeMap.CollegeId;
                                    cM.Department = serializedCollegeMap.Department;
                                    cM.Place = serializedCollegeMap.Place;
                                    cM.Latitude = serializedCollegeMap.Latitude;
                                    cM.Longitude = serializedCollegeMap.Longitude;
                                    cM.CustomField01 = serializedCollegeMap.CustomField01;
                                    cM.CustomField02 = serializedCollegeMap.CustomField02;
                                    cM.CustomField03 = serializedCollegeMap.CustomField03;
                                    cM.CustomField04 = serializedCollegeMap.CustomField04;
                                    cM.CustomField05 = serializedCollegeMap.CustomField05;
                                    cM.LastModifiedBy = currentUser.UserId;
                                    cM.LastModifiedOn = DateTime.Now;
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

        public HttpResponseMessage Delete(ApiViewModel apiViewModel)
        {
            //_logger.Info("CollegeMap Delete - Called");
            CurrentUser currentUser = null;
            UniversityContext dbContext = null;
            int apiDataLogId = 0;
            Tenant tenant = null;
            string data = string.Empty;
            CollegeMap cD = null;
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
                            apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Delete, "CollegeMap", data);
                            CollegeMap_vm serializedCollegeMap = JsonConvert
                                .DeserializeObject<CollegeMap_vm>(apiViewModel.custom.ToString());
                            if (serializedCollegeMap != null)
                            {
                                dbContext = new UniversityContext();
                                cD = dbContext.CollegeMaps.SingleOrDefault(x => x.CollegeMapId == serializedCollegeMap.CollegeMapId
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
                                    _logger.Warn("CollegeMap does not exists");
                                    return Serializer.ReturnContent("CollegeMap does not exists", this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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
