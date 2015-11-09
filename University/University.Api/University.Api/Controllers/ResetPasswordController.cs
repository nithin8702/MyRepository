﻿using Newtonsoft.Json;
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
    public class ResetPasswordController : UnSecuredController
    {

        public HttpResponseMessage Post(ApiViewModel apiViewModel)
        {
            //_logger.Info("ResetPassword HttpPost - Called");
            CurrentUser currentUser = null;
            UniversityContext dbContext = null;
            int apiDataLogId = 0;
            Tenant tenant = null;
            string data = string.Empty;
            string newPassword = string.Empty;
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
                        apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Post, "ResetPassword", data);
                        ApplicationUser_vm serializedUser = JsonConvert.DeserializeObject<ApplicationUser_vm>(apiViewModel.custom.ToString());
                        if (currentUser.AccountType != AccountType.Admin)
                        {
                            _logger.Warn(HttpConstants.NotAnAdminUser);
                            return Serializer.ReturnContent(HttpConstants.NotAnAdminUser, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                        }
                        if (serializedUser != null && !string.IsNullOrEmpty(serializedUser.UserName))
                        {
                            dbContext = new UniversityContext();
                            var dbuser = dbContext.ApplicationUsers.SingleOrDefault(x => x.UserName == serializedUser.UserName &&
                                x.TenantId == tenant.TenantId && x.StatusCode != StatusCodeConstants.INACTIVE);
                            if (dbuser != null)
                            {
                                int charaters = HttpConstants.PASSWORDLENGTH;
                                newPassword = charaters.RandomString();
                                string strCurrentDate = DateTime.Now.ToString();
                                byte[] strSaltTemp = Encryptor.EncryptText(strCurrentDate, dbuser.UserName);
                                string se = Convert.ToBase64String(strSaltTemp);
                                byte[] strPasswordHash = Encryptor.GenerateHash(newPassword, se.ToString());
                                dbuser.PasswordHash = strPasswordHash;
                                dbuser.PasswordSalt = strSaltTemp;
                                dbuser.LastModifiedBy = dbuser.ApplicationUserId;
                                dbuser.LastModifiedOn = DateTime.Now;
                                dbContext.Entry<ApplicationUser>(dbuser).State = EntityState.Modified;

                                return Serializer.ReturnContent(newPassword
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
