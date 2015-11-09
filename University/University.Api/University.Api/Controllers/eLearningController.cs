using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
using University.Security.Models.ViewModel;
using University.Utilities;

namespace University.Api.Controllers
{
    public class ELearningController : UnSecuredController
    {
        public HttpResponseMessage Get([FromUri]int applicationUserId)
        {
            //_logger.Info("ELearning HttpGet All - Called");
            UniversityContext dbcontext = null;
            List<ELearning_vm> ELearning = null;
            Tenant tenant = null;
            int apiDataLogId = 0;
            CurrentUser currentUser = null;
            try
            {
                tenant = CurrentTenant;
                if (tenant.HasValue())
                {
                    dbcontext = new UniversityContext();
                    ELearning = (from gM in dbContext.ELearnings
                                   .Where(x =>
                                       (x.ApplicationUserId == applicationUserId ||
                                       x.ApplicationUser.AccountType == AccountType.Admin)
                                       && x.StatusCode == StatusCodeConstants.ACTIVE
                                       && x.TenantId == tenant.TenantId)
                                 select new ELearning_vm
                                 {
                                     ELearningId=gM.ELearningId,
                                     ApplicationUserId = gM.ApplicationUser.ApplicationUserId,
                                     UserName = gM.ApplicationUser.FirstName + " " + gM.ApplicationUser.LastName,
                                     FirstName = gM.ApplicationUser.FirstName,
                                     LastName = gM.ApplicationUser.LastName,
                                     EmailAddress = gM.ApplicationUser.EmailAddress,
                                     Contact = gM.ApplicationUser.Contact,
                                     Title = gM.Title,
                                     Link = gM.Link,
                                     Website = gM.Website,
                                     Desciption = gM.Desciption,
                                     ImageUploadPath = gM.ImageUploadPath,
                                     PostedDate = gM.CreatedOn.ToString(),
                                     //DaysAgo = DbFunctions.DiffDays(gM.CreatedOn, DateTime.Today).Value,
                                 }).ToList();
                    if (ELearning.HasValue())
                    {
                        ELearning.ForEach(x => Separate(x));
                    }
                    return Serializer.ReturnContent(ELearning, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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

        private object Separate(ELearning_vm x)
        {
            if (x != null)
            {
                x.DaysAgo = TimeSpanFormat.FormatDaysAgo(x.PostedDate);
            }
            return x;
        }

        public HttpResponseMessage Post(ApiViewModel apiViewModel)
        {
            //_logger.Info("ELearning HttpPost - Called");
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
                            apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Post, "ELearning", data);
                            ELearning_vm serializedELearning = JsonConvert
                                .DeserializeObject<ELearning_vm>(apiViewModel.custom.ToString());
                            if (serializedELearning != null)
                            {
                                dbContext = new UniversityContext();
                                ELearning eL = new ELearning
                                {
                                    ApplicationUserId = currentUser.UserId,
                                    Title = serializedELearning.Title,
                                    Link = serializedELearning.Link,
                                    Website = serializedELearning.Website,
                                    Desciption = serializedELearning.Desciption,
                                    ImageUploadPath = serializedELearning.ImageUploadPath,
                                    CreatedBy = currentUser.UserId,
                                    CreatedOn = DateTime.Now,
                                    TenantId = currentUser.TenantId,
                                    StatusCode = StatusCodeConstants.ACTIVE,
                                    Language = Language.English
                                };
                                dbContext.ELearnings.Add(eL);
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
            //_logger.Info("eLearning HttpPUt - Called");
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
                            apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Put, "ELearning", data);
                            ELearning_vm serializedELearning_vm = JsonConvert
                                .DeserializeObject<ELearning_vm>(apiViewModel.custom.ToString());
                            if (serializedELearning_vm != null)
                            {
                                dbContext = new UniversityContext();
                                ELearning eL = dbContext.ELearnings.SingleOrDefault(x => x.ELearningId == serializedELearning_vm.ELearningId
                                    && x.StatusCode == StatusCodeConstants.ACTIVE
                                   && x.TenantId == tenant.TenantId);
                                if (eL != null)
                                {
                                    eL.Title = serializedELearning_vm.Title;
                                    eL.Link = serializedELearning_vm.Link;
                                    eL.Website = serializedELearning_vm.Website;
                                    eL.Desciption = serializedELearning_vm.Desciption;
                                    eL.ImageUploadPath = serializedELearning_vm.ImageUploadPath;
                                    eL.LastModifiedBy = currentUser.UserId;
                                    eL.LastModifiedOn = DateTime.Now;
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
            //_logger.Info("ELearning Delete - Called");
            CurrentUser currentUser = null;
            UniversityContext dbContext = null;
            int apiDataLogId = 0;
            Tenant tenant = null;
            string data = string.Empty;
            ELearning eL = null;
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
                            apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Delete, "ELearning", data);
                            ELearning_vm serializedELearning = JsonConvert
                                .DeserializeObject<ELearning_vm>(apiViewModel.custom.ToString());
                            if (serializedELearning != null)
                            {
                                dbContext = new UniversityContext();
                                eL = dbContext.ELearnings.SingleOrDefault(x => x.ELearningId == serializedELearning.ELearningId
                                    && x.StatusCode == StatusCodeConstants.ACTIVE && x.TenantId == tenant.TenantId);
                                if (eL != null)
                                {
                                    eL.StatusCode = StatusCodeConstants.INACTIVE;
                                    eL.LastModifiedBy = currentUser.UserId;
                                    eL.LastModifiedOn = DateTime.Now;
                                    dbContext.SaveChanges();

                                    return Serializer.ReturnContent(HttpConstants.Deleted
                                    , this.Configuration.Services.GetContentNegotiator()
                                    , this.Configuration.Formatters, this.Request);
                                }
                                else
                                {
                                    _logger.Warn("ELearning does not exists");
                                    return Serializer.ReturnContent("ELearning does not exists", this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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
