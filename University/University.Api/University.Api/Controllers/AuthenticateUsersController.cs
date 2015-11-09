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
    public class AuthenticateUsersController : UnSecuredController
    {
        public HttpResponseMessage Post(ApiViewModel apiViewModel)
        {
            //_logger.Info("AuthenticateUsers HttpPost - Called");
            CurrentUser currentUser = null;
            UniversityContext dbContext = null;
            int apiDataLogId = 0;
            Tenant tenant = null;
            string data = string.Empty;
            List<AuthenticateUser_vm> lstAuthenticateUser = null;
            try
            {
                tenant = CurrentTenant;
                if (tenant.HasValue())
                {
                    Token = apiViewModel.Token;
                    currentUser = ApiUser;
                    if (currentUser.HasValue())
                    {
                        //data = JsonConvert.SerializeObject(apiViewModel.custom);
                        //apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Post, "AuthenticateUsers", data);
                        if (currentUser.AccountType == AccountType.Admin)
                        {
                            dbContext = new UniversityContext();
                            //_logger.Info("currentUser.AccountType : " + currentUser.AccountType);
                            //_logger.Info("tenant.TenantId : " + tenant.TenantId);
                            lstAuthenticateUser = (from user in dbContext.DraftedUsers.Include("ApplicationUser").Where(x =>
                                x.ApplicationUser.AccountType == AccountType.Faculty && x.StatusCode == StatusCodeConstants.NEW &&
                                x.TenantId == tenant.TenantId)
                                                   select new AuthenticateUser_vm
                                                   {
                                                       //StatusCode=user.StatusCode,
                                                       UserId = user.ApplicationUserId,
                                                       AuthenticateToken = user.Token,
                                                       UserName = user.ApplicationUser.UserName,
                                                       FirstName = user.ApplicationUser.FirstName,
                                                       LastName = user.ApplicationUser.LastName,
                                                       Contact = user.ApplicationUser.Contact,
                                                       EmailAddress = user.ApplicationUser.EmailAddress,
                                                       CollegeId = user.ApplicationUser.CollegeId.Value,
                                                       CollegeName = user.ApplicationUser.College.CollegeName,
                                                       DepartmentId = user.ApplicationUser.DepartmentId.Value,
                                                       DepartmentName = user.ApplicationUser.Department.DepartmentName,
                                                       WorkIdPicturePath = user.ApplicationUser.WorkIdPicturePath,
                                                       ProfilePicturePath = user.ApplicationUser.ProfilePicturePath,
                                                       PostedDate = user.ApplicationUser.CreatedOn.ToString(),
                                                       //DaysAgo = DbFunctions.DiffDays(user.ApplicationUser.CreatedOn, DateTime.Today).Value
                                                   }).OrderByDescending(x => x.PostedDate).ToList();
                            if (lstAuthenticateUser.HasValue())
                            {
                                lstAuthenticateUser.ForEach(x => Separate(x));
                            }
                            return Serializer.ReturnContent(lstAuthenticateUser, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                ErrorLog.LogCustomError(currentUser, ex, apiDataLogId);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        public HttpResponseMessage Put(ApiViewModel apiViewModel)
        {
            //_logger.Info("AuthenticateUsers HttpPut - Called");
            CurrentUser currentUser = null;
            UniversityContext dbContext = null;
            int apiDataLogId = 0;
            Tenant tenant = null;
            string data = string.Empty;
            DraftedUser aUser = null;
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
                            apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Put, "AuthenticateUsers", data);
                            if (currentUser.AccountType != AccountType.Admin)
                            {
                                _logger.Warn(HttpConstants.NotAnAdminUser);
                                return Serializer.ReturnContent(HttpConstants.NotAnAdminUser, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                            }
                            AuthenticateUser_vm serializedDrafterUser = JsonConvert
                                .DeserializeObject<AuthenticateUser_vm>(apiViewModel.custom.ToString());
                            if (serializedDrafterUser != null)
                            {
                                dbContext = new UniversityContext();
                                aUser = dbContext.DraftedUsers.SingleOrDefault(x => x.ApplicationUserId == serializedDrafterUser.UserId
                                    && x.StatusCode == StatusCodeConstants.NEW && x.TenantId == tenant.TenantId);
                                if (aUser != null)
                                {
                                    aUser.StatusCode = StatusCodeConstants.INACTIVE;
                                    aUser.LastModifiedBy = currentUser.UserId;
                                    aUser.LastModifiedOn = DateTime.Now;
                                    //dbContext.Entry<College>(aUser).State = EntityState.Modified;
                                    dbContext.SaveChanges();

                                    return Serializer.ReturnContent(HttpConstants.Updated
                                    , this.Configuration.Services.GetContentNegotiator()
                                    , this.Configuration.Formatters, this.Request);
                                }
                                else
                                {
                                    _logger.Warn("User does not exists");
                                    return Serializer.ReturnContent("User does not exists", this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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

        private object Separate(AuthenticateUser_vm x)
        {
            if (x != null)
            {
                x.DaysAgo = TimeSpanFormat.FormatDaysAgo(x.PostedDate);
            }
            return x;
        }


    }
}
