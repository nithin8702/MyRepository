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
using University.Bussiness.Models;
using University.Common.Models;
using University.Common.Models.Enums;
using University.Common.Models.Security;
using University.Common.Models.ViewModel;
using University.Constants;
using University.Context;
using University.Security.Models;
using University.Security.Models.ViewModel;
using University.Utilities;

namespace University.Api.Controllers
{
    public class FeedbackController : UnSecuredController
    {

        public HttpResponseMessage Get()
        {
            //_logger.Info("Feedback HttpGet All - Called");
            UniversityContext dbcontext = null;
            List<Feedback_vm> lstFeedback = null;
            Tenant tenant = null;
            int apiDataLogId = 0;
            CurrentUser currentUser = null;
            try
            {
                tenant = CurrentTenant;
                if (tenant.HasValue())
                {
                    dbcontext = new UniversityContext();
                    lstFeedback = (from fB in dbContext.Feedbacks.Include("ApplicationUser")
                                   .Where(x =>
                                       x.StatusCode == StatusCodeConstants.ACTIVE
                                       && x.TenantId == tenant.TenantId)
                                   select new Feedback_vm
                                   {
                                       ApplicationRating = fB.ApplicationRating,
                                       CustomKey1 = fB.CustomKey1,
                                       CustomKeyValue1 = fB.CustomKeyValue1,
                                       CustomKey2 = fB.CustomKey2,
                                       CustomKeyValue2 = fB.CustomKeyValue2,
                                       FeedbackStatement = fB.FeedbackStatement,
                                       ProblemStatement = fB.ProblemStatement,
                                       SuggestionStatement = fB.SuggestionStatement,
                                       PostedDate = fB.CreatedOn.ToString(),
                                       UserName = fB.ApplicationUser.FirstName + " " + fB.ApplicationUser.LastName,
                                       ApplicationUserId = fB.ApplicationUserId.Value,
                                       FirstName = fB.ApplicationUser.FirstName,
                                       LastName = fB.ApplicationUser.LastName,
                                       EmailAddress = fB.ApplicationUser.EmailAddress
                                   }).ToList();
                    return Serializer.ReturnContent(lstFeedback, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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
            //_logger.Info("Feedback HttpPost - Called");
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
                            apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Post, "Feedback", data);
                            Feedback_vm serializedFeedback = JsonConvert
                                .DeserializeObject<Feedback_vm>(apiViewModel.custom.ToString());
                            if (serializedFeedback != null)
                            {
                                dbContext = new UniversityContext();
                                Feedback fB = new Feedback
                                {
                                    ApplicationUserId = currentUser.UserId,
                                    ApplicationRating = serializedFeedback.ApplicationRating,
                                    CustomKey1 = serializedFeedback.CustomKey1,
                                    CustomKeyValue1 = serializedFeedback.CustomKeyValue1,
                                    CustomKey2 = serializedFeedback.CustomKey2,
                                    CustomKeyValue2 = serializedFeedback.CustomKeyValue2,
                                    FeedbackStatement = serializedFeedback.FeedbackStatement,
                                    ProblemStatement = serializedFeedback.ProblemStatement,
                                    SuggestionStatement = serializedFeedback.SuggestionStatement,
                                    CreatedBy = currentUser.UserId,
                                    CreatedOn = DateTime.Now,
                                    TenantId = currentUser.TenantId,
                                    StatusCode = StatusCodeConstants.ACTIVE,
                                    Language = Language.English
                                };
                                dbContext.Feedbacks.Add(fB);
                                dbContext.SaveChanges();
                                var adminIds = dbContext.ApplicationUsers.Where(x => x.TenantId == tenant.TenantId && x.StatusCode == StatusCodeConstants.ACTIVE
                                    && x.AccountType == AccountType.Admin).Select(x => x.ApplicationUserId).ToList();
                                if (adminIds.HasValue())
                                {
                                    foreach (var id in adminIds)
                                    {
                                        if (id > 0)
                                        {
                                            Notify.LogData(currentUser, dbContext, id, Module.Message,
                                                        currentUser.FullName + " has posted feedback", fB.FeedbackId, null, "Feedback");
                                        }
                                    }
                                }
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

        public HttpResponseMessage Delete(ApiViewModel apiViewModel)
        {
            //_logger.Info("Feedback Delete - Called");
            CurrentUser currentUser = null;
            UniversityContext dbContext = null;
            int apiDataLogId = 0;
            Tenant tenant = null;
            string data = string.Empty;
            Feedback cD = null;
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
                            apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Delete, "Feedback", data);
                            Feedback_vm serializedFeedback = JsonConvert
                                .DeserializeObject<Feedback_vm>(apiViewModel.custom.ToString());
                            if (serializedFeedback != null)
                            {
                                dbContext = new UniversityContext();
                                cD = dbContext.Feedbacks.SingleOrDefault(x => x.FeedbackId == serializedFeedback.FeedbackId
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
                                    _logger.Warn("Feedback does not exists");
                                    return Serializer.ReturnContent("Feedback does not exists", this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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
