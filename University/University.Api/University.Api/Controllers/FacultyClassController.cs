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
using University.Security.Models.ViewModel;
using University.Utilities;

namespace University.Api.Controllers
{
    public class FacultyClassController : UnSecuredController
    {
        public HttpResponseMessage Post(ApiViewModel apiViewModel)
        {
            //_logger.Info("Class HttpPost - Called");
            CurrentUser currentUser = null;
            UniversityContext dbContext = null;
            List<ClassDetail_vm> lstClassDetail = null;
            Tenant tenant = null;
            string data = string.Empty;
            int apiDataLogId = 0;
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
                            ClassDetail_vm serializedClass = JsonConvert.DeserializeObject<ClassDetail_vm>(apiViewModel.custom.ToString());
                            if (serializedClass != null)
                            {
                                dbContext = new UniversityContext();
                                if (serializedClass.FacultyId > 0)
                                {
                                    lstClassDetail = (from cD in dbContext.ClassDetails.Include("ApplicationUser").Include("College")
                                                .Where(x => x.ApplicationUser.StatusCode == StatusCodeConstants.ACTIVE && x.ApplicationUserId == serializedClass.FacultyId
                                                    && x.College.StatusCode == StatusCodeConstants.ACTIVE
                                                    && x.Department.StatusCode == StatusCodeConstants.ACTIVE
                                                    && x.StatusCode == StatusCodeConstants.ACTIVE
                                                    && x.TenantId == tenant.TenantId)
                                                      select new ClassDetail_vm
                                                      {
                                                          ClassDetailId = cD.ClassDetailId,
                                                          ClassName = cD.ClassName,
                                                          CollegeId = cD.CollegeId,
                                                          CollegeName = cD.College.CollegeName,
                                                          DepartmentId = cD.DepartmentId,
                                                          DepartmentName = cD.Department.DepartmentName,
                                                          IsPasswordProtected = cD.IsPasswordProtected,
                                                          FacultyId = cD.ApplicationUserId,
                                                          FacultyName = cD.ApplicationUser.FirstName + " " + cD.ApplicationUser.LastName,
                                                          From = cD.From,
                                                          To = cD.To,
                                                          Day = cD.Day.ToString(),
                                                          Link = cD.Link,
                                                          ClassRoomNo = cD.ClassRoomNo,
                                                          BuildingNo = cD.BuildingNo,
                                                          ExamDate = cD.ExamDate,
                                                          ExamName = cD.ExamName,
                                                          Location = cD.Location,
                                                          Notes = cD.Notes,
                                                          PostedDate = cD.CreatedOn.ToString(),
                                                          //DaysAgo = DbFunctions.DiffDays(cD.CreatedOn, DateTime.Today).Value,
                                                      }).OrderByDescending(x => x.PostedDate).ToList();
                                }
                                else
                                {
                                    lstClassDetail = (from cD in dbContext.ClassDetails.Include("ApplicationUser").Include("College")
                                                .Where(x => x.ApplicationUser.StatusCode == StatusCodeConstants.ACTIVE &&
                                                    x.College.StatusCode == StatusCodeConstants.ACTIVE
                                                    && x.Department.StatusCode == StatusCodeConstants.ACTIVE
                                                    && x.ApplicationUserId != currentUser.UserId
                                                    && x.StatusCode == StatusCodeConstants.ACTIVE
                                                    && x.TenantId == tenant.TenantId)
                                                      select new ClassDetail_vm
                                                      {
                                                          ClassDetailId = cD.ClassDetailId,
                                                          ClassName = cD.ClassName,
                                                          CollegeId = cD.CollegeId,
                                                          CollegeName = cD.College.CollegeName,
                                                          DepartmentId = cD.DepartmentId,
                                                          DepartmentName = cD.Department.DepartmentName,
                                                          IsPasswordProtected = cD.IsPasswordProtected,
                                                          FacultyId = cD.ApplicationUserId,
                                                          FacultyName = cD.ApplicationUser.FirstName + cD.ApplicationUser.LastName,
                                                          From = cD.From,
                                                          To = cD.To,
                                                          Day = cD.Day.ToString(),
                                                          Link = cD.Link,
                                                          ClassRoomNo = cD.ClassRoomNo,
                                                          BuildingNo = cD.BuildingNo,
                                                          ExamDate = cD.ExamDate,
                                                          Notes = cD.Notes,
                                                          PostedDate = cD.CreatedOn.ToString(),
                                                          //DaysAgo = DbFunctions.DiffDays(cD.CreatedOn, DateTime.Today).Value,
                                                      }).ToList();
                                }
                                if (lstClassDetail.HasValue())
                                {
                                    lstClassDetail.ForEach(x => Separate(x));
                                }
                                return Serializer.ReturnContent(lstClassDetail, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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

        private object Separate(ClassDetail_vm x)
        {
            if (x != null)
            {
                x.DaysAgo = TimeSpanFormat.FormatDaysAgo(x.PostedDate);
            }
            return x;
        }
    }
}
