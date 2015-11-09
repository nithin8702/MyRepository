using Newtonsoft.Json;
using PushSharp;
using PushSharp.Android;
using PushSharp.Apple;
using PushSharp.Core;
using System;
using System.IO;
using System.Linq;
using University.Api.Extensions;
using University.Common.Models.Enums;
using University.Common.Models.Security;
using University.Constants;
using University.Context;
using University.Security.Models.ViewModel;
using System.Collections;
using System.Collections.Generic;
using University.Security.Models;

namespace University.Api.Controllers.Log
{
    public class Notify : SecuredController
    {
        public static bool LogData(CurrentUser currentUser, UniversityContext dbContext, int userid, Module module,
            string msg, int? id = null, int? classId = null, string type = null, string CustomField01 = null)
        {
            bool result = false;
            int tmp = 0;
            try
            {
                if (currentUser.HasValue() && !string.IsNullOrEmpty(msg) && userid > 0)
                {
                    var settings = dbContext.Settings.SingleOrDefault(x => x.TenantId == currentUser.TenantId
                            && x.ApplicationUserId == userid && x.StatusCode == StatusCodeConstants.ACTIVE);
                    if (settings == null || settings.IsNotify == true
                        && (
                            (settings.IsBookCorner && module == Module.BookCorner)
                            || (settings.IsBroadcast && module == Module.BroadCast)
                            || (settings.IsMessage && module == Module.Message)
                            || (settings.IsTrafficNews && module == Module.TrafficNews)
                            || (settings.IsChat && module == Module.Chat)
                            )
                        )
                    {
                        IQueryable<ApplicationUser> query = dbContext.ApplicationUsers
                            .Where(x => x.ApplicationUserId == userid && x.StatusCode == StatusCodeConstants.ACTIVE);
                        if (currentUser.AccountType != AccountType.RootAdmin)
                        {
                            query.Where(x => x.TenantId == currentUser.TenantId);
                        }
                        var toUser = query.SingleOrDefault();
                        if (toUser != null)
                        {
                            University.Bussiness.Models.Notification notification = new University.Bussiness.Models.Notification
                            {
                                ApplicationUserId = userid,
                                Module = module,
                                Message = msg,
                                ClassDetailId = classId,
                                TenantId = currentUser.TenantId,
                                Type = type,
                                CustomField01 = CustomField01,
                                StatusCode = StatusCodeConstants.ACTIVE,
                                Language = Language.English,
                                CreatedBy = currentUser.UserId,
                                CreatedOn = DateTime.Now
                            };
                            if (id.HasValue)
                            {
                                notification.ID = id.Value;
                                tmp = id.Value;
                            }
                            dbContext.Notifications.Add(notification);
                            var sToken = dbContext.SecurityTokens.SingleOrDefault(x => x.StatusCode == StatusCodeConstants.ACTIVE
                            && x.TenantId == currentUser.TenantId && x.ApplicationUserId == userid);
                            if (sToken != null)
                            {
                                PushNotify(toUser, msg, type, tmp, classId);
                            }
                        }
                    }
                }
                else
                {
                    _logger.Warn("not a valid user");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
            return result;
        }

        private static void PushNotify(Security.Models.ApplicationUser user, string msg, string module = null,
            int? id = null, int? classId = null)
        {
            try
            {
                if (user != null)
                {
                    //_logger.Info("user.DeviceType : " + user.DeviceType);
                    if (!string.IsNullOrEmpty(user.DeviceType))
                    {
                        var push = new PushBroker();
                        push.OnNotificationSent += NotificationSent;
                        push.OnChannelException += ChannelException;
                        push.OnServiceException += ServiceException;
                        push.OnNotificationFailed += NotificationFailed;
                        push.OnDeviceSubscriptionExpired += DeviceSubscriptionExpired;
                        push.OnDeviceSubscriptionChanged += DeviceSubscriptionChanged;
                        push.OnChannelCreated += ChannelCreated;
                        push.OnChannelDestroyed += ChannelDestroyed;
                        string pushCertificatePath = System.Web.Hosting.HostingEnvironment.MapPath("/Resources/Devlopment.p12");//user.PushCertificatePath;
                        //_logger.Info("path : " + pushCertificatePath);
                        //_logger.Info("DeviceType : " + user.DeviceType.ToUpper());
                        string registrationId = user.RegistrationId;
                        switch (user.DeviceType.ToUpper())
                        {
                            case "APPLE":
                            case "IPHONE":
                            case "IPAD":
                            case "IOS":
                                var appleCert = File.ReadAllBytes(pushCertificatePath);
                                if (appleCert != null)
                                {
                                    //_logger.Info(appleCert.Length);
                                    //_logger.Info("not null : " + appleCert);
                                    string pushCertificatePassword = user.PushCertificatePassword;
                                    if (string.IsNullOrEmpty(pushCertificatePassword))
                                    {
                                        pushCertificatePassword = "";
                                    }
                                    push.RegisterAppleService(new ApplePushChannelSettings(appleCert, "welcome"));//Extension method
                                    //push.QueueNotification(new AppleNotification()
                                    //                   .ForDeviceToken(user.DeviceToken)
                                    //                   .WithAlert(msg)
                                    //                   .WithBadge(7)
                                    //                   .WithSound("sound.caf"));
                                    push.QueueNotification(new AppleNotification()
                                   .ForDeviceToken(registrationId)
                                   .WithAlert(msg)
                                   .WithBadge(7)
                                   .WithSound("sound.caf")
                                   .WithCustomItem("customeItem", module + "," + id + "," + classId));
                                }
                                else
                                {
                                    _logger.Info("applecert is null");
                                }

                                break;

                            case "ANDROID":

                                if (!string.IsNullOrEmpty(registrationId))
                                {
                                    Device_vm device = new Device_vm
                                    {
                                        alert = msg,
                                        badge = 7,
                                        sound = "sound.caf"
                                    };
                                    string deviceJson = JsonConvert.SerializeObject(device);
                                    push.RegisterGcmService(new GcmPushChannelSettings
                                        ("976594147484", "AIzaSyAmgKlb56LiLoyJdm93KMDX85tw6viUw1c",
                                        "com.ionicframework.uniteeky"));
                                    //Dictionary<string, string> dic = new Dictionary<string, string>();
                                    //dic.Add("message", msg);
                                    //dic.Add("badge", "7");
                                    //dic.Add("sound", "sound.caf");
                                    //if (id.HasValue && !string.IsNullOrEmpty(module))
                                    //{
                                    //    dic.Add(module, id.Value.ToString());
                                    //}
                                    push.QueueNotification(new GcmNotification().ForDeviceRegistrationId(registrationId)
                                        .WithCollapseKey("score_update")
                                          //.WithJson(deviceJson)
                                          //.WithJson("{\"message\":\"" + msg + "\",\"badge\":7,\"sound\":\"sound.caf\"}")
                                          //.WithData(dic)
                                          //.WithJson("{\"message\":\"" + module + "\",\"messagetype\":\"" + msgType + "\",\"id\":\"" + id + "\",\"message\":\"" + msg + "\",\"badge\":7,\"sound\":\"sound.caf\"}")
                                          .WithJson("{\"messagetype\":\"" + module + "\",\"id\":\"" + id + "\",\"message\":\"" + msg + "\",\"classId\":\"" + classId + "\",\"badge\":7,\"sound\":\"sound.caf\"}")
                                          .WithTimeToLive(108)
                                          );
                                }
                                break;
                            default:
                                break;
                        }
                        push.StopAllServices();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
        }

        static void DeviceSubscriptionChanged(object sender, string oldSubscriptionId, string newSubscriptionId, INotification notification)
        {
            //Currently this event will only ever happen for Android GCM
            //_logger.Info("Device Registration Changed:  Old-> " + oldSubscriptionId + "  New-> " + newSubscriptionId + " -> " + notification);
        }

        static void NotificationSent(object sender, INotification notification)
        {
            //_logger.Info("Sent: " + sender + " -> " + notification);
        }

        static void NotificationFailed(object sender, INotification notification, Exception notificationFailureException)
        {
            _logger.Info("Failure: " + sender + " -> " + notificationFailureException.Message + " -> " + notification);
        }

        static void ChannelException(object sender, IPushChannel channel, Exception exception)
        {
            _logger.Info("Channel Exception: " + sender + " -> " + exception);
        }

        static void ServiceException(object sender, Exception exception)
        {
            _logger.Info("Service Exception: " + sender + " -> " + exception);
        }

        static void DeviceSubscriptionExpired(object sender, string expiredDeviceSubscriptionId, DateTime timestamp, INotification notification)
        {
            _logger.Info("Device Subscription Expired: " + sender + " -> " + expiredDeviceSubscriptionId);
        }

        static void ChannelDestroyed(object sender)
        {
            //_logger.Info("Channel Destroyed for: " + sender);
        }

        static void ChannelCreated(object sender, IPushChannel pushChannel)
        {
            //_logger.Info("Channel Created for: " + sender);
        }
    }
}