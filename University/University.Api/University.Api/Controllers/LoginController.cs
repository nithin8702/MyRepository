using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using University.Api.Controllers.Log;
using University.Api.Controllers.Serialize;
using University.Common.Models;
using University.Common.Models.Enums;
using University.Common.Models.Security;
using University.Constants;
using University.Context;
using University.Security.Models;
using University.Security.Models.ViewModel;
using University.Utilities;
using University.Api.Extensions;
using System.Data.Entity.Validation;

namespace University.Api.Controllers
{
    public class LoginController : UnSecuredController
    {
        public HttpResponseMessage Post(ApiViewModel apiViewModel)
        {
            //_logger.Info("Login HttpPost - Called");
            string data = string.Empty;
            CurrentUser currentUser = null;
            UniversityContext dbContext = null;
            ApplicationUser_vm applicationUser = null;
            int apiDataLogId = 0;
            Tenant tenant = null;
            try
            {
                if (apiViewModel != null && apiViewModel.custom != null)
                {
                    data = JsonConvert.SerializeObject(apiViewModel.custom);
                    ApplicationUser_vm serializedUser = JsonConvert.DeserializeObject<ApplicationUser_vm>(apiViewModel.custom.ToString());
                    if (serializedUser != null)
                    {
                        //_logger.Info("serializedUser has value" + serializedUser.UserName);
                        dbContext = new UniversityContext();
                        var dbuser = dbContext.ApplicationUsers.Include("Tenant")
                            //.Include("Department").Include("College")
                            .SingleOrDefault(x => x.UserName == serializedUser.UserName &&
                            x.StatusCode == StatusCodeConstants.ACTIVE);
                        if (dbuser != null)
                        {
                            //_logger.Info("dbuser has value");
                            currentUser = new CurrentUser
                            {
                                UserId = dbuser.ApplicationUserId,
                                UserName = dbuser.UserName,
                                TenantId = dbuser.TenantId
                            };
                            tenant = dbuser.Tenant;
                            apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Post, "Login", data);
                            if (tenant.StatusCode != StatusCodeConstants.ACTIVE)
                            {
                                _logger.Warn(HttpConstants.InvalidTenant);
                                return Serializer.ReturnContent(HttpConstants.InvalidTenant, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                            }
                            byte[] strSalt = dbuser.PasswordSalt;
                            string salt = Convert.ToBase64String(strSalt);
                            byte[] dbPasswordHash = dbuser.PasswordHash;
                            byte[] userPasswordHash = Encryptor.GenerateHash(serializedUser.Password, salt);
                            bool chkPassword = Encryptor.CompareByteArray(dbPasswordHash, userPasswordHash);
                            if (chkPassword)
                            {
                                //_logger.Info("currentUser.UserId : " + currentUser.UserId);
                                SecurityToken securityToken = dbContext.SecurityTokens
                                    .FirstOrDefault(x => x.ApplicationUserId == currentUser.UserId);
                                if (securityToken != null)
                                {
                                    //_logger.Info("token already exists");
                                    securityToken.Expiration = DateTime.Now.AddMinutes(tenant.SessionTime);
                                    //securityToken.Token = Guid.NewGuid().ToString("D") + Guid.NewGuid().ToString("D");
                                }
                                else
                                {
                                    //_logger.Info("new token");
                                    securityToken = new SecurityToken
                                    {
                                        Token = Guid.NewGuid().ToString("D") + Guid.NewGuid().ToString("D"),
                                        TenantId = tenant.TenantId,
                                        CreatedBy = currentUser.UserId,
                                        CreatedOn = DateTime.Now,
                                        IsExpired = false,
                                        Expiration = DateTime.Now.AddMinutes(tenant.SessionTime),
                                        StatusCode = StatusCodeConstants.ACTIVE,
                                        Language = Language.English,
                                        ApplicationUserId = currentUser.UserId,
                                    };
                                    dbContext.SecurityTokens.Add(securityToken);
                                }

                                dbContext.SaveChanges();
                                var tenantToken = dbContext.TenantTokens.SingleOrDefault(x => x.TenantId == dbuser.TenantId && x.StatusCode == StatusCodeConstants.ACTIVE);
                                if (tenantToken != null)
                                {
                                    applicationUser = new ApplicationUser_vm
                                    {
                                        TenantId=dbuser.TenantId,
                                        Tenant=dbuser.Tenant,
                                        ApplicationUserId = dbuser.ApplicationUserId,
                                        UserName = dbuser.UserName,
                                        FirstName = dbuser.FirstName,
                                        LastName = dbuser.LastName,
                                        FullName = dbuser.FirstName + " " + dbuser.LastName,
                                        EmailAddress = dbuser.EmailAddress,
                                        Gender = dbuser.Gender,
                                        RefId = dbuser.RefId,
                                        Contact = dbuser.Contact,
                                        DOB = dbuser.DOB,
                                        AccountType = dbuser.AccountType,
                                        AccountTypeName = dbuser.AccountType.ToString(),
                                        RowVersion = dbuser.RowVersion,
                                        StatusCode = dbuser.StatusCode,
                                        Token = securityToken.Token,
                                        TenantToken = tenantToken.Token,
                                        CollegeId = dbuser.CollegeId.Value,
                                        DepartmentId = dbuser.DepartmentId.Value,
                                        WorkIdPicturePath = dbuser.WorkIdPicturePath,
                                        ProfilePicturePath = dbuser.ProfilePicturePath,
                                        RegistrationId=dbuser.RegistrationId,
                                        DeviceType=dbuser.DeviceType,
                                        DeviceUDDI=dbuser.DeviceUDDI,
                                        DeviceToken=dbuser.DeviceToken,
                                        CreatedBy = dbuser.CreatedBy,
                                        CreatedOn = dbuser.CreatedOn,
                                        LastModifiedBy = dbuser.LastModifiedBy,
                                        LastModifiedOn = dbuser.LastModifiedOn
                                    };
                                    //_logger.Info("applicationUser.UserName" + applicationUser.UserName);
                                }
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
                    _logger.Warn(HttpConstants.InvalidApiViewModel);
                    return Serializer.ReturnContent(HttpConstants.InvalidApiViewModel, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                }
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                ErrorLog.LogCustomError(currentUser, ex, apiDataLogId);
            }
            return Serializer.ReturnContent(applicationUser, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
        }
    }
}
