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
using University.Common.Models.Security;
using University.Constants;
using University.Context;
using University.Security.Models.ViewModel;

namespace University.Api.Controllers
{
    public class UsersSecurityController : UnSecuredController
    {
        public HttpResponseMessage Post(ApiViewModel apiViewModel)
        {
            //_logger.Info("UsersSecurity HttpPost - Called");
            CurrentUser currentUser = null;
            UniversityContext dbContext = null;
            int apiDataLogId = 0;
            Tenant tenant = null;
            string data = string.Empty;
            SecurityQuestion_vm SecurityQuestion = null;
            List<QuestionDetail> lstQuestions = null;
            try
            {
                if (apiViewModel.HasValue())
                {
                    data = JsonConvert.SerializeObject(apiViewModel.custom);
                    ApplicationUser_vm serializedUser = JsonConvert.DeserializeObject<ApplicationUser_vm>(apiViewModel.custom.ToString());
                    if (serializedUser != null)
                    {
                        dbContext = new UniversityContext();
                        var user = dbContext.ApplicationUsers.Include("Tenant")
                                   .SingleOrDefault(x => x.UserName == serializedUser.UserName
                                       && x.StatusCode != StatusCodeConstants.INACTIVE);
                        if (user != null)
                        {
                            tenant = user.Tenant;
                            Tuple<string, string, string> dob = user.DOB.FormHiddenDate();
                            string year = dob.Item1;
                            string month = dob.Item2;
                            string day = dob.Item3;
                            lstQuestions = (from ques in dbContext.SecurityQuestions
                                       .Where(x => x.StatusCode == StatusCodeConstants.ACTIVE)
                                            select new QuestionDetail
                                            {
                                                QuestionId = ques.SecurityQuestionId,
                                                Question = ques.Question,
                                                IsUserQuestion = (dbContext.ApplicationUserSecurityQuestions.Count(x => x.ApplicationUserId == user.ApplicationUserId
                                                && x.SecurityQuestionId == ques.SecurityQuestionId &&
                                                x.TenantId == tenant.TenantId) > 0 ? true : false)
                                            }).ToList();
                            SecurityQuestion = new SecurityQuestion_vm
                            {
                                ApplicationUserId = user.ApplicationUserId,
                                Year = year,
                                Month = month,
                                Day = day,
                                Questions = lstQuestions
                            };
                            return Serializer.ReturnContent(SecurityQuestion
                                , this.Configuration.Services.GetContentNegotiator()
                                , this.Configuration.Formatters, this.Request);
                        }
                        else
                        {
                            _logger.Warn(HttpConstants.UserNotExists);
                            return Serializer.ReturnContent(HttpConstants.UserNotExists, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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
