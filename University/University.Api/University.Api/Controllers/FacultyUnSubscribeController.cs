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
    public class FacultyUnSubscribeController : UnSecuredController
    {
        public HttpResponseMessage Delete(ApiViewModel apiViewModel)
        {
            //_logger.Info("Faculty Student UnSubscription Delete - Called");
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
                            apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Delete, "FacultyUnSubscribe", data);
                            if (currentUser.AccountType != AccountType.Faculty)
                            {
                                return Serializer.ReturnContent(HttpConstants.InvalidFaculty, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                            }
                            StudentSubscription_vm serializedStudentSubscription = JsonConvert
                                .DeserializeObject<StudentSubscription_vm>(apiViewModel.custom.ToString());
                            if (serializedStudentSubscription != null)
                            {
                                dbContext = new UniversityContext();
                                var map = dbContext.StudentSubscriptions.Include("ClassDetail")
                                    .SingleOrDefault(x => x.ApplicationUserId == serializedStudentSubscription.ApplicationUserId
                                    && x.ClassDetailId == serializedStudentSubscription.ClassId
                                    && x.TenantId == tenant.TenantId);
                                if (map != null)
                                {
                                    //map.StatusCode = StatusCodeConstants.INACTIVE;
                                    //dbContext.Entry<StudentSubscription>(map).State = EntityState.Modified;

                                    Notify.LogData(currentUser, dbContext, map.ApplicationUserId, Module.Message,
                                                        currentUser.FullName + " has Unsubscribed you for Class - " + map.ClassDetail.ClassName,map.StudentSubscriptionId);
                                    dbContext.StudentSubscriptions.Remove(map);
                                    dbContext.SaveChanges();
                                    return Serializer.ReturnContent(HttpConstants.Deleted, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                                }
                                else
                                {
                                    _logger.Warn("Student UnSubscription does not exists");
                                    return Serializer.ReturnContent("Student UnSubscription does not exists", this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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
    }
}
