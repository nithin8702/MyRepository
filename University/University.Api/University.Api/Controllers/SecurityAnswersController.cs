using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
using System.Linq;

namespace University.Api.Controllers
{
    public class SecurityAnswersController : UnSecuredController
    {
        public HttpResponseMessage Post(ApiViewModel apiViewModel)
        {
            //_logger.Info("SecurityAnswers HttpPost - Called");
            CurrentUser currentUser = null;
            UniversityContext dbContext = null;
            int apiDataLogId = 0;
            Tenant tenant = null;
            string data = string.Empty;
            try
            {
                if (apiViewModel.HasValue())
                {
                    Token = apiViewModel.Token;
                    data = JsonConvert.SerializeObject(apiViewModel.custom);
                    apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Post, "SecurityAnswers", data);
                    List<QuestionDetail> serializedSecurityAnswer = JsonConvert
                        .DeserializeObject<List<QuestionDetail>>(apiViewModel.custom.ToString());
                    if (serializedSecurityAnswer != null)
                    {
                        dbContext = new UniversityContext();
                        //_logger.Info("serializedSecurityAnswer" + serializedSecurityAnswer.Count);
                        if (serializedSecurityAnswer.Where(x => x.QuestionId > 0 && x.Answer != null && x.Answer != "").Count() > 0)
                        {
                            var dbuser = dbContext.DraftedUsers.Include("ApplicationUser")
                                .SingleOrDefault(x => x.Token == Token &&
                                x.StatusCode == StatusCodeConstants.DRAFT);
                            if (dbuser != null)
                            {
                                //_logger.Info("dbuser.ApplicationUserId" + dbuser.ApplicationUserId);
                                if (dbContext.ApplicationUserSecurityQuestions
                                    .Count(x => x.ApplicationUserId == dbuser.ApplicationUserId
                                        && x.StatusCode == StatusCodeConstants.ACTIVE) == 0)
                                {
                                    foreach (var item in serializedSecurityAnswer)
                                    {
                                        if (item != null)
                                        {
                                            ApplicationUserSecurityQuestion userSecurity = new ApplicationUserSecurityQuestion
                                            {
                                                ApplicationUserId = dbuser.ApplicationUserId,
                                                SecurityQuestionId = item.QuestionId,
                                                SecurityAnswer = item.Answer,
                                                CreatedBy = dbuser.ApplicationUserId,
                                                CreatedOn = DateTime.Now,
                                                TenantId = dbuser.TenantId,
                                                StatusCode = StatusCodeConstants.ACTIVE,
                                                Language = Language.English
                                            };
                                            dbContext.ApplicationUserSecurityQuestions.Add(userSecurity);
                                        }
                                        else
                                        {
                                            _logger.Warn(HttpConstants.InvalidInput);
                                            return Serializer.ReturnContent(HttpConstants.InvalidInput, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                                        }
                                    }
                                    AccountType aType = dbuser.ApplicationUser.AccountType;
                                    if (aType == AccountType.Admin)
                                    {
                                        dbuser.StatusCode = StatusCodeConstants.ACTIVE;
                                        dbuser.ApplicationUser.StatusCode = StatusCodeConstants.ACTIVE;
                                    }
                                    else if (aType == AccountType.Faculty || aType == AccountType.Student)
                                    {
                                        dbuser.StatusCode = StatusCodeConstants.NEW;
                                    }
                                    dbContext.SaveChanges();
                                    return Serializer.ReturnContent(HttpConstants.Inserted
                                        , this.Configuration.Services.GetContentNegotiator()
                                        , this.Configuration.Formatters, this.Request);
                                }
                                else
                                {
                                    _logger.Warn(HttpConstants.USERALREADYANSWEREDSECURITYQUESTIONS);
                                    return Serializer.ReturnContent(HttpConstants.USERALREADYANSWEREDSECURITYQUESTIONS, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                                }
                            }
                            else
                            {
                                _logger.Warn(HttpConstants.UserNotExists);
                                return Serializer.ReturnContent(HttpConstants.UserNotExists, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                            }
                        }
                        else
                        {
                            _logger.Warn(HttpConstants.TWOSECURITYQUESTIONSREQUIRED);
                            return Serializer.ReturnContent(HttpConstants.TWOSECURITYQUESTIONSREQUIRED, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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
