using Newtonsoft.Json;
using System;
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

namespace University.Api.Controllers
{
    public class ProfileController : UnSecuredController
    {
        public HttpResponseMessage Post(ApiViewModel apiViewModel)
        {
            //_logger.Info("Profile HttpPost - Called");
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
                            apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Post, "Profile", data);
                            ApplicationUser_vm serializedApplicationUser = JsonConvert
                                .DeserializeObject<ApplicationUser_vm>(apiViewModel.custom.ToString());
                            if (serializedApplicationUser != null)
                            {
                                dbContext = new UniversityContext();

                                var dbuser = dbContext.ApplicationUsers.Include("College").Include("Department").Include("DummyColleges")
                                           .SingleOrDefault(x => x.ApplicationUserId == serializedApplicationUser.ApplicationUserId
                                               && x.TenantId == tenant.TenantId
                                               && x.StatusCode == StatusCodeConstants.ACTIVE);
                                if (dbuser != null)
                                {
                                    if (currentUser.UserId != serializedApplicationUser.ApplicationUserId)
                                    {
                                        ProfileVisit visit = new ProfileVisit
                                        {
                                            //ApplicationUserId = currentUser.UserId,
                                            CreatedBy = currentUser.UserId,
                                            CreatedOn = DateTime.Now,
                                            TenantId = currentUser.TenantId,
                                            StatusCode = StatusCodeConstants.ACTIVE,
                                            Language = Language.English
                                        };
                                        dbuser.ProfileVisits.Add(visit);
                                        dbContext.SaveChanges();
                                    }
                                    ApplicationUser_vm userVM = new ApplicationUser_vm
                                    {
                                        ApplicationUserId = dbuser.ApplicationUserId,
                                        FirstName = dbuser.FirstName,
                                        LastName = dbuser.LastName,
                                        UserName = dbuser.UserName,
                                        FullName = dbuser.FirstName + " " + dbuser.LastName,
                                        EmailAddress = dbuser.EmailAddress,
                                        Gender = dbuser.Gender,
                                        GenderName = dbuser.Gender.ToString(),
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
                                        DummyColleges = dbuser.DummyColleges,
                                        CreatedBy = dbuser.CreatedBy,
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
                                return Serializer.ReturnContent(HttpConstants.InvalidCurrentUser, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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

        public HttpResponseMessage Put(ApiViewModel apiViewModel)
        {
            //_logger.Info("Profile HttpPut - Called");
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
                        apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Put, "Profile", data);
                        ApplicationUser_vm serializedUser = JsonConvert.DeserializeObject<ApplicationUser_vm>(apiViewModel.custom.ToString());
                        if (serializedUser != null && !string.IsNullOrEmpty(currentUser.UserName))
                        {
                            dbContext = new UniversityContext();
                            var dbuser = dbContext.ApplicationUsers.SingleOrDefault(x => x.UserName == currentUser.UserName &&
                                x.TenantId == tenant.TenantId && x.StatusCode == StatusCodeConstants.ACTIVE);
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
