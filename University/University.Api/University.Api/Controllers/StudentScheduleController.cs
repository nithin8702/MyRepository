using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using University.Api.Controllers.Log;
using University.Api.Controllers.Serialize;
using University.Common.Models.Security;
using University.Constants;
using University.Context;
using University.Api.Extensions;
using University.Bussiness.Models.ViewModel;
using University.Common.Models;
using System.Net;
using University.Security.Models.ViewModel;
using Newtonsoft.Json;
using University.Bussiness.Models;
using University.Common.Models.Enums;
using University.Utilities;

namespace University.Api.Controllers
{
    public class StudentScheduleController : UnSecuredController
    {
        public HttpResponseMessage Post(ApiViewModel apiViewModel)
        {
            //_logger.Info("StudentSchedule HttpPost - Called");
            UniversityContext dbContext = null;
            List<ClassDetail_vm> lstClassDetail = null;
            Tenant tenant = null;
            int apiDataLogId = 0;
            CurrentUser currentUser = null;
            try
            {
                tenant = CurrentTenant;
                if (tenant.HasValue())
                {
                    Token = apiViewModel.Token;
                    currentUser = ApiUser;
                    if (currentUser.HasValue())
                    {
                        dbContext = new UniversityContext();
                        lstClassDetail = (from cD in dbContext.StudentSubscriptions.Include("ClassDetail")
                                       .Where(x => x.ApplicationUserId == currentUser.UserId
                                           && x.StatusCode == StatusCodeConstants.ACTIVE
                                           && x.ClassDetail.StatusCode==StatusCodeConstants.ACTIVE
                                           && x.ClassDetail.College.StatusCode==StatusCodeConstants.ACTIVE
                                           && x.ClassDetail.Department.StatusCode==StatusCodeConstants.ACTIVE
                                           && x.TenantId == tenant.TenantId)
                                          select new ClassDetail_vm
                                          {
                                              ClassDetailId = cD.ClassDetailId.Value,
                                              ClassName = cD.ClassDetail.ClassName,
                                              CollegeId = cD.ClassDetail.CollegeId,
                                              CollegeName = cD.ClassDetail.College.CollegeName,
                                              DepartmentId = cD.ClassDetail.DepartmentId,
                                              IsPasswordProtected = cD.ClassDetail.IsPasswordProtected,
                                              FacultyId = cD.ClassDetail.ApplicationUser.ApplicationUserId,
                                              FacultyName = cD.ClassDetail.ApplicationUser.FirstName + " " + cD.ClassDetail.ApplicationUser.LastName,
                                              From = cD.ClassDetail.From,
                                              To = cD.ClassDetail.To,
                                              Day = cD.ClassDetail.Day.ToString(),
                                              Link = cD.ClassDetail.Link,
                                              ClassRoomNo = cD.ClassDetail.ClassRoomNo,
                                              BuildingNo = cD.ClassDetail.BuildingNo,
                                              ExamDate = cD.ClassDetail.ExamDate,
                                              ExamName = cD.ClassDetail.ExamName,
                                              Location = cD.ClassDetail.Location,
                                              Notes = cD.ClassDetail.Notes
                                          }).ToList();
                        if (lstClassDetail == null)
                        {
                            //_logger.Info("lstClassDetail.Count" + lstClassDetail.Count);
                            lstClassDetail = new List<ClassDetail_vm>();
                        }
                        else
                        {
                            //_logger.Info("lstClassDetail.Count is zero");
                        }
                        var dF = dbContext.DummyFaculties.Where(x => x.CreatedBy == currentUser.UserId
                            && x.StatusCode == StatusCodeConstants.ACTIVE
                            && x.TenantId == tenant.TenantId).ToList();
                        if (dF.HasValue())
                        {
                            foreach (var item in dF)
                            {
                                lstClassDetail.Add(new ClassDetail_vm
                                    {
                                        ClassName = item.ClassName,
                                        CollegeName = item.CollegeName,
                                        FacultyId = item.DummyFacultyId,
                                        FacultyName = item.FacultyName,
                                        From = item.From,
                                        To = item.To,
                                        Day = item.Day.ToString(),
                                        Link = item.Link,
                                        ClassRoomNo = item.ClassRoomNo,
                                        BuildingNo = item.BuildingNo,
                                        ExamDate = item.ExamDate,
                                        ExamName = item.ExamName,
                                        Location = item.Location,
                                        Notes = item.Notes
                                    });
                            }
                        }
                        return Serializer.ReturnContent(lstClassDetail, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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
                if (!currentUser.HasValue())
                {
                    currentUser = new CurrentUser { TenantId = tenant.TenantId };
                }
                ErrorLog.LogCustomError(currentUser, ex, apiDataLogId);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        public HttpResponseMessage Put(ApiViewModel apiViewModel)
        {
            //_logger.Info("DUmmyFaculty HttpPut - Called");
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
                            apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Put, "DummyFaculty", data);
                            ClassDetail_vm serializedClassDetail = JsonConvert
                                .DeserializeObject<ClassDetail_vm>(apiViewModel.custom.ToString());
                            if (serializedClassDetail != null)
                            {
                                dbContext = new UniversityContext();
                                DummyFaculty dummyFaculty = new DummyFaculty
                                {
                                    ClassName = serializedClassDetail.ClassName,
                                    CollegeName = serializedClassDetail.CollegeName,
                                    DepartmentName = serializedClassDetail.DepartmentName,
                                    FacultyName = serializedClassDetail.FacultyName,
                                    From = serializedClassDetail.From,
                                    To = serializedClassDetail.To,
                                    Day = serializedClassDetail.Day,
                                    Link = serializedClassDetail.Link,
                                    ClassRoomNo = serializedClassDetail.ClassRoomNo,
                                    BuildingNo = serializedClassDetail.BuildingNo,
                                    ExamDate = serializedClassDetail.ExamDate,
                                    ExamName = serializedClassDetail.ExamName,
                                    Notes = serializedClassDetail.Notes,
                                    Location = serializedClassDetail.Location,
                                    CreatedBy = currentUser.UserId,
                                    CreatedOn = DateTime.Now,
                                    TenantId = currentUser.TenantId,
                                    StatusCode = StatusCodeConstants.ACTIVE,
                                    Language = Language.English
                                };
                                dbContext.DummyFaculties.Add(dummyFaculty);
                                dbContext.SaveChanges();

                                _logger.Warn(HttpConstants.Inserted);
                                return Serializer.ReturnContent(HttpConstants.Inserted, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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

        public HttpResponseMessage Delete(ApiViewModel apiViewModel)
        {
            //_logger.Info("Dummy Faculty Delete - Called");
            CurrentUser currentUser = null;
            UniversityContext dbContext = null;
            int apiDataLogId = 0;
            Tenant tenant = null;
            string data = string.Empty;
            DummyFaculty cD = null;
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
                            apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Delete, "DummyFaculty", data);
                            DummyFaculty serializedDummyFaculty = JsonConvert
                                .DeserializeObject<DummyFaculty>(apiViewModel.custom.ToString());
                            if (serializedDummyFaculty != null)
                            {
                                dbContext = new UniversityContext();
                                cD = dbContext.DummyFaculties.SingleOrDefault(x => x.DummyFacultyId == serializedDummyFaculty.DummyFacultyId
                                    && x.StatusCode == StatusCodeConstants.ACTIVE
                                    && x.TenantId == tenant.TenantId && x.CreatedBy == currentUser.UserId
                                    );
                                if (cD != null)
                                {
                                    dbContext.DummyFaculties.Remove(cD);
                                    //cD.StatusCode = StatusCodeConstants.INACTIVE;
                                    //cD.LastModifiedBy = currentUser.UserId;
                                    //cD.LastModifiedOn = DateTime.Now;
                                    dbContext.SaveChanges();

                                    return Serializer.ReturnContent(HttpConstants.Deleted
                                    , this.Configuration.Services.GetContentNegotiator()
                                    , this.Configuration.Formatters, this.Request);
                                }
                                else
                                {
                                    _logger.Warn("DummyFaculty does not exists");
                                    return Serializer.ReturnContent("DummyFaculty does not exists", this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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
