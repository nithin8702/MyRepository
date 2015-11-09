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
    public class StudentSubscriptionController : UnSecuredController
    {
        public HttpResponseMessage Post(ApiViewModel apiViewModel)
        {
            //_logger.Info("Student Subscription HttpPost - Called");
            CurrentUser currentUser = null;
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
                            apiDataLogId = DataLog
                                .LogData(currentUser, VerbConstants.Post, "Student Subscription", data);
                            StudentSubscription_vm serializedSubscription = JsonConvert
                                .DeserializeObject<StudentSubscription_vm>(apiViewModel.custom.ToString());
                            if (serializedSubscription != null)
                            {
                                dbContext = new UniversityContext();
                                var studentSubs = dbContext.StudentSubscriptions
                                    .SingleOrDefault(x => x.ApplicationUserId == currentUser.UserId
                                    && x.ClassDetailId == serializedSubscription.ClassId
                                    && x.StatusCode == StatusCodeConstants.ACTIVE
                                    && x.TenantId == tenant.TenantId);
                                if (studentSubs == null)
                                {
                                    var cD = dbContext.ClassDetails.Include("ApplicationUser")
                                        .SingleOrDefault(x => x.ClassDetailId == serializedSubscription.ClassId
                                       && x.TenantId == tenant.TenantId && x.StatusCode == StatusCodeConstants.ACTIVE);
                                    if (cD != null)
                                    {
                                        if (cD.IsPasswordProtected)
                                        {
                                            byte[] strSalt = cD.PasswordSalt;
                                            string salt = Convert.ToBase64String(strSalt);
                                            byte[] dbPasswordHash = cD.PasswordHash;
                                            byte[] userPasswordHash = Encryptor.GenerateHash(serializedSubscription.Password, salt);
                                            bool chkPassword = Encryptor.CompareByteArray(dbPasswordHash, userPasswordHash);
                                            if (!chkPassword)
                                            {
                                                _logger.Warn(HttpConstants.PasswordMismatch);
                                                return Serializer.ReturnContent(HttpConstants.PasswordMismatch, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                                            }
                                        }
                                        StudentSubscription subscription = new StudentSubscription
                                        {
                                            ClassDetailId = cD.ClassDetailId,
                                            ApplicationUserId = currentUser.UserId,
                                            TenantId = tenant.TenantId,
                                            StatusCode = StatusCodeConstants.ACTIVE,
                                            Language = Language.English,
                                            CreatedBy = currentUser.UserId,
                                            CreatedOn = DateTime.Now
                                        };
                                        dbContext.StudentSubscriptions.Add(subscription);
                                        dbContext.SaveChanges();

                                        Notify.LogData(currentUser, dbContext, cD.ApplicationUser.ApplicationUserId, Module.Message,
                                                        currentUser.FullName + " has subscribed you for Class - " + cD.ClassName, subscription.StudentSubscriptionId, null, "Subscription");

                                        dbContext.SaveChanges();
                                        return Serializer.ReturnContent("Subscribed Successfully.", this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                                    }
                                    else
                                    {
                                        _logger.Warn("Class does not exists");
                                        return Serializer.ReturnContent("Class does not exists", this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                                    }
                                }
                                else
                                {
                                    if (studentSubs.StatusCode != StatusCodeConstants.ACTIVE)
                                    {
                                        studentSubs.StatusCode = StatusCodeConstants.ACTIVE;
                                        dbContext.SaveChanges();
                                        _logger.Warn(HttpConstants.Updated);
                                        return Serializer.ReturnContent(HttpConstants.Updated, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                                    }
                                    else
                                    {
                                        _logger.Warn("user already subscribed");
                                        return Serializer.ReturnContent("user already subscribed", this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                                    }
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

        public HttpResponseMessage Delete(ApiViewModel apiViewModel)
        {
            //_logger.Info("Student Subscription Delete - Called");
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
                            apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Delete, "Student Subscription", data);
                            StudentBroadcast_vm serializedStudentBroadcast = JsonConvert
                                .DeserializeObject<StudentBroadcast_vm>(apiViewModel.custom.ToString());
                            if (serializedStudentBroadcast != null)
                            {
                                dbContext = new UniversityContext();
                                var map = dbContext.StudentSubscriptions.Include("ClassDetail")
                                    .SingleOrDefault(x => x.StudentSubscriptionId == serializedStudentBroadcast.BroadcastMapId
                                   && x.TenantId == tenant.TenantId && x.StatusCode == StatusCodeConstants.ACTIVE);
                                int clsid = 0;
                                if (map != null)
                                {
                                    //map.StatusCode = StatusCodeConstants.INACTIVE;
                                    //dbContext.Entry<StudentSubscription>(map).State = EntityState.Deleted;
                                    clsid = map.ClassDetailId.Value;
                                    Notify.LogData(currentUser, dbContext, map.ApplicationUserId, Module.Message,
                                                        currentUser.FullName + " has Unsubscribed you for Class - " + map.ClassDetail.ClassName, map.StudentSubscriptionId);
                                    dbContext.StudentSubscriptions.Remove(map);
                                    var tmpn = dbContext.Notifications.Where(x => x.TenantId == tenant.TenantId
                                      && x.ClassDetailId == clsid && x.ApplicationUserId == currentUser.UserId).ToList();
                                    dbContext.Notifications.RemoveRange(tmpn);
                                    dbContext.SaveChanges();
                                    return Serializer.ReturnContent("UnSubscribed Successfully.", this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                                }
                                else
                                {
                                    _logger.Warn("Class does not exists");
                                    return Serializer.ReturnContent("Class does not exists", this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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
