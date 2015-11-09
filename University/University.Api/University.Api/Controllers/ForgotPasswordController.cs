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
    public class ForgotPasswordController : UnSecuredController
    {
        public HttpResponseMessage Post(ApiViewModel apiViewModel)
        {
            //_logger.Info("ForgotPassword HttpPost - Called");
            CurrentUser currentUser = null;
            UniversityContext dbContext = null;
            int apiDataLogId = 0;
            Tenant tenant = null;
            string data = string.Empty;
            try
            {
                if (apiViewModel.HasValue())
                {
                    data = JsonConvert.SerializeObject(apiViewModel.custom);
                    apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Post, "ForgotPassword", data);
                    SecurityQuestion_vm serializedSecurityQuestion = JsonConvert
                        .DeserializeObject<SecurityQuestion_vm>(apiViewModel.custom.ToString());
                    if (serializedSecurityQuestion != null && serializedSecurityQuestion.Questions.HasValue())
                    {
                        dbContext = new UniversityContext();
                        var dbuser = dbContext.ApplicationUsers.Include("Tenant").SingleOrDefault(x =>
                            x.UserName == serializedSecurityQuestion.UserName
                            && x.StatusCode != StatusCodeConstants.INACTIVE);
                        if (dbuser != null)
                        {
                            tenant = dbuser.Tenant;
                            //_logger.Info("serializedSecurityQuestion.Questions " + serializedSecurityQuestion.Questions.Count);
                            foreach (var item in serializedSecurityQuestion.Questions)
                            {
                                //_logger.Info("item.QuestionId " + item.QuestionId);
                                //_logger.Info("item.Answer " + item.Answer);
                                //_logger.Info("item.ApplicationUserId " + dbuser.ApplicationUserId);
                                var question = dbContext.ApplicationUserSecurityQuestions
                                    .SingleOrDefault(x => x.SecurityQuestionId == item.QuestionId &&
                                        x.SecurityAnswer == item.Answer && x.ApplicationUserId == dbuser.ApplicationUserId &&
                                        x.TenantId == tenant.TenantId && x.StatusCode == StatusCodeConstants.ACTIVE);
                                if (question == null)
                                {
                                    _logger.Warn(HttpConstants.SECURITYANSWERMISMATCH);
                                    return Serializer.ReturnContent(HttpConstants.SECURITYANSWERMISMATCH, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                                }
                            }
                            if (dbuser.DOB.Year == Convert.ToInt32(serializedSecurityQuestion.Year) && dbuser.DOB.Month == Convert.ToInt32(serializedSecurityQuestion.Month) && dbuser.DOB.Day == Convert.ToInt32(serializedSecurityQuestion.Day))
                            {
                                int charaters = HttpConstants.PASSWORDLENGTH;
                                string newPassword = charaters.RandomString();
                                string strCurrentDate = DateTime.Now.ToString();
                                byte[] strSaltTemp = Encryptor.EncryptText(strCurrentDate, dbuser.UserName);
                                string se = Convert.ToBase64String(strSaltTemp);
                                byte[] strPasswordHash = Encryptor.GenerateHash(newPassword, se.ToString());
                                dbuser.PasswordHash = strPasswordHash;
                                dbuser.PasswordSalt = strSaltTemp;
                                dbuser.LastModifiedBy = dbuser.ApplicationUserId;
                                dbuser.LastModifiedOn = DateTime.Now;
                                dbContext.Entry<ApplicationUser>(dbuser).State = EntityState.Modified;

                                string sToken = Guid.NewGuid().ToString("D");
                                SecurityToken securityToken = new SecurityToken
                                {
                                    Token = sToken,
                                    TenantId = tenant.TenantId,
                                    CreatedBy = dbuser.ApplicationUserId,
                                    CreatedOn = DateTime.Now,
                                    IsExpired = false,
                                    Expiration = DateTime.Now.AddMinutes(tenant.SessionTime),
                                    StatusCode = StatusCodeConstants.ACTIVE,
                                    Language = Language.English,
                                    ApplicationUserId = dbuser.ApplicationUserId
                                };
                                dbContext.SecurityTokens.Add(securityToken);

                                dbContext.SaveChanges();

                                var tToken = dbContext.TenantTokens.SingleOrDefault(x => x.TenantId == tenant.TenantId && x.StatusCode == StatusCodeConstants.ACTIVE).Token;
                                return Serializer.ReturnContent(new ForgotPassword_vm { Password = newPassword, Token = sToken, TenantToken = tToken }
                                    , this.Configuration.Services.GetContentNegotiator()
                                    , this.Configuration.Formatters, this.Request);
                            }
                            else
                            {
                                _logger.Warn(HttpConstants.DOBDOESNOTMATCH);
                                return Serializer.ReturnContent(HttpConstants.DOBDOESNOTMATCH, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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
