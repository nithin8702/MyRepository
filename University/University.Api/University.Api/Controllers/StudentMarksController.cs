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
    public class StudentMarksController : UnSecuredController
    {

        public HttpResponseMessage Get([FromUri]StudentMark_vm studentMark_vm)
        {
            //_logger.Info("StudentMark HttpGet - Called");
            UniversityContext dbContext = null;
            Tenant tenant = null;
            int apiDataLogId = 0;
            CurrentUser currentUser = null;
            List<StudentClass_vm> lstStudentClass = null;
            try
            {
                tenant = CurrentTenant;
                if (tenant.HasValue())
                {
                    dbContext = new UniversityContext();
                    lstStudentClass = (from studentClass in dbContext.StudentClasses.Include("ClassDetail").Include("StudentAttendances")
                                            .Where(x => x.ClassDetailId == studentMark_vm.classId && x.TenantId == tenant.TenantId
                                                && x.StatusCode == StatusCodeConstants.ACTIVE)
                                       select new StudentClass_vm
                                       {
                                           //NoOfDaysPresent=studentClass.StudentAttendances.Count(x=>x.IsPresent==true && x.StatusCode==StatusCodeConstants.ACTIVE),
                                           //NoOfDaysAbsent = studentClass.StudentAttendances.Count(x => x.IsAbsent == true && x.StatusCode == StatusCodeConstants.ACTIVE),
                                           //NoOfDaysLate = studentClass.StudentAttendances.Count(x => x.IsLate == true && x.StatusCode == StatusCodeConstants.ACTIVE),
                                           StudentClassId = studentClass.StudentClassId,
                                           ClassId = studentClass.ClassDetailId.Value,
                                           ClassName = studentClass.ClassDetail.ClassName,
                                           CreatedOn = studentClass.ClassDetail.CreatedOn.Value,
                                           StudentMark_vm = from mark in studentClass.StudentMarks.Where(x =>
                                               (studentMark_vm.userId == null || x.StudentId == studentMark_vm.userId)
                                               && x.TenantId == tenant.TenantId && x.StatusCode == StatusCodeConstants.ACTIVE)
                                                            join user in dbContext.ApplicationUsers
                                                            .Where(x => x.TenantId == tenant.TenantId && x.StatusCode == StatusCodeConstants.ACTIVE)
                                                            on mark.StudentId equals user.ApplicationUserId
                                                            orderby mark.StudentMarkId descending
                                                            select new StudentMark_vm
                                                            {
                                                                StudentMarkId = mark.StudentMarkId,
                                                                StudentId = mark.StudentId.Value,
                                                                RefId = user.RefId,
                                                                StudentName = user.FirstName + " " + user.LastName,
                                                                ExamName = mark.ExamName,
                                                                ExamDate = mark.ExamDate,
                                                                Comments = mark.Comments,
                                                                Mark = mark.Mark
                                                            }
                                       }).OrderByDescending(x => x.StudentClassId)
                                             .ToList();
                    if (lstStudentClass.HasValue())
                    {
                        lstStudentClass.ForEach(x => Separate(x));
                    }
                    return Serializer.ReturnContent(lstStudentClass, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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
                if (!currentUser.HasValue())
                {
                    currentUser = new CurrentUser { TenantId = tenant.TenantId };
                }
                ErrorLog.LogCustomError(currentUser, ex, apiDataLogId);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        private object Separate(StudentClass_vm x)
        {
            if (x != null && x.StudentMark_vm != null && x.StudentMark_vm.Count() > 0)
            {
                decimal avg = x.StudentMark_vm.Where(y => y.Mark >= 0).Sum(y => y.Mark);
                if (avg >= 95)
                {
                    x.Grade = "A";
                }
                else if (avg <= 94 && avg >= 90)
                {
                    x.Grade = "A-";
                }
                else if (avg <= 89 && avg >= 87)
                {
                    x.Grade = "B+";
                }
                else if (avg <= 86 && avg >= 83)
                {
                    x.Grade = "B";
                }
                else if (avg <= 82 && avg >= 80)
                {
                    x.Grade = "B-";
                }
                else if (avg <= 79 && avg >= 77)
                {
                    x.Grade = "C+";
                }
                else if (avg <= 76 && avg >= 73)
                {
                    x.Grade = "C";
                }
                else if (avg <= 72 && avg >= 70)
                {
                    x.Grade = "C-";
                }
                else if (avg <= 69 && avg >= 65)
                {
                    x.Grade = "D+";
                }
                else if (avg <= 64 && avg >= 60)
                {
                    x.Grade = "D";
                }
                else
                {
                    x.Grade = "F";
                }
            }
            return x;
        }

        public HttpResponseMessage Post(ApiViewModel apiViewModel)
        {
            //_logger.Info("StudentMarks HttpPost - Called");
            CurrentUser currentUser = null;
            UniversityContext dbContext = null;
            int apiDataLogId = 0;
            Tenant tenant = null;
            string data = string.Empty;
            List<StudentMark> lstStudentMark = null;
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
                            apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Post, "StudentMarks", data);
                            StudentClass_vm serializedStudentClass = JsonConvert
                                .DeserializeObject<StudentClass_vm>(apiViewModel.custom.ToString());
                            if (serializedStudentClass != null)
                            {
                                dbContext = new UniversityContext();
                                StudentClass studentClass = dbContext.StudentClasses.Include("ClassDetail")
                                    .SingleOrDefault(x => x.ClassDetailId == serializedStudentClass.ClassId &&
                                    x.ClassDetail.ApplicationUserId == currentUser.UserId && x.TenantId == tenant.TenantId);
                                if (studentClass == null)
                                {
                                    studentClass = new StudentClass
                                    {
                                        ClassDetailId = serializedStudentClass.ClassId,
                                        TenantId = tenant.TenantId,
                                        StatusCode = StatusCodeConstants.ACTIVE,
                                        Language = Language.English,
                                        CreatedBy = currentUser.UserId,
                                        CreatedOn = DateTime.Now
                                    };
                                    dbContext.StudentClasses.Add(studentClass);
                                    //dbContext.SaveChanges();
                                }
                                if (serializedStudentClass.StudentMark_vm.ToList().HasValue())
                                {
                                    lstStudentMark = new List<StudentMark>();
                                    foreach (var item in serializedStudentClass.StudentMark_vm)
                                    {
                                        if (item != null)
                                        {
                                            StudentMark studentMark = new StudentMark
                                            {
                                                StudentClassId = studentClass.StudentClassId,
                                                StudentId = item.StudentId,
                                                ExamDate = item.ExamDate,
                                                ExamName = item.ExamName,
                                                Mark = item.Mark,
                                                Comments = item.Comments,
                                                TenantId = tenant.TenantId,
                                                StatusCode = StatusCodeConstants.ACTIVE,
                                                Language = Language.English,
                                                CreatedBy = currentUser.UserId,
                                                CreatedOn = DateTime.Now
                                            };
                                            lstStudentMark.Add(studentMark);
                                            studentClass.StudentMarks.Add(studentMark);
                                        }
                                    }
                                }
                                dbContext.SaveChanges();
                                return Serializer.ReturnContent(lstStudentMark
                                    , this.Configuration.Services.GetContentNegotiator()
                                    , this.Configuration.Formatters, this.Request);
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

        public HttpResponseMessage Put(ApiViewModel apiViewModel)
        {
            //_logger.Info("StudentMarks HttpPut - Called");
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
                    if (tenant != null)
                    {
                        Token = apiViewModel.Token;
                        currentUser = ApiUser;
                        if (currentUser.HasValue())
                        {
                            data = JsonConvert.SerializeObject(apiViewModel.custom);
                            apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Put, "StudentMarks", data);
                            List<StudentMark_vm> serializedStudentMark = JsonConvert
                                .DeserializeObject<List<StudentMark_vm>>(apiViewModel.custom.ToString());
                            if (serializedStudentMark.HasValue())
                            {
                                dbContext = new UniversityContext();
                                foreach (var item in serializedStudentMark)
                                {
                                    if (item != null && item.StudentMarkId > 0)
                                    {
                                        StudentMark studentMark = dbContext.StudentMarks
                                            .SingleOrDefault(x => x.StudentMarkId == item.StudentMarkId
                                                && x.TenantId == tenant.TenantId);
                                        if (studentMark != null)
                                        {
                                            studentMark.ExamName = item.ExamName;
                                            studentMark.ExamDate = item.ExamDate;
                                            studentMark.Mark = item.Mark;
                                            studentMark.Comments = item.Comments;
                                            studentMark.LastModifiedBy = currentUser.UserId;
                                            studentMark.LastModifiedOn = DateTime.Now;
                                        }
                                    }
                                }
                                dbContext.SaveChanges();
                            }
                            return Serializer.ReturnContent(HttpConstants.Updated
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

        public HttpResponseMessage Delete(ApiViewModel apiViewModel)
        {
            //_logger.Info("StudentMarks HttpDelete - Called");
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
                    if (tenant != null)
                    {
                        Token = apiViewModel.Token;
                        currentUser = ApiUser;
                        if (currentUser.HasValue())
                        {
                            data = JsonConvert.SerializeObject(apiViewModel.custom);
                            apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Delete, "StudentMarks", data);
                            List<StudentMark_vm> serializedStudentMark = JsonConvert
                                .DeserializeObject<List<StudentMark_vm>>(apiViewModel.custom.ToString());
                            if (serializedStudentMark != null)
                            {
                                dbContext = new UniversityContext();
                                if (serializedStudentMark.ToList().HasValue())
                                {
                                    foreach (var item in serializedStudentMark)
                                    {
                                        if (item != null && item.StudentMarkId > 0)
                                        {
                                            StudentMark studentMark = dbContext.StudentMarks
                                                .SingleOrDefault(x => x.StudentMarkId == item.StudentMarkId
                                                    && x.TenantId == tenant.TenantId);
                                            if (studentMark != null)
                                            {
                                                dbContext.StudentMarks.Remove(studentMark);
                                            }
                                        }
                                    }
                                    dbContext.SaveChanges();
                                }
                                return Serializer.ReturnContent(HttpConstants.Deleted
                                    , this.Configuration.Services.GetContentNegotiator()
                                    , this.Configuration.Formatters, this.Request);
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

    }
}
