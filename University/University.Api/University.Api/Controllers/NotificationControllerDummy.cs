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
    public class NotificationDummyController : UnSecuredController
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

                        lstNotification = (from noti in dbContext.Notifications
                                           .Include("ApplicationUser")
                                           .Where(x => x.CreatedBy != currentUser.UserId && x.ApplicationUserId == currentUser.UserId &&
                                               x.ApplicationUser.StatusCode == StatusCodeConstants.ACTIVE
                                               && (x.StatusCode == StatusCodeConstants.ACTIVE)
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



    }
}
