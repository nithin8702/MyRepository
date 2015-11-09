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
using University.Bussiness.Models.ViewModel;
using University.Common.Models;
using University.Common.Models.Security;
using University.Constants;
using University.Context;
using University.Security.Models.ViewModel;

namespace University.Api.Controllers
{
    public class FacultyStudentsController : UnSecuredController
    {
        public HttpResponseMessage Post(ApiViewModel apiViewModel)
        {
            //_logger.Info("Faculty Students HttpPost - Called");
            CurrentUser currentUser = null;
            int apiDataLogId = 0;
            Tenant tenant = null;
            string data = string.Empty;
            UniversityContext dbContext = null;
            List<FacultyStudents_vm> lstApplicationUser_vm = null;
            try
            {
                tenant = CurrentTenant;
                if (tenant.HasValue())
                {
                    Token = apiViewModel.Token;
                    currentUser = ApiUser;
                    if (currentUser.HasValue())
                    {
                        data = JsonConvert.SerializeObject(apiViewModel.custom);
                        apiDataLogId = DataLog
                            .LogData(currentUser, VerbConstants.Post, "Faculty Students", data);
                        dbContext = new UniversityContext();
                        _logger.Info("currentUser.UserId" + currentUser.UserId);
                        lstApplicationUser_vm = (from cD in dbContext.ClassDetails.Include("College")
                                                     .Include("Department").Where(x => x.ApplicationUserId == currentUser.UserId
                                                         && x.StatusCode == StatusCodeConstants.ACTIVE && x.College.StatusCode == StatusCodeConstants.ACTIVE
                                                         && x.Department.StatusCode == StatusCodeConstants.ACTIVE
                                                         && x.TenantId == tenant.TenantId)
                                                 join
                                                 student in dbContext.StudentSubscriptions
                                                 on cD.ClassDetailId equals student.ClassDetailId
                                                 join user in dbContext.ApplicationUsers
                                                 .Where(x => x.StatusCode == StatusCodeConstants.ACTIVE)
                                                 on student.ApplicationUserId equals user.ApplicationUserId
                                                 select new FacultyStudents_vm
                                                 {
                                                     ApplicationUserId = student.ApplicationUserId,
                                                     FirstName = user.FirstName,
                                                     LastName = user.LastName,
                                                     UserName = user.FirstName + " " + user.LastName,
                                                     EmailAddress = user.EmailAddress,
                                                     Gender = user.Gender,
                                                     GenderName = user.Gender.ToString(),
                                                     RefId = user.RefId,
                                                     Contact = user.Contact,
                                                     WorkIdPicturePath = user.WorkIdPicturePath,
                                                     ProfilePicturePath = user.ProfilePicturePath,
                                                     StatusCode = student.StatusCode,
                                                     CollegeId = cD.CollegeId,
                                                     CollegeName = cD.College.CollegeName,
                                                     DepartmentId = cD.DepartmentId,
                                                     DepartmentName = cD.Department.DepartmentName,
                                                     ClassDetailId = cD.ClassDetailId,
                                                     ClassName = cD.ClassName
                                                 }).ToList();
                        return Serializer.ReturnContent(lstApplicationUser_vm, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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
    }
}
