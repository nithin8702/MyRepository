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
using University.Api.Utilities;
using University.Common.Models;
using University.Common.Models.Enums;
using University.Common.Models.Security;
using University.Common.Models.ViewModel;
using University.Constants;
using University.Context;
using University.Security.Models;
using University.Security.Models.ViewModel;
using University.Utilities;

namespace University.Api.Controllers
{
    public class ChatController : UnSecuredController
    {

        public HttpResponseMessage Get([FromUri]UserChat_vm userChat_vm)
        {
            //_logger.Info("UserChat HttpGet - Called");
            UniversityContext dbContext = null;
            Tenant tenant = null;
            int apiDataLogId = 0;
            CurrentUser currentUser = null;
            List<UserChat_vm> lstUserChat = null;
            try
            {
                tenant = CurrentTenant;
                if (tenant.HasValue())
                {
                    dbContext = new UniversityContext();
                    lstUserChat = (from uChat in dbContext.UserChats.Include("FromApplicationUser").Include("ToApplicationUser")
                                            .Where(x => x.StatusCode == StatusCodeConstants.ACTIVE)
                                   join
                                   frm in dbContext.ApplicationUsers.Where(x => x.StatusCode == StatusCodeConstants.ACTIVE)
                                   on uChat.FromApplicationUserId equals frm.ApplicationUserId
                                   join
                                   to in dbContext.ApplicationUsers.Where(x => x.StatusCode == StatusCodeConstants.ACTIVE)
                                   on uChat.ToApplicationUserId equals to.ApplicationUserId
                                   select new UserChat_vm
                                   {
                                       UserChatId = uChat.UserChatId,
                                       FromUserId = uChat.FromApplicationUserId,
                                       FromUserName = frm.FirstName + " " + frm.LastName,
                                       FromEmailAddress = frm.EmailAddress,
                                       FromContact = frm.Contact,

                                       ToUserId = uChat.ToApplicationUserId,
                                       ToUserName = to.FirstName + " " + to.LastName,
                                       ToEmailAddress = to.EmailAddress,
                                       ToContact = to.Contact,

                                       PostedDate = uChat.CreatedOn.ToString(),
                                       Message = uChat.Message,
                                   }).OrderByDescending(x => x.PostedDate)
                                             .ToList();
                    if (lstUserChat.HasValue())
                    {
                        lstUserChat.ForEach(x => Separate(x));
                    }
                    return Serializer.ReturnContent(lstUserChat, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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

        private object Separate(UserChat_vm x)
        {
            if (x != null)
            {
                x.DaysAgo = TimeSpanFormat.FormatDaysAgo(x.PostedDate);
            }
            return x;
        }


        public HttpResponseMessage Post(ApiViewModel apiViewModel)
        {
            //_logger.Info("UserChat HttpPost - Called");
            CurrentUser currentUser = null;
            UniversityContext dbContext = null;
            int apiDataLogId = 0;
            Tenant tenant = null;
            string data = string.Empty;
            UserChat userChat = null;
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
                            apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Post, "UserChat", data);
                            UserChat_vm serializedUserChat_vm = JsonConvert
                                .DeserializeObject<UserChat_vm>(apiViewModel.custom.ToString());
                            if (serializedUserChat_vm != null)
                            {
                                dbContext = new UniversityContext();
                                userChat = new UserChat
                                {
                                    FromApplicationUserId = serializedUserChat_vm.FromUserId,
                                    ToApplicationUserId = serializedUserChat_vm.ToUserId,
                                    Message = serializedUserChat_vm.Message,
                                    CreatedBy = currentUser.UserId,
                                    CreatedOn = DateTime.Now,
                                    TenantId = tenant.TenantId,
                                    StatusCode = StatusCodeConstants.ACTIVE,
                                    Language = Language.English
                                };
                                dbContext.UserChats.Add(userChat);
                                dbContext.SaveChanges();
                                return Serializer.ReturnContent(HttpConstants.Inserted
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
