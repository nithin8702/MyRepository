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
using University.Bussiness.Models;
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
    public class UniversityController : UnSecuredController
    {
        public HttpResponseMessage Post(ApiViewModel apiViewModel)
        {
            //_logger.Info("University HttpPost - Called");
            CurrentUser currentUser = null;
            UniversityContext dbContext = null;
            ApplicationUser applicationUser = null;
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
                        data = JsonConvert.SerializeObject(apiViewModel.custom);
                        //_logger.Info("data : " + data);
                        ApplicationUser_vm serializedUser = JsonConvert.DeserializeObject<ApplicationUser_vm>(apiViewModel.custom.ToString());
                        if (serializedUser != null && !string.IsNullOrEmpty(serializedUser.UserName))
                        {
                            if (serializedUser.AccountType == AccountType.Admin)
                            {
                                Token = apiViewModel.Token;
                                currentUser = ApiUser;
                                //_logger.Info("token : " + Token);
                                //_logger.Info("serializedUser.AccountType : " + serializedUser.AccountType);
                                if (currentUser.HasValue())
                                {
                                    //_logger.Info("currentUser.AccountType : " + currentUser.AccountType);
                                    if (currentUser.AccountType == AccountType.BranchAdmin)
                                    {
                                        //_logger.Warn("BranchAdmin cannot perform this operation.");
                                        return Serializer.ReturnContent("BranchAdmin cannot perform this operation.", this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                                    }
                                    if (currentUser.AccountType == AccountType.Admin || currentUser.AccountType == AccountType.RootAdmin)
                                    {
                                    }
                                    else
                                    {
                                        //_logger.Warn(HttpConstants.NotAnAdminUser);
                                        return Serializer.ReturnContent(HttpConstants.NotAnAdminUser, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                                    }
                                }
                                else
                                {
                                    _logger.Warn(HttpConstants.InvalidCurrentUser);
                                    return Serializer.ReturnContent(HttpConstants.InvalidCurrentUser, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                                }
                            }
                            if (serializedUser.AccountType == AccountType.BranchAdmin)
                            {
                                //_logger.Warn("AccountType Not Valid.");
                                return Serializer.ReturnContent("AccountType Not Valid.", this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                            }
                            dbContext = new UniversityContext();
                            if (dbContext.ApplicationUsers.Count(x => x.UserName == serializedUser.UserName) > 0)
                            {
                                //_logger.Warn(HttpConstants.UserAlreadyExists);
                                return Serializer.ReturnContent(HttpConstants.UserAlreadyExists, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                            }
                            string strCurrentDate = DateTime.Now.ToString();
                            byte[] passwordSalt = Encryptor.EncryptText(strCurrentDate, serializedUser.UserName);
                            string se = Convert.ToBase64String(passwordSalt);
                            byte[] passwordHash = Encryptor.GenerateHash(serializedUser.Password, se.ToString());
                            //string statusCode = (((AccountType)serializedUser.AccountType == AccountType.Admin) ? StatusCodeConstants.ACTIVE : StatusCodeConstants.DRAFT);
                            string statusCode = StatusCodeConstants.DRAFT;
                            if (currentUser.HasValue() && currentUser.AccountType == AccountType.RootAdmin)
                            {
                                statusCode = StatusCodeConstants.ACTIVE;
                            }
                            //_logger.Info("serializedUser.Contact : " + serializedUser.Contact);
                            applicationUser = new ApplicationUser
                            {
                                UserName = serializedUser.UserName,
                                FirstName = serializedUser.FirstName,
                                LastName = serializedUser.LastName,
                                Gender = (Gender)serializedUser.Gender,
                                EmailAddress = serializedUser.EmailAddress,
                                PasswordHash = passwordHash,
                                PasswordSalt = passwordSalt,
                                AccountType = (AccountType)serializedUser.AccountType,
                                RefId = serializedUser.RefId,
                                CollegeId = serializedUser.CollegeId,
                                DepartmentId = serializedUser.DepartmentId,
                                ProfilePicturePath = serializedUser.ProfilePicturePath,
                                WorkIdPicturePath = serializedUser.WorkIdPicturePath,
                                DOB = serializedUser.DOB,
                                StatusCode = statusCode,
                                Contact = serializedUser.Contact,
                                CreatedOn = DateTime.Now,
                                TenantId = tenant.TenantId,
                                Language = (Language)serializedUser.Language,
                                DeviceType = serializedUser.DeviceType,
                                DeviceToken = serializedUser.DeviceToken,
                                DeviceUDDI = serializedUser.DeviceUDDI,
                                PushCertificatePath = "~/Resources/PUSH NEWS.p12",
                                DeviceSoundPath = serializedUser.DeviceSoundPath,
                                ChannelUri = serializedUser.ChannelUri,
                                PushCertificatePassword = serializedUser.PushCertificatePassword,
                                OSVersion = serializedUser.OSVersion,
                                BatchingInterval = serializedUser.BatchingInterval,
                                NavigatePath = serializedUser.NavigatePath,
                                RegistrationId = serializedUser.RegistrationId
                            };
                            if (serializedUser.DummyColleges.HasValue())
                            {
                                foreach (var item in serializedUser.DummyColleges)
                                {
                                    item.StatusCode = StatusCodeConstants.ACTIVE;
                                    item.CreatedOn = DateTime.Now;
                                    item.CreatedBy = applicationUser.ApplicationUserId;
                                    item.TenantId = tenant.TenantId;
                                    item.Language = (Language)serializedUser.Language;
                                    if (applicationUser != null)
                                    {
                                        applicationUser.DummyColleges.Add(item);
                                    }
                                }

                            }
                            dbContext.ApplicationUsers.Add(applicationUser);

                            string token = Guid.NewGuid().ToString("D") + Guid.NewGuid().ToString("D");
                            string dStatus = StatusCodeConstants.DRAFT;
                            if (currentUser.HasValue() && currentUser.AccountType == AccountType.RootAdmin)
                            {

                                dStatus = StatusCodeConstants.NEW;
                                List<ApplicationUserSecurityQuestion> lstUserSecurityAnswer = new List<ApplicationUserSecurityQuestion>();
                                var ques1 = dbContext.SecurityQuestions.SingleOrDefault(x =>
                                    x.Question == "What was your childhood nickname?"
                                    && x.StatusCode == StatusCodeConstants.ACTIVE);
                                if (ques1 != null)
                                {
                                    lstUserSecurityAnswer.Add(new ApplicationUserSecurityQuestion
                                    {
                                        ApplicationUserId = applicationUser.ApplicationUserId,
                                        SecurityQuestionId = ques1.SecurityQuestionId,
                                        SecurityAnswer = "nickname",
                                        CreatedBy = applicationUser.ApplicationUserId,
                                        CreatedOn = DateTime.Now,
                                        TenantId = tenant.TenantId,
                                        StatusCode = StatusCodeConstants.ACTIVE,
                                        Language = Language.English
                                    });
                                }
                                var ques2 = dbContext.SecurityQuestions.SingleOrDefault(x =>
                                    x.Question == "What was the name of your first stuffed animal?"
                                    && x.StatusCode == StatusCodeConstants.ACTIVE);
                                if (ques2 != null)
                                {
                                    lstUserSecurityAnswer.Add(new ApplicationUserSecurityQuestion
                                    {
                                        ApplicationUserId = applicationUser.ApplicationUserId,
                                        SecurityQuestionId = ques2.SecurityQuestionId,
                                        SecurityAnswer = "animal",
                                        CreatedBy = applicationUser.ApplicationUserId,
                                        CreatedOn = DateTime.Now,
                                        TenantId = tenant.TenantId,
                                        StatusCode = StatusCodeConstants.ACTIVE,
                                        Language = Language.English
                                    });
                                }
                                dbContext.ApplicationUserSecurityQuestions.AddRange(lstUserSecurityAnswer);

                            }

                            DraftedUser draftedUser = new DraftedUser
                            {
                                ApplicationUserId = applicationUser.ApplicationUserId,
                                Token = token,
                                TenantId = tenant.TenantId,
                                CreatedBy = applicationUser.ApplicationUserId,
                                CreatedOn = DateTime.Now,
                                Language = Language.English,
                                ExpiryDate = DateTime.Now.AddMinutes(tenant.SessionTime),
                                StatusCode = dStatus
                            };
                            dbContext.DraftedUsers.Add(draftedUser);

                            Setting stg = new Setting
                            {
                                ApplicationUserId = applicationUser.ApplicationUserId,
                                IsNotify = true,
                                IsBookCorner = false,
                                IsBroadcast = true,
                                IsMessage = true,
                                IsTrafficNews = false,
                                IsChat = true,
                                TenantId = tenant.TenantId,
                                CreatedBy = applicationUser.ApplicationUserId,
                                CreatedOn = DateTime.Now,
                                Language = Language.English,
                                StatusCode = StatusCodeConstants.ACTIVE
                            };
                            dbContext.Settings.Add(stg);

                            dbContext.SaveChanges();

                            if (serializedUser.AccountType == AccountType.Faculty)
                            {
                                var adminIds = dbContext.ApplicationUsers.Where(x => x.TenantId == tenant.TenantId &&
                                    x.StatusCode == StatusCodeConstants.ACTIVE
                                        && x.AccountType == AccountType.Admin).Select(x => x.ApplicationUserId).ToList();
                                if (adminIds.HasValue())
                                {
                                    currentUser = new CurrentUser
                                    {
                                        TenantId = tenant.TenantId,
                                        UserId = applicationUser.ApplicationUserId,
                                    };
                                    foreach (var id in adminIds)
                                    {
                                        if (id > 0)
                                        {
                                            Notify.LogData(currentUser, dbContext, id, Module.Message,
                                               serializedUser.FirstName + " " + serializedUser.LastName + " has registered for faculty account", applicationUser.ApplicationUserId);
                                        }
                                    }
                                    dbContext.SaveChanges();
                                }
                            }

                            return Serializer.ReturnContent(token, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                        }
                        else
                        {
                            _logger.Warn(HttpConstants.InvalidInput);
                            return Serializer.ReturnContent(HttpConstants.InvalidInput, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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
                _logger.Error("ex.Message " + ex.Message);
                string message = (ex.InnerException != null) ? ((ex.InnerException.InnerException != null) ? ex.InnerException.InnerException.Message : ex.InnerException.ToString()) : ex.Message;
                _logger.Error("message " + message);
                string stackTrace = ex.StackTrace;
                _logger.Error("stackTrace : " + stackTrace);
                if (currentUser == null)
                {
                    currentUser = new CurrentUser { TenantId = (tenant != null) ? tenant.TenantId : 0 };
                }
                ErrorLog.LogCustomError(currentUser, ex, apiDataLogId);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        public HttpResponseMessage Put(ApiViewModel apiViewModel)
        {
            //_logger.Info("University HttpPut - Called");
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
                    Token = apiViewModel.Token;
                    currentUser = ApiUser;
                    if (tenant != null)
                    {
                        data = JsonConvert.SerializeObject(apiViewModel.custom);
                        apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Put, "University", data);
                        ApplicationUser_vm serializedUser = JsonConvert.DeserializeObject<ApplicationUser_vm>(apiViewModel.custom.ToString());
                        if (serializedUser != null && !string.IsNullOrEmpty(currentUser.UserName))
                        {
                            dbContext = new UniversityContext();
                            var dbuser = dbContext.ApplicationUsers.SingleOrDefault(x => x.UserName == currentUser.UserName &&
                                x.TenantId == tenant.TenantId && x.StatusCode != StatusCodeConstants.INACTIVE);
                            if (dbuser != null)
                            {
                                byte[] strSalt = dbuser.PasswordSalt;
                                string salt = Convert.ToBase64String(strSalt);
                                byte[] dbPasswordHash = dbuser.PasswordHash;
                                byte[] userPasswordHash = Encryptor.GenerateHash(serializedUser.OldPassword, salt);
                                bool chkPassword = Encryptor.CompareByteArray(dbPasswordHash, userPasswordHash);
                                if (chkPassword)
                                {
                                    string strCurrentDate = DateTime.Now.ToString();
                                    byte[] strSaltTemp = Encryptor.EncryptText(strCurrentDate, currentUser.UserName);
                                    string se = Convert.ToBase64String(strSaltTemp);
                                    byte[] strPasswordHash = Encryptor.GenerateHash(serializedUser.NewPassword, se.ToString());
                                    dbuser.PasswordHash = strPasswordHash;
                                    dbuser.PasswordSalt = strSaltTemp;
                                    dbuser.LastModifiedBy = currentUser.UserId;
                                    dbuser.LastModifiedOn = DateTime.Now;
                                    dbContext.Entry<ApplicationUser>(dbuser).State = EntityState.Modified;
                                    dbContext.SaveChanges();
                                    return Serializer.ReturnContent(HttpConstants.Updated
                                    , this.Configuration.Services.GetContentNegotiator()
                                    , this.Configuration.Formatters, this.Request);
                                }
                                else
                                {
                                    _logger.Warn(HttpConstants.PasswordMismatch);
                                    return Serializer.ReturnContent(HttpConstants.PasswordMismatch, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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
            //_logger.Info("Users Delete - Called");
            CurrentUser currentUser = null;
            UniversityContext dbContext = null;
            int apiDataLogId = 0;
            Tenant tenant = null;
            string data = string.Empty;
            ApplicationUser user = null;
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
                            apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Delete, "University", data);
                            if (currentUser.AccountType == AccountType.Admin)
                            {
                                ApplicationUser_vm serializedApplicationUser = JsonConvert
                                   .DeserializeObject<ApplicationUser_vm>(apiViewModel.custom.ToString());
                                if (serializedApplicationUser != null)
                                {
                                    dbContext = new UniversityContext();
                                    user = dbContext.ApplicationUsers.SingleOrDefault(x => x.ApplicationUserId == serializedApplicationUser.ApplicationUserId
                                        && x.TenantId == tenant.TenantId && x.StatusCode == StatusCodeConstants.ACTIVE);
                                    if (user != null)
                                    {
                                        var sUser = dbContext.SecurityTokens.SingleOrDefault(x => x.ApplicationUserId == user.ApplicationUserId);
                                        if (sUser != null)
                                        {
                                            sUser.StatusCode = StatusCodeConstants.INACTIVE;
                                            sUser.LastModifiedBy = currentUser.UserId;
                                            sUser.LastModifiedOn = DateTime.Now;
                                        }


                                        user.StatusCode = StatusCodeConstants.INACTIVE;
                                        user.LastModifiedBy = currentUser.UserId;
                                        user.LastModifiedOn = DateTime.Now;

                                        var sToken = dbContext.SecurityTokens.SingleOrDefault(x => x.ApplicationUserId == user.ApplicationUserId);
                                        if (sToken != null)
                                        {

                                        }

                                        dbContext.SaveChanges();

                                        return Serializer.ReturnContent(HttpConstants.Deleted
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
                                    return Serializer.ReturnContent(HttpConstants.InvalidCurrentUser, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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
    }
}
