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
    public class FacultyAttendanceController : UnSecuredController
    {
        public HttpResponseMessage Post(ApiViewModel apiViewModel)
        {
            //_logger.Info("FacultyAttendance HttpPost - Called");
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
                    if (tenant.HasValue())
                    {
                        Token = apiViewModel.Token;
                        currentUser = ApiUser;
                        if (currentUser.HasValue())
                        {
                            data = JsonConvert.SerializeObject(apiViewModel.custom);
                            ApplicationUser_vm serializedUser = JsonConvert.DeserializeObject<ApplicationUser_vm>(apiViewModel.custom.ToString());
                            if (serializedUser != null)
                            {
                                dbContext = new UniversityContext();
                                var dbuser = dbContext.ApplicationUsers.Include("College").Include("Department")
                                           .SingleOrDefault(x => x.ApplicationUserId == currentUser.UserId && x.TenantId == tenant.TenantId
                                               && x.StatusCode == StatusCodeConstants.ACTIVE);
                                if (dbuser != null)
                                {
                                    ApplicationUser_vm userVM = new ApplicationUser_vm
                                    {
                                        ApplicationUserId = dbuser.ApplicationUserId,
                                        FirstName = dbuser.FirstName,
                                        LastName = dbuser.LastName,
                                        EmailAddress = dbuser.EmailAddress,
                                        Gender = dbuser.Gender,
                                        RefId = dbuser.RefId,
                                        Contact = dbuser.Contact,
                                        DOB = dbuser.DOB,
                                        AccountType = dbuser.AccountType,
                                        CollegeId = dbuser.CollegeId.Value,
                                        CollegeName = dbuser.College.CollegeName,
                                        DepartmentId = dbuser.DepartmentId.Value,
                                        DepartmentName = dbuser.Department.DepartmentName,
                                        WorkIdPicturePath = dbuser.WorkIdPicturePath,
                                        ProfilePicturePath = dbuser.ProfilePicturePath,
                                        CreatedOn = dbuser.CreatedOn,
                                        LastModifiedBy = dbuser.LastModifiedBy,
                                        LastModifiedOn = dbuser.LastModifiedOn
                                    };
                                    return Serializer.ReturnContent(userVM
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
