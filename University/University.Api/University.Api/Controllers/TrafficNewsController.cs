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
using University.Api.Utilities;
using University.Bussiness.Models;
using University.Bussiness.Models.ViewModel;
using University.Common.Models;
using University.Common.Models.Enums;
using University.Common.Models.Security;
using University.Constants;
using University.Context;
using University.Security.Models;
using University.Security.Models.ViewModel;
using University.Utilities;

namespace University.Api.Controllers
{
    public class TrafficNewsController : UnSecuredController
    {
        public HttpResponseMessage Get()
        {
            //_logger.Info("TrafficNews HttpGet - Called");
            UniversityContext dbcontext = null;
            List<TrafficNews_vm> lstTrafficNews = null;
            Tenant tenant = null;
            int apiDataLogId = 0;
            CurrentUser currentUser = null;
            try
            {
                tenant = CurrentTenant;
                if (tenant.HasValue())
                {
                    dbcontext = new UniversityContext();
                    lstTrafficNews = (from tN in dbContext.TrafficNews.Include("ApplicationUser")
                                   .Where(x =>
                                       x.StatusCode == StatusCodeConstants.ACTIVE
                                       && x.TenantId == tenant.TenantId)
                                      select new TrafficNews_vm
                                      {
                                          ApplicationUserId = tN.ApplicationUser.ApplicationUserId,
                                          UserName = tN.ApplicationUser.FirstName + " " + tN.ApplicationUser.LastName,
                                          FirstName = tN.ApplicationUser.FirstName,
                                          LastName = tN.ApplicationUser.LastName,
                                          EmailAddress = tN.ApplicationUser.EmailAddress,
                                          Contact = tN.ApplicationUser.Contact,
                                          StreetName = tN.StreetName,
                                          Degree = tN.Degree,
                                          Description = tN.Description,
                                          ImagePath1 = tN.ImagePath1,
                                          ImagePath2 = tN.ImagePath2,
                                          ImagePath3 = tN.ImagePath3,
                                          ImagePath4 = tN.ImagePath4,
                                          PostedDate = tN.CreatedOn.ToString()
                                      }).OrderByDescending(x => x.PostedDate).ToList();
                    if (lstTrafficNews.HasValue())
                    {
                        lstTrafficNews.ForEach(x => Separate(x));
                    }
                    return Serializer.ReturnContent(lstTrafficNews, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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

        private object Separate(TrafficNews_vm x)
        {
            if (x != null)
            {
                x.DaysAgo = TimeSpanFormat.FormatDaysAgo(x.PostedDate);
            }
            return x;
        }

        public HttpResponseMessage Get([FromUri]int applicationUserId)
        {
            //_logger.Info("TrafficNews HttpGet - Called");
            UniversityContext dbcontext = null;
            List<TrafficNews_vm> lstTrafficNews = null;
            Tenant tenant = null;
            int apiDataLogId = 0;
            CurrentUser currentUser = null;
            try
            {
                //_logger.Info("aid" + applicationUserId);
                tenant = CurrentTenant;
                if (tenant.HasValue())
                {
                    dbcontext = new UniversityContext();
                    lstTrafficNews = (from tN in dbContext.TrafficNews.Include("ApplicationUser")
                                   .Where(x =>
                                       x.ApplicationUserId == applicationUserId
                                       && x.StatusCode == StatusCodeConstants.ACTIVE
                                       && x.TenantId == tenant.TenantId)
                                      select new TrafficNews_vm
                                      {
                                          TrafficNewsId = tN.TrafficNewsId,
                                          ApplicationUserId = tN.ApplicationUser.ApplicationUserId,
                                          UserName = tN.ApplicationUser.FirstName + " " + tN.ApplicationUser.LastName,
                                          FirstName = tN.ApplicationUser.FirstName,
                                          LastName = tN.ApplicationUser.LastName,
                                          EmailAddress = tN.ApplicationUser.EmailAddress,
                                          Contact = tN.ApplicationUser.Contact,
                                          StreetName = tN.StreetName,
                                          Degree = tN.Degree,
                                          Description = tN.Description,
                                          ImagePath1 = tN.ImagePath1,
                                          ImagePath2 = tN.ImagePath2,
                                          ImagePath3 = tN.ImagePath3,
                                          ImagePath4 = tN.ImagePath4,
                                          PostedDate = tN.CreatedOn.ToString()
                                      }).OrderByDescending(x => x.PostedDate).ToList();
                    if (lstTrafficNews.HasValue())
                    {
                        lstTrafficNews.ForEach(x => Separate(x));
                    }
                    return Serializer.ReturnContent(lstTrafficNews, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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

        private List<int> GetUserId(MessageType msgType, UniversityContext dbContext, Tenant tenant)
        {
            List<int> lstIds = null;
            IQueryable<ApplicationUser> user = null;
            try
            {
                lstIds = new List<int>();
                user = dbContext.ApplicationUsers.Where(x => x.TenantId == tenant.TenantId
                    && x.StatusCode == StatusCodeConstants.ACTIVE);

                switch (msgType)
                {
                    case MessageType.All:
                        lstIds = user.Select(y => y.ApplicationUserId).ToList();
                        break;
                    case MessageType.AllFaculty:
                        lstIds = user.Where(x => x.AccountType == AccountType.Faculty)
                            .Select(y => y.ApplicationUserId).ToList();
                        break;
                    case MessageType.AllStudent:
                        lstIds = user.Where(x => x.AccountType == AccountType.Student)
                            .Select(y => y.ApplicationUserId).ToList();
                        break;
                    case MessageType.OnlyFemaleFaculty:
                        lstIds = user.Where(x => x.AccountType == AccountType.Faculty && x.Gender == Gender.Female)
                            .Select(y => y.ApplicationUserId).ToList();
                        break;
                    case MessageType.OnlyFemaleStudent:
                        lstIds = user.Where(x => x.AccountType == AccountType.Student && x.Gender == Gender.Female)
                            .Select(y => y.ApplicationUserId).ToList();
                        break;
                    case MessageType.OnlyMaleFaculty:
                        lstIds = user.Where(x => x.AccountType == AccountType.Faculty && x.Gender == Gender.Male)
                            .Select(y => y.ApplicationUserId).ToList();
                        break;
                    case MessageType.OnlyMaleStudent:
                        lstIds = user.Where(x => x.AccountType == AccountType.Student && x.Gender == Gender.Male)
                            .Select(y => y.ApplicationUserId).ToList();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {

                _logger.Error("ex.Message " + ex.Message);
                string message = (ex.InnerException != null) ? ((ex.InnerException.InnerException != null) ? ex.InnerException.InnerException.Message : ex.InnerException.ToString()) : ex.Message;
                _logger.Error("message " + message);
                string stackTrace = ex.StackTrace;
                _logger.Error("stackTrace : " + stackTrace);
            }
            return lstIds;
        }


        public HttpResponseMessage Post(ApiViewModel apiViewModel)
        {
            //_logger.Info("TrafficNews HttpPost - Called");
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
                            apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Post, "TrafficNews", data);
                            TrafficNews_vm serializedTrafficNews_vm = JsonConvert
                                .DeserializeObject<TrafficNews_vm>(apiViewModel.custom.ToString());
                            if (serializedTrafficNews_vm != null)
                            {
                                dbContext = new UniversityContext();
                                TrafficNews tN = new TrafficNews
                                {
                                    ApplicationUserId = currentUser.UserId,
                                    StreetName = serializedTrafficNews_vm.StreetName,
                                    Degree = serializedTrafficNews_vm.Degree,
                                    Description = serializedTrafficNews_vm.Description,
                                    ImagePath1 = serializedTrafficNews_vm.ImagePath1,
                                    ImagePath2 = serializedTrafficNews_vm.ImagePath2,
                                    ImagePath3 = serializedTrafficNews_vm.ImagePath3,
                                    ImagePath4 = serializedTrafficNews_vm.ImagePath4,
                                    CreatedBy = currentUser.UserId,
                                    CreatedOn = DateTime.Now,
                                    TenantId = currentUser.TenantId,
                                    StatusCode = StatusCodeConstants.ACTIVE,
                                    Language = Language.English
                                };
                                dbContext.TrafficNews.Add(tN);
                                dbContext.SaveChanges();

                                List<int> lstIds = GetUserId(MessageType.All, dbContext, tenant);
                                if (lstIds.HasValue())
                                {
                                    foreach (var item in lstIds)
                                    {
                                        if (item > 0 && item != currentUser.UserId)
                                        {
                                            Notify.LogData(currentUser, dbContext, item, Module.TrafficNews,
                                                "New Traffic News - " + tN.StreetName + " " + tN.Degree,
                                                tN.TrafficNewsId, null, "TrafficNews");
                                        }
                                    }
                                }

                                //Notify.LogData(currentUser, dbContext, currentUser.UserId, Module.TrafficNews,
                                //               currentUser.FullName + " has added new trafficnews.", tN.TrafficNewsId);

                                dbContext.SaveChanges();
                                return Serializer.ReturnContent("Traffic News Posted Successfully."
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

        public HttpResponseMessage Put(ApiViewModel apiViewModel)
        {
            //_logger.Info("TrafficNews HttpPUt - Called");
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
                            apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Put, "TrafficNews", data);
                            TrafficNews serializedTrafficNews_vm = JsonConvert
                                .DeserializeObject<TrafficNews>(apiViewModel.custom.ToString());
                            if (serializedTrafficNews_vm != null)
                            {
                                dbContext = new UniversityContext();
                                TrafficNews tN = dbContext.TrafficNews.SingleOrDefault(x => x.TrafficNewsId == serializedTrafficNews_vm.TrafficNewsId
                                    && x.StatusCode == StatusCodeConstants.ACTIVE
                                   && x.TenantId == tenant.TenantId);
                                if (tN != null)
                                {
                                    tN.StreetName = serializedTrafficNews_vm.StreetName;
                                    tN.Degree = serializedTrafficNews_vm.Degree;
                                    tN.Description = serializedTrafficNews_vm.Description;
                                    tN.ImagePath1 = serializedTrafficNews_vm.ImagePath1;
                                    tN.ImagePath2 = serializedTrafficNews_vm.ImagePath2;
                                    tN.ImagePath3 = serializedTrafficNews_vm.ImagePath3;
                                    tN.ImagePath4 = serializedTrafficNews_vm.ImagePath4;
                                    tN.LastModifiedBy = currentUser.UserId;
                                    tN.LastModifiedOn = DateTime.Now;
                                }
                                //dbContext.GoogleMaps.Add(gM);
                                dbContext.SaveChanges();
                                return Serializer.ReturnContent("Traffic News Updated Successfully."
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
            //_logger.Info("TrafficNews Delete - Called");
            CurrentUser currentUser = null;
            UniversityContext dbContext = null;
            int apiDataLogId = 0;
            Tenant tenant = null;
            string data = string.Empty;
            TrafficNews cD = null;
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
                            apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Delete, "TrafficNews", data);
                            TrafficNews serializedTrafficNews = JsonConvert
                                .DeserializeObject<TrafficNews>(apiViewModel.custom.ToString());
                            if (serializedTrafficNews != null)
                            {
                                dbContext = new UniversityContext();
                                cD = dbContext.TrafficNews.SingleOrDefault(x => x.TrafficNewsId == serializedTrafficNews.TrafficNewsId
                                    && x.StatusCode == StatusCodeConstants.ACTIVE && x.TenantId == tenant.TenantId);
                                if (cD != null)
                                {
                                    cD.StatusCode = StatusCodeConstants.INACTIVE;
                                    cD.LastModifiedBy = currentUser.UserId;
                                    cD.LastModifiedOn = DateTime.Now;
                                    dbContext.SaveChanges();

                                    return Serializer.ReturnContent("Traffic News deleted Successfully."
                                    , this.Configuration.Services.GetContentNegotiator()
                                    , this.Configuration.Formatters, this.Request);
                                }
                                else
                                {
                                    _logger.Warn("TrafficNews does not exists");
                                    return Serializer.ReturnContent("TrafficNews does not exists", this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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
