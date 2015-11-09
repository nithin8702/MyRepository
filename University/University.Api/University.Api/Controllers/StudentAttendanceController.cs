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
    public class StudentAttendanceController : UnSecuredController
    {
        public HttpResponseMessage Get([FromUri]StudentAttendance_vm studentAttendance_vm)
        {
            //_logger.Info("StudentAttendance HttpGet - Called");
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
                    lstStudentClass = (from studentClass in dbContext.StudentClasses.Include("ClassDetail")
                                            .Where(x => x.ClassDetailId == studentAttendance_vm.classId
                                                && x.TenantId == tenant.TenantId && x.StatusCode == StatusCodeConstants.ACTIVE)
                                       select new StudentClass_vm
                                             {
                                                 StudentClassId = studentClass.StudentClassId,
                                                 ClassId = studentClass.ClassDetailId.Value,
                                                 ClassName = studentClass.ClassDetail.ClassName,
                                                 CreatedOn = studentClass.ClassDetail.CreatedOn.Value,
                                                 StudentAttendance_vm = from attn in studentClass.StudentAttendances
                                                                            .Where(x => (studentAttendance_vm.userId == null
                                                                                || x.StudentId == studentAttendance_vm.userId)
                                                                                && x.TenantId == tenant.TenantId
                                                                                && x.StatusCode == StatusCodeConstants.ACTIVE)
                                                                        join user in dbContext.ApplicationUsers
                                                                        .Where(x => x.TenantId == tenant.TenantId && x.StatusCode==StatusCodeConstants.ACTIVE) 
                                                                        on attn.StudentId equals user.ApplicationUserId
                                                                        orderby attn.StudentAttendanceId descending
                                                                        select new StudentAttendance_vm
                                                                        {
                                                                            StudentAttendanceId = attn.StudentAttendanceId,
                                                                            StudentId = attn.StudentId.Value,
                                                                            RefId=user.RefId,
                                                                            IsAbsent = attn.IsAbsent,
                                                                            IsPresent = attn.IsPresent,
                                                                            IsLate = attn.IsLate,
                                                                            LateTime = attn.LateTime,
                                                                            Comments = attn.Comments,
                                                                            ClassDate = attn.ClassDate,
                                                                            UserName = user.FirstName + " " + user.LastName,
                                                                            FirstName = user.FirstName,
                                                                            LastName = user.LastName,
                                                                            EmailAddress = attn.Student.EmailAddress
                                                                        }
                                             }).OrderByDescending(x => x.StudentClassId)
                                             .ToList();
                    //if (lstStudentClass.HasValue())
                    //{
                    //    lstStudentClass.ForEach(x => CountDays(x));
                    //}
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

        //private object CountDays(StudentClass_vm x)
        //{
        //    if (x != null)
        //    {
        //        x.NoOfDaysPresent = x.StudentAttendance_vm.Count(y => y.IsPresent == true);
        //        x.NoOfDaysAbsent = x.StudentAttendance_vm.Count(y => y.IsAbsent == true);
        //        x.NoOfDaysLate = x.StudentAttendance_vm.Count(y => y.IsLate == true);
        //    }
        //    return x;
        //}

        public HttpResponseMessage Post(ApiViewModel apiViewModel)
        {
            //_logger.Info("StudentAttendance HttpPost - Called");
            CurrentUser currentUser = null;
            UniversityContext dbContext = null;
            int apiDataLogId = 0;
            Tenant tenant = null;
            string data = string.Empty;
            List<StudentAttendance> lstStudentAttendance = null;
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
                            apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Post, "StudentClass", data);
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
                                if (serializedStudentClass.StudentAttendance_vm.ToList().HasValue())
                                {
                                    lstStudentAttendance = new List<StudentAttendance>();
                                    foreach (var item in serializedStudentClass.StudentAttendance_vm)
                                    {
                                        if (item != null)
                                        {
                                            StudentAttendance attendance = new StudentAttendance
                                            {
                                                StudentClassId = studentClass.StudentClassId,
                                                StudentId = item.StudentId,
                                                ClassDate = item.ClassDate,
                                                IsAbsent = item.IsAbsent,
                                                IsPresent = item.IsPresent,
                                                IsLate = item.IsLate,
                                                LateTime = item.LateTime,
                                                Comments = item.Comments,
                                                TenantId = tenant.TenantId,
                                                StatusCode = StatusCodeConstants.ACTIVE,
                                                Language = Language.English,
                                                CreatedBy = currentUser.UserId,
                                                CreatedOn = DateTime.Now
                                            };
                                            lstStudentAttendance.Add(attendance);
                                            studentClass.StudentAttendances.Add(attendance);
                                        }
                                    }
                                }
                                dbContext.SaveChanges();
                                return Serializer.ReturnContent(lstStudentAttendance
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

        public HttpResponseMessage Put(ApiViewModel apiViewModel)
        {
            //_logger.Info("StudentAttendance HttpPut - Called");
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
                            apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Put, "StudentAttendance", data);
                            List<StudentAttendance_vm> serializedStudentAttendance = JsonConvert
                                .DeserializeObject<List<StudentAttendance_vm>>(apiViewModel.custom.ToString());
                            if (serializedStudentAttendance.ToList().HasValue())
                            {
                                dbContext = new UniversityContext();
                                foreach (var item in serializedStudentAttendance)
                                {
                                    if (item != null && item.StudentAttendanceId > 0)
                                    {
                                        StudentAttendance attendance = dbContext.StudentAttendances
                                            .SingleOrDefault(x => x.StudentAttendanceId == item.StudentAttendanceId
                                                && x.TenantId == tenant.TenantId);
                                        if (attendance != null)
                                        {
                                            attendance.ClassDate = item.ClassDate;
                                            attendance.IsPresent = item.IsPresent;
                                            attendance.IsAbsent = item.IsAbsent;
                                            attendance.IsLate = item.IsLate;
                                            attendance.LateTime = item.LateTime;
                                            attendance.Comments = item.Comments;
                                            attendance.LastModifiedBy = currentUser.UserId;
                                            attendance.LastModifiedOn = DateTime.Now;
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
            //_logger.Info("StudentAttendance HttpDelete - Called");
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
                            apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Delete, "StudentAttendance", data);
                            List<StudentAttendance_vm> serializedStudentAttendance = JsonConvert
                                .DeserializeObject<List<StudentAttendance_vm>>(apiViewModel.custom.ToString());
                            if (serializedStudentAttendance.HasValue())
                            {
                                dbContext = new UniversityContext();
                                foreach (var item in serializedStudentAttendance)
                                {
                                    if (item != null && item.StudentAttendanceId > 0)
                                    {
                                        StudentAttendance attendance = dbContext.StudentAttendances
                                            .SingleOrDefault(x => x.StudentAttendanceId == item.StudentAttendanceId
                                                && x.TenantId == tenant.TenantId);
                                        if (attendance != null)
                                        {
                                            dbContext.StudentAttendances.Remove(attendance);
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
