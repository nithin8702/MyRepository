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
using University.Bussiness.Models;
using University.Common.Models;
using University.Common.Models.Enums;
using University.Common.Models.Security;
using University.Constants;
using University.Context;
using University.Security.Models.ViewModel;
using University.Utilities;

namespace University.Api.Controllers
{
    public class StudentViewController : UnSecuredController
    {
        public HttpResponseMessage Get([FromUri]int classId)
        {
            //_logger.Info("StudentView HttpGet - Called");
            int apiDataLogId = 0;
            UniversityContext dbcontext = new UniversityContext();
            List<StudentView> lstStudentView = null;
            CurrentUser currentUser = null;
            Tenant tenant = null;
            try
            {
                tenant = CurrentTenant;
                if (tenant.HasValue())
                {
                    lstStudentView = dbcontext.StudentViews.Include("ClassDetail")
                            .Where(x => x.ClassDetailId == classId && x.StatusCode == StatusCodeConstants.ACTIVE
                                && x.TenantId == tenant.TenantId && x.ClassDetail.StatusCode==StatusCodeConstants.ACTIVE
                                ).ToList();
                    //_logger.Info("lstStudentView count : " + lstStudentView.Count);
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
            return Serializer.ReturnContent(lstStudentView, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
        }

        public HttpResponseMessage Post(ApiViewModel apiViewModel)
        {
            //_logger.Info("StudentView HttpPost - Called");
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
                            apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Post, "StudentView", data);
                            if (currentUser.AccountType == AccountType.Faculty)
                            {
                                List<StudentView> lstSerializedStudentView = JsonConvert
                                    .DeserializeObject<List<StudentView>>(apiViewModel.custom.ToString());
                                if (lstSerializedStudentView.HasValue())
                                {
                                    dbContext = new UniversityContext();
                                    foreach (var item in lstSerializedStudentView)
                                    {
                                        var studentView = new StudentView
                                        {
                                            //ApplicationUserId = item.ApplicationUserId,
                                            ClassDetailId = item.ClassDetailId,
                                            CanViewMark = item.CanViewMark,
                                            CanViewOtherStudentMark = item.CanViewOtherStudentMark,
                                            CreatedBy = currentUser.UserId,
                                            CreatedOn = DateTime.Now,
                                            TenantId = tenant.TenantId,
                                            StatusCode = StatusCodeConstants.ACTIVE,
                                            Language = Language.English
                                        };
                                        dbContext.StudentViews.Add(studentView);
                                    }
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
                                _logger.Warn(HttpConstants.InvalidFaculty);
                                return Serializer.ReturnContent(HttpConstants.InvalidFaculty, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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
            //_logger.Info("StudentView HttpPut - Called");
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
                            apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Put, "StudentView", data);
                            if (currentUser.AccountType == AccountType.Faculty)
                            {
                                List<StudentView> lstSerializedStudentView = JsonConvert
                                    .DeserializeObject<List<StudentView>>(apiViewModel.custom.ToString());
                                if (lstSerializedStudentView.HasValue())
                                {
                                    dbContext = new UniversityContext();
                                    foreach (var item in lstSerializedStudentView)
                                    {
                                        var studentView = dbContext.StudentViews.SingleOrDefault(x => x.StudentViewId == item.StudentViewId
                                            && x.TenantId == tenant.TenantId && x.StatusCode == StatusCodeConstants.ACTIVE);
                                        if (studentView != null)
                                        {
                                            studentView.CanViewMark = item.CanViewMark;
                                            studentView.CanViewOtherStudentMark = item.CanViewOtherStudentMark;
                                            studentView.LastModifiedBy = currentUser.UserId;
                                            studentView.LastModifiedOn = DateTime.Now;
                                            dbContext.Entry<StudentView>(studentView).State = EntityState.Modified;
                                        }
                                        else
                                        {
                                            _logger.Warn(HttpConstants.InvalidInput);
                                            return Serializer.ReturnContent(HttpConstants.InvalidInput, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                                        }
                                    }
                                    dbContext.SaveChanges();
                                    return Serializer.ReturnContent(HttpConstants.Updated
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
                                _logger.Warn(HttpConstants.InvalidFaculty);
                                return Serializer.ReturnContent(HttpConstants.InvalidFaculty, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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
