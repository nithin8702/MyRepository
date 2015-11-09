using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
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
using University.Security.Models.ViewModel;
using University.Utilities;

namespace University.Api.Controllers
{
    public class ClassController : UnSecuredController
    {
        public HttpResponseMessage Get()
        {
            //_logger.Info("Class HttpGet All - Called");
            UniversityContext dbcontext = null;
            List<ClassDetail_vm> lstClassDetail = null;
            Tenant tenant = null;
            int apiDataLogId = 0;
            CurrentUser currentUser = null;
            try
            {
                tenant = CurrentTenant;
                if (tenant.HasValue())
                {
                    dbcontext = new UniversityContext();
                    lstClassDetail = (from cD in dbContext.ClassDetails.Include("ApplicationUser").Include("College")
                                   .Where(x =>
                                       x.StatusCode == StatusCodeConstants.ACTIVE
                                       && x.College.StatusCode == StatusCodeConstants.ACTIVE
                                       && x.Department.StatusCode == StatusCodeConstants.ACTIVE
                                       && x.TenantId == tenant.TenantId)
                                      join
                                     dept in dbContext.Departments on cD.DepartmentId equals dept.DepartmentId
                                      select new ClassDetail_vm
                                      {
                                          ClassDetailId = cD.ClassDetailId,
                                          ClassName = cD.ClassName,
                                          CollegeId = cD.CollegeId,
                                          CollegeName = cD.College.CollegeName,
                                          DepartmentId = cD.DepartmentId,
                                          DepartmentName=dept.DepartmentName,
                                          IsPasswordProtected = cD.IsPasswordProtected,
                                          FacultyId = cD.ApplicationUserId,
                                          FacultyName = cD.ApplicationUser.FirstName + " " + cD.ApplicationUser.LastName,
                                          From = cD.From,
                                          To = cD.To,
                                          Day = cD.Day.ToString(),
                                          Location = cD.Location,
                                          Link = cD.Link,
                                          ClassRoomNo = cD.ClassRoomNo,
                                          BuildingNo = cD.BuildingNo,
                                          ExamDate = cD.ExamDate,
                                          ExamName = cD.ExamName,
                                          Notes = cD.Notes,
                                          PostedDate = cD.CreatedOn.ToString(),
                                          //DaysAgo = DbFunctions.DiffDays(cD.CreatedOn, DateTime.Today).Value,
                                      }).ToList();
                    if (lstClassDetail.HasValue())
                    {
                        lstClassDetail.ForEach(x => Separate(x));
                    }
                    return Serializer.ReturnContent(lstClassDetail, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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

        private object Separate(ClassDetail_vm x)
        {
            if (x != null)
            {
                x.DaysAgo = TimeSpanFormat.FormatDaysAgo(x.PostedDate);
            }
            return x;
        }

        public HttpResponseMessage Get([FromUri]int classId)
        {
            //_logger.Info("Class HttpGet - Called");
            UniversityContext dbcontext = null;
            List<ClassDetail_vm> lstClassDetail = null;
            Tenant tenant = null;
            int apiDataLogId = 0;
            CurrentUser currentUser = null;
            try
            {
                tenant = CurrentTenant;
                if (tenant.HasValue())
                {
                    dbcontext = new UniversityContext();
                    lstClassDetail = (from cD in dbContext.ClassDetails.Include("ApplicationUser").Include("College")
                                   .Where(x => x.ClassDetailId == classId
                                       && x.College.StatusCode == StatusCodeConstants.ACTIVE
                                       && x.Department.StatusCode == StatusCodeConstants.ACTIVE
                                       && x.StatusCode == StatusCodeConstants.ACTIVE
                                       && x.TenantId == tenant.TenantId)
                                      join
                                      dept in dbContext.Departments on cD.DepartmentId equals dept.DepartmentId
                                      select new ClassDetail_vm
                                      {
                                          ClassDetailId = cD.ClassDetailId,
                                          ClassName = cD.ClassName,
                                          CollegeId = cD.CollegeId,
                                          CollegeName = cD.College.CollegeName,
                                          DepartmentId = cD.DepartmentId,
                                          DepartmentName = dept.DepartmentName,
                                          IsPasswordProtected = cD.IsPasswordProtected,
                                          FacultyId = cD.ApplicationUserId,
                                          FacultyName = cD.ApplicationUser.FirstName +" " + cD.ApplicationUser.LastName,
                                          From = cD.From,
                                          To = cD.To,
                                          Day = cD.Day.ToString(),
                                          Location = cD.Location,
                                          Link = cD.Link,
                                          ClassRoomNo = cD.ClassRoomNo,
                                          BuildingNo = cD.BuildingNo,
                                          ExamDate = cD.ExamDate,
                                          ExamName = cD.ExamName,
                                          Notes = cD.Notes,
                                          PostedDate = cD.CreatedOn.ToString(),
                                          //DaysAgo = DbFunctions.DiffDays(cD.CreatedOn, DateTime.Today).Value,
                                      }).ToList();
                    if (lstClassDetail.HasValue())
                    {
                        lstClassDetail.ForEach(x => Separate(x));
                    }
                    return Serializer.ReturnContent(lstClassDetail, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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

        public HttpResponseMessage Post(ApiViewModel apiViewModel)
        {
            //_logger.Info("Class HttpPost - Called");
            CurrentUser currentUser = null;
            UniversityContext dbContext = null;
            int apiDataLogId = 0;
            Tenant tenant = null;
            string data = string.Empty;
            string se = default(string);
            string strCurrentDate = default(string);
            byte[] passwordSalt = default(byte[]);
            byte[] passwordHash = default(byte[]);
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
                            if (currentUser.AccountType == AccountType.Faculty)
                            {
                                data = JsonConvert.SerializeObject(apiViewModel.custom);
                                apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Post, "Class", data);
                                ClassDetail_vm serializedClassDetail = JsonConvert
                                    .DeserializeObject<ClassDetail_vm>(apiViewModel.custom.ToString());
                                if (serializedClassDetail != null)
                                {
                                    dbContext = new UniversityContext();
                                    if (serializedClassDetail.IsPasswordProtected)
                                    {
                                        strCurrentDate = DateTime.Now.ToString();
                                        passwordSalt = Encryptor.EncryptText(strCurrentDate, currentUser.UserName);
                                        se = Convert.ToBase64String(passwordSalt);
                                        passwordHash = Encryptor.GenerateHash(serializedClassDetail.Password, se.ToString());
                                    }
                                    ClassDetail cD = new ClassDetail
                                    {
                                        CollegeId = serializedClassDetail.CollegeId,
                                        ClassName = serializedClassDetail.ClassName,
                                        DepartmentId = serializedClassDetail.DepartmentId,
                                        IsPasswordProtected = serializedClassDetail.IsPasswordProtected,
                                        PasswordHash = passwordHash,
                                        PasswordSalt = passwordSalt,
                                        From = serializedClassDetail.From,
                                        To = serializedClassDetail.To,
                                        Day = serializedClassDetail.Day,
                                        Link = serializedClassDetail.Link,
                                        ClassRoomNo = serializedClassDetail.ClassRoomNo,
                                        BuildingNo = serializedClassDetail.BuildingNo,
                                        ExamDate = serializedClassDetail.ExamDate,
                                        ExamName = serializedClassDetail.ExamName,
                                        Location = serializedClassDetail.Location,
                                        Notes = serializedClassDetail.Notes,
                                        ApplicationUserId = currentUser.UserId,
                                        CreatedBy = currentUser.UserId,
                                        CreatedOn = DateTime.Now,
                                        TenantId = currentUser.TenantId,
                                        StatusCode = StatusCodeConstants.ACTIVE,
                                        Language = Language.English
                                    };
                                    dbContext.ClassDetails.Add(cD);
                                    dbContext.SaveChanges();
                                    return Serializer.ReturnContent(HttpConstants.Inserted
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
                                _logger.Warn("Not a valid faculty");
                                return Serializer.ReturnContent("Not a valid faculty", this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
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
            //_logger.Info("Class HttpPut - Called");
            CurrentUser currentUser = null;
            UniversityContext dbContext = null;
            int apiDataLogId = 0;
            Tenant tenant = null;
            string data = string.Empty;
            ClassDetail cD = null;
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
                            apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Put, "Class", data);
                            ClassDetail_vm serializedClassDetail = JsonConvert
                                .DeserializeObject<ClassDetail_vm>(apiViewModel.custom.ToString());
                            if (serializedClassDetail != null)
                            {
                                dbContext = new UniversityContext();
                                cD = dbContext.ClassDetails.SingleOrDefault(x => x.ClassDetailId == serializedClassDetail.ClassDetailId
                                    && x.ApplicationUserId == currentUser.UserId
                                    && x.StatusCode == StatusCodeConstants.ACTIVE && x.TenantId == tenant.TenantId);
                                if (cD != null)
                                {
                                    cD.CollegeId = serializedClassDetail.CollegeId;
                                    cD.ClassName = serializedClassDetail.ClassName;
                                    cD.DepartmentId = serializedClassDetail.DepartmentId;
                                    cD.From = serializedClassDetail.From;
                                    cD.To = serializedClassDetail.To;
                                    cD.Day = serializedClassDetail.Day;
                                    cD.Link = serializedClassDetail.Link;
                                    cD.ClassRoomNo = serializedClassDetail.ClassRoomNo;
                                    cD.BuildingNo = serializedClassDetail.BuildingNo;
                                    cD.ExamDate = serializedClassDetail.ExamDate;
                                    cD.ExamName = serializedClassDetail.ExamName;
                                    cD.Notes = serializedClassDetail.Notes;
                                    cD.ApplicationUserId = currentUser.UserId;
                                    cD.LastModifiedBy = currentUser.UserId;
                                    cD.LastModifiedOn = DateTime.Now;
                                    cD.TenantId = currentUser.TenantId;
                                    cD.StatusCode = StatusCodeConstants.ACTIVE;
                                    cD.Language = Language.English;
                                    cD.Location = serializedClassDetail.Location;
                                    dbContext.Entry<ClassDetail>(cD).State = EntityState.Modified;
                                    dbContext.SaveChanges();

                                    return Serializer.ReturnContent(HttpConstants.Updated
                                    , this.Configuration.Services.GetContentNegotiator()
                                    , this.Configuration.Formatters, this.Request);
                                }
                                else
                                {
                                    _logger.Warn(HttpConstants.CLASSDOESNOTEXISTS);
                                    return Serializer.ReturnContent(HttpConstants.CLASSDOESNOTEXISTS, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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
            catch (DbEntityValidationException ex)
            {
                foreach (var item1 in ex.EntityValidationErrors)
                {
                    foreach (var item in item1.ValidationErrors.ToList())
                    {
                        _logger.Error(item.PropertyName + " - " + item.ErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                ErrorLog.LogCustomError(currentUser, ex, apiDataLogId);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
            return Serializer.ReturnContent(HttpStatusCode.NotImplemented
                            , this.Configuration.Services.GetContentNegotiator()
                            , this.Configuration.Formatters, this.Request);
        }

        public HttpResponseMessage Delete(ApiViewModel apiViewModel)
        {
            //_logger.Info("Class Delete - Called");
            CurrentUser currentUser = null;
            UniversityContext dbContext = null;
            int apiDataLogId = 0;
            Tenant tenant = null;
            string data = string.Empty;
            ClassDetail cD = null;
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
                            apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Delete, "Class", data);
                            ClassDetail_vm serializedClassDetail = JsonConvert
                                .DeserializeObject<ClassDetail_vm>(apiViewModel.custom.ToString());
                            if (serializedClassDetail != null)
                            {
                                dbContext = new UniversityContext();
                                cD = dbContext.ClassDetails.SingleOrDefault(x => x.ClassDetailId == serializedClassDetail.ClassDetailId
                                    && x.ApplicationUserId == currentUser.UserId
                                    && x.StatusCode == StatusCodeConstants.ACTIVE && x.TenantId == tenant.TenantId);
                                if (cD != null)
                                {
                                    cD.StatusCode = StatusCodeConstants.INACTIVE;
                                    cD.LastModifiedBy = currentUser.UserId;
                                    cD.LastModifiedOn = DateTime.Now;
                                    dbContext.SaveChanges();

                                    return Serializer.ReturnContent(HttpConstants.Deleted
                                    , this.Configuration.Services.GetContentNegotiator()
                                    , this.Configuration.Formatters, this.Request);
                                }
                                else
                                {
                                    _logger.Warn(HttpConstants.CLASSDOESNOTEXISTS);
                                    return Serializer.ReturnContent(HttpConstants.CLASSDOESNOTEXISTS, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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
