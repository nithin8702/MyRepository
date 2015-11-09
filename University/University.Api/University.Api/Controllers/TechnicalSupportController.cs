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
    public class TechnicalSupportController : UnSecuredController
    {

        public HttpResponseMessage Get([FromUri]int ApplicationUserId = 0)
        {
            //_logger.Info("TechnicalSupport HttpGet - Called");
            int apiDataLogId = 0;
            UniversityContext dbcontext = new UniversityContext();
            List<TechnicalSupport_vm> lstTechnicalSupport = null;
            CurrentUser currentUser = null;
            Tenant tenant = null;
            try
            {
                tenant = CurrentTenant;
                if (tenant.HasValue())
                {
                    if (ApplicationUserId > 0)
                    {
                        lstTechnicalSupport = (from tS in dbcontext.TechnicalSupports.Include("ApplicationUser")

                                .Where(x => x.StatusCode == StatusCodeConstants.ACTIVE && x.ApplicationUserId == ApplicationUserId
                                    && x.TenantId == tenant.TenantId && x.ApplicationUser.StatusCode == StatusCodeConstants.ACTIVE
                                    )
                                               select new TechnicalSupport_vm
                                               {
                                                   TechnicalSupportId = tS.TechnicalSupportId,
                                                   ApplicationUserId = tS.ApplicationUserId,
                                                   Question = tS.Question,
                                                   Answer = tS.CustomField01,
                                                   Details = tS.Details,
                                                   Name = tS.ApplicationUser.FirstName + " " + tS.ApplicationUser.LastName,
                                                   EmailAddress = tS.ApplicationUser.EmailAddress,
                                                   PhoneNo = tS.ApplicationUser.Contact,
                                                   PostedDate = tS.CreatedOn.ToString()
                                               }).OrderByDescending(x => x.PostedDate).ToList();
                        if (lstTechnicalSupport.HasValue())
                        {
                            lstTechnicalSupport.ForEach(x => Separate(x));
                        }
                    }
                    else
                    {
                        lstTechnicalSupport = (from tS in dbcontext.TechnicalSupports.Include("ApplicationUser")

                            .Where(x => x.StatusCode == StatusCodeConstants.ACTIVE
                                && x.TenantId == tenant.TenantId && x.ApplicationUser.StatusCode == StatusCodeConstants.ACTIVE
                                )
                                               select new TechnicalSupport_vm
                                               {
                                                   TechnicalSupportId = tS.TechnicalSupportId,
                                                   ApplicationUserId = tS.ApplicationUserId,
                                                   Question = tS.Question,
                                                   Answer = tS.CustomField01,
                                                   Details = tS.Details,
                                                   Name = tS.ApplicationUser.FirstName + " " + tS.ApplicationUser.LastName,
                                                   EmailAddress = tS.ApplicationUser.EmailAddress,
                                                   PhoneNo = tS.ApplicationUser.Contact,
                                                   PostedDate = tS.CreatedOn.ToString()
                                               }).OrderByDescending(x => x.PostedDate).ToList();
                        if (lstTechnicalSupport.HasValue())
                        {
                            lstTechnicalSupport.ForEach(x => Separate(x));
                        }
                    }

                    //_logger.Info("lstTechnicalSupport count : " + lstTechnicalSupport.Count);
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
            return Serializer.ReturnContent(lstTechnicalSupport, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
        }

        private object Separate(TechnicalSupport_vm x)
        {
            if (x != null)
            {
                x.DaysAgo = TimeSpanFormat.FormatDaysAgo(x.PostedDate);
            }
            return x;

        }

        public HttpResponseMessage Delete(ApiViewModel apiViewModel)
        {
            //_logger.Info("TechnicalSupport Delete - Called");
            CurrentUser currentUser = null;
            UniversityContext dbContext = null;
            int apiDataLogId = 0;
            Tenant tenant = null;
            string data = string.Empty;
            TechnicalSupport tS = null;
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
                            apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Delete, "TechnicalSupport", data);
                            TechnicalSupport serializedTechnicalSupport = JsonConvert
                                .DeserializeObject<TechnicalSupport>(apiViewModel.custom.ToString());
                            if (serializedTechnicalSupport != null)
                            {
                                dbContext = new UniversityContext();
                                tS = dbContext.TechnicalSupports.SingleOrDefault(x => x.TechnicalSupportId == serializedTechnicalSupport.TechnicalSupportId
                                    && x.StatusCode == StatusCodeConstants.ACTIVE && x.TenantId == tenant.TenantId);
                                if (tS != null)
                                {
                                    dbContext.TechnicalSupports.Remove(tS);
                                    dbContext.SaveChanges();

                                    return Serializer.ReturnContent("Message deleted."
                                    , this.Configuration.Services.GetContentNegotiator()
                                    , this.Configuration.Formatters, this.Request);
                                }
                                else
                                {
                                    _logger.Warn("TechnicalSupport does not exists");
                                    return Serializer.ReturnContent("TechnicalSupport does not exists", this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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

        public HttpResponseMessage Post(ApiViewModel apiViewModel)
        {
            //_logger.Info("TechnicalSupport HttpPost - Called");
            CurrentUser currentUser = null;
            UniversityContext dbContext = null;
            int apiDataLogId = 0;
            Tenant tenant = null;
            string data = string.Empty;
            TechnicalSupport technicalSupport = null;
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
                            apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Post, "TechnicalSupport", data);
                            TechnicalSupport serializedTechnicalSupport = JsonConvert
                                .DeserializeObject<TechnicalSupport>(apiViewModel.custom.ToString());
                            if (serializedTechnicalSupport != null)
                            {
                                dbContext = new UniversityContext();
                                technicalSupport = new TechnicalSupport
                                {
                                    Question = serializedTechnicalSupport.Question,
                                    Details = serializedTechnicalSupport.Details,
                                    ApplicationUserId = currentUser.UserId,
                                    CreatedBy = currentUser.UserId,
                                    CreatedOn = DateTime.Now,
                                    TenantId = tenant.TenantId,
                                    StatusCode = StatusCodeConstants.ACTIVE,
                                    Language = Language.English
                                };
                                dbContext.TechnicalSupports.Add(technicalSupport);
                                dbContext.SaveChanges();
                                var adminIds = dbContext.ApplicationUsers.Where(x => x.TenantId == tenant.TenantId &&
                                x.StatusCode == StatusCodeConstants.ACTIVE
                                    && x.AccountType == AccountType.Admin).Select(x => x.ApplicationUserId).ToList();
                                if (adminIds.HasValue())
                                {
                                    foreach (var id in adminIds)
                                    {
                                        if (id > 0)
                                        {
                                            Notify.LogData(currentUser, dbContext, id, Module.Message,
                                               currentUser.FullName + " has posted a query for technical support.", technicalSupport.TechnicalSupportId, null, "TechnicalSupportMessage");
                                        }
                                    }
                                }
                                dbContext.SaveChanges();
                                return Serializer.ReturnContent("Message Posted."
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
            //_logger.Info("TechnicalSupport HttpPUt - Called");
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
                            apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Put, "TechnicalSupport", data);
                            TechnicalSupport_vm serializedTechnicalSupport = JsonConvert
                                .DeserializeObject<TechnicalSupport_vm>(apiViewModel.custom.ToString());
                            if (serializedTechnicalSupport != null)
                            {
                                dbContext = new UniversityContext();
                                TechnicalSupport tS = dbContext.TechnicalSupports.SingleOrDefault(x => x.TechnicalSupportId == serializedTechnicalSupport.TechnicalSupportId && x.StatusCode == StatusCodeConstants.ACTIVE
                                   && x.TenantId == tenant.TenantId);
                                if (tS != null)
                                {
                                    tS.CustomField01 = serializedTechnicalSupport.Answer;
                                    tS.LastModifiedBy = currentUser.UserId;
                                    tS.LastModifiedOn = DateTime.Now;
                                }
                                dbContext.SaveChanges();
                                Notify.LogData(currentUser, dbContext, tS.ApplicationUserId, Module.Message,
                                               currentUser.FullName + " has posted a query for technical support.", tS.TechnicalSupportId, null, "TechnicalSupportMessage");
                                dbContext.SaveChanges();
                                return Serializer.ReturnContent("Replied Successfully."
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
