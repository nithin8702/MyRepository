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
using University.Security.Models.ViewModel;
using University.Utilities;

namespace University.Api.Controllers
{
    public class AttendanceDetailsController : UnSecuredController
    {
        public HttpResponseMessage Post(ApiViewModel apiViewModel)
        {
            //_logger.Info("AttendanceDetails HttpPost - Called");
            CurrentUser currentUser = null;
            UniversityContext dbContext = null;
            int apiDataLogId = 0;
            Tenant tenant = null;
            string data = string.Empty;
            List<StudentAttendanceDetail_vm> lstStudentAttendanceDetail = new List<StudentAttendanceDetail_vm>();
            bool hasClass = false;
            try
            {

                tenant = CurrentTenant;
                if (tenant != null)
                {
                    Token = apiViewModel.Token;
                    currentUser = ApiUser;
                    if (currentUser.HasValue())
                    {
                        dbContext = new UniversityContext();
                        //List<StudentClass> studentClasses = null;
                        if (apiViewModel.custom != null)
                        {
                            StudentAttendanceDetail_vm serializedStudentAttendanceDetail = JsonConvert.DeserializeObject<StudentAttendanceDetail_vm>(apiViewModel.custom.ToString());
                            if (serializedStudentAttendanceDetail != null && serializedStudentAttendanceDetail.ClassId > 0)
                            {
                                hasClass = true;
                                var studentClasses1 = (from classes in dbContext.StudentClasses.Where(x => x.StatusCode == StatusCodeConstants.ACTIVE
                                && x.TenantId == tenant.TenantId && x.ClassDetailId == serializedStudentAttendanceDetail.ClassId)
                                                       select new { ClassDetailId = classes.ClassDetailId, StudentClassId = classes.StudentClassId }).ToList();
                                if (studentClasses1.HasValue())
                                {
                                    foreach (var cls in studentClasses1)
                                    {
                                        if (cls.StudentClassId > 0)
                                        {
                                            var attn = dbContext.StudentAttendances.Where(x => x.TenantId == tenant.TenantId
                                            && x.StatusCode == StatusCodeConstants.ACTIVE
                                            && x.StudentClassId == cls.StudentClassId).GroupBy(x => x.StudentId).ToList();
                                            if (attn.HasValue())
                                            {
                                                foreach (var item in attn)
                                                {
                                                    lstStudentAttendanceDetail.Add(new StudentAttendanceDetail_vm
                                                    {
                                                        StudentId = item.Select(x => x.StudentId).FirstOrDefault().Value,
                                                        ClassId = cls.ClassDetailId.Value,
                                                        NoOfDaysPresent = item.Count(x => x.IsPresent == true),
                                                        NoOfDaysAbsent = item.Count(x => x.IsAbsent == true),
                                                        NoOfDaysLate = item.Count(x => x.IsLate == true)
                                                    });
                                                }
                                            }
                                        }
                                    }
                                }

                            }
                        }
                        if (!hasClass)
                        {
                            var studentClasses2 = (from classes in dbContext.StudentClasses.Where(x => x.CreatedBy == currentUser.UserId && x.StatusCode == StatusCodeConstants.ACTIVE
                            && x.TenantId == tenant.TenantId)
                                                   select new { ClassDetailId = classes.ClassDetailId, StudentClassId = classes.StudentClassId }).ToList();
                            if (studentClasses2.HasValue())
                            {
                                foreach (var cls in studentClasses2)
                                {
                                    if (cls.StudentClassId > 0)
                                    {
                                        var attn = dbContext.StudentAttendances.Where(x => x.TenantId == tenant.TenantId
                                        && x.StatusCode == StatusCodeConstants.ACTIVE
                                        && x.StudentClassId == cls.StudentClassId).GroupBy(x => x.StudentId).ToList();
                                        if (attn.HasValue())
                                        {
                                            foreach (var item in attn)
                                            {
                                                lstStudentAttendanceDetail.Add(new StudentAttendanceDetail_vm
                                                {
                                                    StudentId = item.Select(x => x.StudentId).FirstOrDefault().Value,
                                                    ClassId = cls.ClassDetailId.Value,
                                                    NoOfDaysPresent = item.Count(x => x.IsPresent == true),
                                                    NoOfDaysAbsent = item.Count(x => x.IsAbsent == true),
                                                    NoOfDaysLate = item.Count(x => x.IsLate == true)
                                                });
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        return Serializer.ReturnContent(lstStudentAttendanceDetail
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
    }
}
