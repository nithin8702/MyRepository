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
    public class SecurityQuestionsController : UnSecuredController
    {
        public HttpResponseMessage Post(ApiViewModel apiViewModel)
        {
            //_logger.Info("SecurityQuestions HttpPost - Called");
            UniversityContext dbContext = null;
            string data = string.Empty;
            List<QuestionDetail> lstQuestions = null;
            try
            {
                if (!string.IsNullOrEmpty(apiViewModel.Token))
                {
                    dbContext = new UniversityContext();
                    var dUser = dbContext.DraftedUsers.SingleOrDefault(x => x.Token == apiViewModel.Token);
                    if (dUser != null)
                    {
                        //_logger.Info("dbuser found : " + dUser.TenantId);
                        lstQuestions = (from ques in dbContext.SecurityQuestions
                           .Where(x => x.StatusCode == StatusCodeConstants.ACTIVE)
                                        select new QuestionDetail
                                        {
                                            QuestionId = ques.SecurityQuestionId,
                                            Question = ques.Question
                                        }).ToList();
                    }
                    else
                    {
                        //_logger.Warn(HttpConstants.UserNotExists);
                        return Serializer.ReturnContent(HttpConstants.UserNotExists, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                    }
                }
                else
                {
                    _logger.Warn("Token not found");
                    return Serializer.ReturnContent("Token not found", this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                }
                return Serializer.ReturnContent(lstQuestions
                    , this.Configuration.Services.GetContentNegotiator()
                    , this.Configuration.Formatters, this.Request);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }
    }
}
