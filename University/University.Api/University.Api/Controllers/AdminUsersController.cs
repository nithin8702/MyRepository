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
using University.Bussiness.Models;
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
    public class AdminUsersController : UnSecuredController
    {
        public HttpResponseMessage Post(ApiViewModel apiViewModel)
        {
            //_logger.Info("AdminUsers HttpPost - Called");
            CurrentUser currentUser = null;
            UniversityContext dbContext = null;
            int apiDataLogId = 0;
            Tenant tenant = null;
            List<ApplicationUser_vm> lstApplicationUser = null;
            try
            {
                tenant = CurrentTenant;
                if (tenant != null)
                {
                    Token = apiViewModel.Token;
                    currentUser = ApiUser;
                    if (currentUser.HasValue())
                    {
                        //if (currentUser.AccountType != AccountType.Admin || currentUser.AccountType != AccountType.RootAdmin)
                        //{
                        //    return Serializer.ReturnContent(HttpConstants.NotAnAdminUser
                        //       , this.Configuration.Services.GetContentNegotiator()
                        //       , this.Configuration.Formatters, this.Request);
                        //}
                        dbContext = new UniversityContext();
                        IQueryable<ApplicationUser> query = dbContext.ApplicationUsers.Include("College").Include("Department")
                            .Where(x => (x.AccountType == AccountType.Admin || x.AccountType == AccountType.RootAdmin || x.AccountType == AccountType.BranchAdmin) && x.StatusCode == StatusCodeConstants.ACTIVE);
                        if (currentUser.AccountType != AccountType.RootAdmin && currentUser.AccountType != AccountType.BranchAdmin)
                        {
                            query = query.Where(x => x.TenantId == tenant.TenantId);
                        }
                        var data = query.ToList();
                        if (data.HasValue())
                        {
                            lstApplicationUser = new List<ApplicationUser_vm>();
                            foreach (var item in data)
                            {
                                lstApplicationUser.Add(new ApplicationUser_vm
                                {
                                    ApplicationUserId = item.ApplicationUserId,
                                    UserName = item.UserName,
                                    FirstName = item.FirstName,
                                    LastName = item.LastName,
                                    EmailAddress = item.EmailAddress,
                                    Contact = item.Contact,
                                    CollegeId = item.CollegeId.Value,
                                    CollegeName = item.College.CollegeName,
                                    DepartmentId = item.DepartmentId.Value,
                                    DepartmentName = item.Department.DepartmentName,
                                    DOB = item.DOB,
                                    Gender = item.Gender,
                                    GenderName = item.Gender.ToString(),
                                    ProfilePicturePath = item.ProfilePicturePath,
                                    RefId = item.RefId,
                                    WorkIdPicturePath = item.WorkIdPicturePath
                                });
                            }
                        }
                        return Serializer.ReturnContent(lstApplicationUser
                            , this.Configuration.Services.GetContentNegotiator()
                            , this.Configuration.Formatters, this.Request);
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
            //_logger.Info("AdminUsers HttpPut - Called");
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
                        apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Put, "AdminUsers", data);
                        ApplicationUser_vm serializedUser = JsonConvert.DeserializeObject<ApplicationUser_vm>(apiViewModel.custom.ToString());
                        if (serializedUser != null && !string.IsNullOrEmpty(currentUser.UserName))
                        {
                            dbContext = new UniversityContext();
                            var dbuser = dbContext.ApplicationUsers.SingleOrDefault(x => x.ApplicationUserId == currentUser.UserId && x.TenantId == tenant.TenantId);
                            if (dbuser != null)
                            {
                                dbuser.FirstName = serializedUser.FirstName;
                                dbuser.LastName = serializedUser.LastName;
                                dbuser.Gender = (Gender)serializedUser.Gender;
                                dbuser.EmailAddress = serializedUser.EmailAddress;
                                dbuser.AccountType = (AccountType)serializedUser.AccountType;
                                dbuser.RefId = serializedUser.RefId;
                                dbuser.ProfilePicturePath = serializedUser.ProfilePicturePath;
                                dbuser.WorkIdPicturePath = serializedUser.WorkIdPicturePath;
                                dbuser.DOB = serializedUser.DOB;
                                dbuser.Contact = serializedUser.Contact;
                                //dbuser.CollegeId = serializedUser.CollegeId;
                                //dbuser.DepartmentId = serializedUser.DepartmentId;
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
