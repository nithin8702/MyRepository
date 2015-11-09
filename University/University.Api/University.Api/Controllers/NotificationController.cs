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
    public class NotificationController : UnSecuredController
    {
        public HttpResponseMessage Post(ApiViewModel apiViewModel)
        {
            //_logger.Info("Notification HttpPost - Called");
            CurrentUser currentUser = null;
            int apiDataLogId = 0;
            Tenant tenant = null;
            string data = string.Empty;
            UniversityContext dbContext = null;
            List<Notification_vm> lstNotification = null;
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
                        var settings = dbContext.Settings.SingleOrDefault(x => x.TenantId == tenant.TenantId
                            && x.ApplicationUserId == currentUser.UserId && x.StatusCode == StatusCodeConstants.ACTIVE);
                        if (settings != null && settings.IsNotify)
                        {
                            lstNotification = (from noti in dbContext.Notifications.Include("ClassDetail")
                                               .Include("ApplicationUser").Include("BookCorner").Include("TrafficNews")
                                               .Where(x => x.CreatedBy != currentUser.UserId && x.ApplicationUserId == currentUser.UserId
                                                   && x.ApplicationUser.StatusCode == StatusCodeConstants.ACTIVE
                                                   &&
                                                   (
                                                       (x.Module == Module.BookCorner && settings.IsBookCorner == true)
                                                       || (x.Module == Module.BroadCast && settings.IsBroadcast == true)
                                                       || (x.Module == Module.Message && settings.IsMessage == true)
                                                       || (x.Module == Module.TrafficNews && settings.IsTrafficNews == true)
                                                   )
                                                   && (x.StatusCode == StatusCodeConstants.ACTIVE || x.StatusCode == StatusCodeConstants.NEW)
                                                   && x.TenantId == tenant.TenantId)
                                               select new Notification_vm
                                               {
                                                   NotificationId = noti.NotificationId,
                                                   Message = noti.Message,
                                                   Module = noti.Module.ToString(),
                                                   PostedDate = noti.CreatedOn.ToString(),
                                                   ClassDetailId = noti.ClassDetailId,
                                                   BookCornerId = noti.BookCornerId,
                                                   TrafficNewsId = noti.TrafficNewsId,
                                                   Id = noti.ID,
                                                   ClassDetail = noti.ClassDetail,
                                                   BookCorner = noti.BookCorner,
                                                   TrafficNews = noti.TrafficNews,
                                                   Type = noti.Type,
                                                   CustomField01 = noti.CustomField01,
                                                   //DaysAgo = DbFunctions.DiffDays(noti.CreatedOn, DateTime.Today).Value,
                                               })
                                               .OrderByDescending(x => x.PostedDate).ToList();
                            if (lstNotification.HasValue())
                            {
                                foreach (var item in lstNotification)
                                {
                                    TimeSpan span = (DateTime.Now - Convert.ToDateTime(item.PostedDate));
                                    item.DaysAgo = String.Format("{0} days {1} hours {2} minutes",
                                                span.Days, span.Hours, span.Minutes);
                                    var notification = dbContext.Notifications.SingleOrDefault(x => x.NotificationId == item.NotificationId && x.TenantId == tenant.TenantId);
                                    if (notification != null)
                                    {
                                        notification.StatusCode = StatusCodeConstants.NEW;
                                    }
                                }
                                dbContext.SaveChanges();
                            }
                        }
                        //var dat = lstNotification.GroupBy(x => x.ClassDetailId).ToList();
                        return Serializer.ReturnContent(lstNotification.GroupBy(x => x.Module).ToList(), this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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

        public HttpResponseMessage Put(ApiViewModel apiViewModel)
        {
            //_logger.Info("Notification HttpPut - Called");
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
                                .LogData(currentUser, VerbConstants.Put, "Notification", data);
                            List<int> lstNotification = JsonConvert
                                .DeserializeObject<List<int>>(apiViewModel.custom.ToString());
                            if (lstNotification.HasValue())
                            {
                                dbContext = new UniversityContext();
                                foreach (var notificationId in lstNotification)
                                {
                                    if (notificationId > 0)
                                    {
                                        var notification = dbContext.Notifications.SingleOrDefault(x => x.NotificationId == notificationId
                                            && x.TenantId == tenant.TenantId && x.StatusCode == StatusCodeConstants.NEW);
                                        if (notification != null)
                                        {
                                            notification.StatusCode = StatusCodeConstants.INACTIVE;
                                            notification.LastModifiedBy = currentUser.UserId;
                                            notification.LastModifiedOn = DateTime.Now;
                                        }
                                        dbContext.Entry<Notification>(notification).State = EntityState.Modified;
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
