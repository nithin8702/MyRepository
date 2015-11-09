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
    public class UserSearchController : UnSecuredController
    {
        public HttpResponseMessage Post(ApiViewModel apiViewModel)
        {
            //_logger.Info("UserSearch HttpPost - Called");
            string data = string.Empty;
            CurrentUser currentUser = null;
            UniversityContext dbContext = null;
            int apiDataLogId = 0;
            Tenant tenant = null;
            IQueryable<ApplicationUser> queryableUser = null;
            List<ApplicationUser_vm> lstApplicationUser = null;
            bool hasSearch = false;
            try
            {
                if (apiViewModel.HasValue())
                {
                    tenant = CurrentTenant;
                    if (tenant.HasValue())
                    {
                        Token = apiViewModel.Token;
                        currentUser = ApiUser;
                        data = JsonConvert.SerializeObject(apiViewModel.custom);
                        UserSearch_vm serializedUserSearch = JsonConvert.DeserializeObject<UserSearch_vm>(apiViewModel.custom.ToString());
                        if (serializedUserSearch != null)
                        {
                            dbContext = new UniversityContext();
                            queryableUser = dbContext.ApplicationUsers.Where(x => x.TenantId == tenant.TenantId && x.StatusCode == StatusCodeConstants.ACTIVE);
                            if (currentUser.AccountType == AccountType.Student)
                            {
                                var facs = dbContext.StudentSubscriptions.Include("ClassDetail").Where(x => x.TenantId == tenant.TenantId
                                        && x.StatusCode == StatusCodeConstants.ACTIVE
                                        && x.ApplicationUserId == currentUser.UserId
                                        )
                                        .Select(x => x.ClassDetail.ApplicationUserId).ToList();
                                if (facs.HasValue())
                                {
                                    queryableUser = queryableUser.Where(x => facs.Contains(x.ApplicationUserId));
                                }
                            }
                            if (serializedUserSearch.ClassIds.HasValue())
                            {
                                hasSearch = true;
                                var clsUsers = dbContext.ClassDetails.Where(x => x.TenantId == tenant.TenantId && x.StatusCode == StatusCodeConstants.ACTIVE
                                        && serializedUserSearch.ClassIds.Contains(x.ClassDetailId)).Select(x => x.ApplicationUserId).ToList();
                                if (clsUsers.HasValue())
                                {
                                    queryableUser = queryableUser.Where(x => clsUsers.Contains(x.ApplicationUserId));
                                }
                                if (currentUser.AccountType == AccountType.Faculty)
                                {
                                    var students = dbContext.StudentSubscriptions.Where(x => x.TenantId == tenant.TenantId
                                        && x.StatusCode == StatusCodeConstants.ACTIVE
                                        && serializedUserSearch.ClassIds.Contains(x.ClassDetailId.Value))
                                        .Select(x => x.ApplicationUserId).ToList();
                                    if (students.HasValue())
                                    {
                                        queryableUser = queryableUser.Where(x => students.Contains(x.ApplicationUserId));
                                    }
                                }
                            }
                            if (serializedUserSearch.IsExcludeStudent)
                            {
                                queryableUser = queryableUser.Where(x => x.AccountType != AccountType.Student);
                            }
                            if (serializedUserSearch.AccountType == AccountType.Faculty)
                            {
                                hasSearch = true;
                                queryableUser = queryableUser.Where(x => x.AccountType == AccountType.Faculty);
                            }
                            else if (serializedUserSearch.AccountType == AccountType.Student)
                            {
                                hasSearch = true;
                                queryableUser = queryableUser.Where(x => x.AccountType == AccountType.Student);
                            }
                            else if (serializedUserSearch.AccountType == AccountType.Admin)
                            {
                                hasSearch = true;
                                queryableUser = queryableUser.Where(x => x.AccountType == AccountType.Admin);
                            }
                            if (serializedUserSearch.Module == Module.Message)
                            {
                                hasSearch = true;
                                var lstresticted = dbContext.RestrictedUsers.Where(x => x.ApplicationUserId == currentUser.UserId
                                    && x.StatusCode == StatusCodeConstants.ACTIVE && x.TenantId == tenant.TenantId)
                                    .Select(x => x.CreatedBy).ToList();
                                queryableUser = queryableUser.Where(x => !lstresticted.Contains(x.ApplicationUserId));
                            }
                            //if (serializedUserSearch.MessageType == MessageType.All)
                            //{
                            //    if (currentUser.AccountType == AccountType.Faculty)
                            //    {

                            //    }
                            //}                            
                            if (!string.IsNullOrEmpty(serializedUserSearch.Name))
                            {
                                hasSearch = true;
                                queryableUser = queryableUser.Where(x =>
                                    x.UserName.Contains(serializedUserSearch.Name)
                                    || x.FirstName.Contains(serializedUserSearch.Name)
                                    || x.LastName.Contains(serializedUserSearch.Name));
                            }
                            if (!hasSearch)
                            {
                                if (currentUser.AccountType != AccountType.Admin)
                                {
                                    lstApplicationUser = null;
                                    return Serializer.ReturnContent(lstApplicationUser, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                                }
                            }
                            lstApplicationUser = (from dbuser in queryableUser
                                                  select new ApplicationUser_vm
                                                  {
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
                                                      CollegeId = dbuser.CollegeId.Value,
                                                      CollegeName = dbuser.College.CollegeName,
                                                      DepartmentId = dbuser.DepartmentId.Value,
                                                      DepartmentName = dbuser.Department.DepartmentName,
                                                      WorkIdPicturePath = dbuser.WorkIdPicturePath,
                                                      ProfilePicturePath = dbuser.ProfilePicturePath,
                                                      //CreatedBy = dbuser.CreatedBy,
                                                      //CreatedOn = dbuser.CreatedOn,
                                                      //LastModifiedBy = dbuser.LastModifiedBy,
                                                  }).ToList();
                            return Serializer.ReturnContent(lstApplicationUser, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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
