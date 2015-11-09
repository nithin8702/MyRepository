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
using University.Api.Utilities;
using University.Bussiness.Models;
using University.Bussiness.Models.ViewModel;
using University.Common.Models;
using University.Common.Models.Enums;
using University.Common.Models.Security;
using University.Constants;
using University.Context;
using University.Security.Models;
using University.Security.Models.ViewModel;
using University.Utilities;

namespace University.Api.Controllers
{
    public class ComposeMessageController : UnSecuredController
    {
        public HttpResponseMessage Get([FromUri]int applicationUserId)
        {
            //_logger.Info("ComposeMessage HttpGet - Called");
            UniversityContext dbcontext = null;
            List<ViewMessage_vm> lstViewMessage = null;
            Tenant tenant = null;
            int apiDataLogId = 0;
            CurrentUser currentUser = null;
            try
            {
                tenant = CurrentTenant;
                if (tenant.HasValue())
                {
                    dbcontext = new UniversityContext();
                    var a = dbContext.ApplicationUsers;
                    var b = a.Where(x => x.ApplicationUserId == 1);
                    lstViewMessage = (from cB in dbContext.ComposeMessages
                                   .Where(x =>
                                       x.ToUserId == applicationUserId || x.FromUserId == applicationUserId
                                       && x.StatusCode == StatusCodeConstants.ACTIVE
                                       && x.TenantId == tenant.TenantId)
                                      join touser in dbContext.ApplicationUsers on cB.ToUserId equals touser.ApplicationUserId
                                      join fromuser in dbContext.ApplicationUsers on cB.FromUserId equals fromuser.ApplicationUserId
                                      //new { cB.FromUserId, cB.ToUserId } equals new { user.ApplicationUserId,user.ApplicationUserId }
                                      select new ViewMessage_vm
                                      {
                                          MessageId = cB.ComposeMessageId,
                                          ToUserId = touser.ApplicationUserId,
                                          ToUserName = touser.FirstName + " " + touser.LastName,
                                          ToFirstName = touser.FirstName,
                                          ToLastName = touser.LastName,
                                          ToEmailAddress = touser.EmailAddress,
                                          ToContact = touser.Contact,

                                          FromUserId = fromuser.ApplicationUserId,
                                          FromUserName = fromuser.FirstName + " " + fromuser.LastName,
                                          FromFirstName = fromuser.FirstName,
                                          FromLastName = fromuser.LastName,
                                          FromEmailAddress = fromuser.EmailAddress,
                                          FromContact = fromuser.Contact,

                                          Subject = cB.Subject,
                                          Body = cB.Body,
                                          Path1 = cB.Path1,
                                          Path2 = cB.Path2,
                                          Path3 = cB.Path3,
                                          Path4 = cB.Path4,
                                          PostedDate = cB.CreatedOn.ToString(),
                                          //DaysAgo = DbFunctions.DiffDays(cB.CreatedOn, DateTime.Today).Value,
                                      }).OrderByDescending(x => x.PostedDate).ToList();
                    if (lstViewMessage.HasValue())
                    {
                        lstViewMessage.ForEach(x => Separate(x));
                    }
                    return Serializer.ReturnContent(lstViewMessage, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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

        private object Separate(ViewMessage_vm x)
        {
            if (x != null)
            {
                x.Path1s = x.Path1.SplitIntoStringList(TechConstants.Separator);
                x.Path2s = x.Path2.SplitIntoStringList(TechConstants.Separator);
                x.Path3s = x.Path3.SplitIntoStringList(TechConstants.Separator);
                x.Path4s = x.Path4.SplitIntoStringList(TechConstants.Separator);
                x.DaysAgo = TimeSpanFormat.FormatDaysAgo(x.PostedDate);
            }
            return x;
        }

        public HttpResponseMessage Post(ApiViewModel apiViewModel)
        {
            //_logger.Info("ComposeMessage HttpPost - Called");
            CurrentUser currentUser = null;
            UniversityContext dbContext = null;
            int apiDataLogId = 0;
            Tenant tenant = null;
            string data = string.Empty;
            IQueryable<ApplicationUser> queryableUser = null;
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
                            apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Post, "ComposeMessage", data);
                            ComposeMessage_vm serializedComposeMessage = JsonConvert
                                .DeserializeObject<ComposeMessage_vm>(apiViewModel.custom.ToString());
                            if (serializedComposeMessage != null)
                            {
                                dbContext = new UniversityContext();
                                List<int?> lst = new List<int?>();
                                if (currentUser.AccountType != AccountType.Faculty)
                                {
                                    //_logger.Warn("User Restricted.");
                                    lst = dbContext.RestrictedUsers.Where(x => x.ApplicationUserId == currentUser.UserId
                                         && (x.Module == Module.Message || x.Module == Module.All)).Select(x => x.CreatedBy).ToList();
                                    //return Serializer.ReturnContent("User Restricted.", this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                                }
                                if (serializedComposeMessage.ClassIds.HasValue())
                                {
                                    queryableUser = dbContext.ApplicationUsers.Where(x => x.TenantId == tenant.TenantId
                                    && x.StatusCode == StatusCodeConstants.ACTIVE);
                                    if (lst.HasValue())
                                    {
                                        queryableUser = queryableUser.Where(y => !lst.Contains(y.ApplicationUserId));
                                    }
                                    //var clsUsers = dbContext.ClassDetails.Where(x => x.TenantId == tenant.TenantId && x.StatusCode == StatusCodeConstants.ACTIVE
                                    //    && serializedComposeMessage.ClassIds.Contains(x.ClassDetailId)).Select(x => x.ApplicationUserId).ToList();
                                    //if (clsUsers.HasValue())
                                    //{
                                    //    queryableUser = queryableUser.Where(x => clsUsers.Contains(x.ApplicationUserId));
                                    //}
                                    if (currentUser.AccountType == AccountType.Faculty)
                                    {
                                        var students = dbContext.StudentSubscriptions.Where(x => x.TenantId == tenant.TenantId
                                            && x.StatusCode == StatusCodeConstants.ACTIVE
                                            && serializedComposeMessage.ClassIds.Contains(x.ClassDetailId.Value))
                                            .Select(x => x.ApplicationUserId).ToList();
                                        if (students.HasValue())
                                        {
                                            queryableUser = queryableUser.Where(x => students.Contains(x.ApplicationUserId));
                                        }
                                    }
                                    var ids = queryableUser.Select(x => x.ApplicationUserId).ToList();
                                    if (ids.HasValue())
                                    {
                                        foreach (var id in ids)
                                        {
                                            ComposeMessage studentcM = new ComposeMessage
                                            {
                                                FromUserId = currentUser.UserId,
                                                ToUserId = id,
                                                Subject = serializedComposeMessage.Subject,
                                                Body = serializedComposeMessage.Body,

                                                Path1 = serializedComposeMessage.Path1,
                                                Path2 = serializedComposeMessage.Path2,
                                                Path3 = serializedComposeMessage.Path3,
                                                Path4 = serializedComposeMessage.Path4,

                                                CreatedBy = currentUser.UserId,
                                                CreatedOn = DateTime.Now,
                                                TenantId = currentUser.TenantId,
                                                StatusCode = StatusCodeConstants.ACTIVE,
                                                Language = Language.English
                                            };
                                            dbContext.ComposeMessages.Add(studentcM);
                                            dbContext.SaveChanges();
                                            Notify.LogData(currentUser, dbContext, id, Module.Message,
                                    currentUser.FullName + " has posted new message.", studentcM.ComposeMessageId, null, "ComposeMessage");
                                            dbContext.SaveChanges();
                                        }
                                    }

                                }
                                if (serializedComposeMessage.ToUserId.HasValue())
                                {
                                    List<string> lststring = new List<string>();
                                    foreach (var item in serializedComposeMessage.ToUserId)
                                    {
                                        if (dbContext.RestrictedUsers.Count(x => x.ApplicationUserId == currentUser.UserId
                                      && (x.Module == Module.Message || x.Module == Module.All) && x.CreatedBy == item) > 0)
                                        {
                                            if (currentUser.AccountType == AccountType.Student)
                                            {
                                                return Serializer.ReturnContent("user restricted."
                                    , this.Configuration.Services.GetContentNegotiator()
                                    , this.Configuration.Formatters, this.Request);
                                            }
                                        }
                                        ComposeMessage cM = new ComposeMessage
                                        {
                                            FromUserId = currentUser.UserId,
                                            ToUserId = item,
                                            Subject = serializedComposeMessage.Subject,
                                            Body = serializedComposeMessage.Body,

                                            Path1 = serializedComposeMessage.Path1,
                                            Path2 = serializedComposeMessage.Path2,
                                            Path3 = serializedComposeMessage.Path3,
                                            Path4 = serializedComposeMessage.Path4,


                                            CreatedBy = currentUser.UserId,
                                            CreatedOn = DateTime.Now,
                                            TenantId = currentUser.TenantId,
                                            StatusCode = StatusCodeConstants.ACTIVE,
                                            Language = Language.English
                                        };
                                        dbContext.ComposeMessages.Add(cM);
                                        dbContext.SaveChanges();

                                        Notify.LogData(currentUser, dbContext, item, Module.Message,
                                currentUser.FullName + " has posted new message.", cM.ComposeMessageId, null, "ComposeMessage");
                                        dbContext.SaveChanges();
                                    }
                                }
                                dbContext.SaveChanges();
                                return Serializer.ReturnContent("Message Sent."
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

        public HttpResponseMessage Delete(ApiViewModel apiViewModel)
        {
            //_logger.Info("ComposeMessage Delete - Called");
            CurrentUser currentUser = null;
            UniversityContext dbContext = null;
            int apiDataLogId = 0;
            Tenant tenant = null;
            string data = string.Empty;
            ComposeMessage cD = null;
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
                            apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Delete, "ComposeMessage", data);
                            ComposeMessage serializedComposeMessage = JsonConvert
                                .DeserializeObject<ComposeMessage>(apiViewModel.custom.ToString());
                            if (serializedComposeMessage != null)
                            {
                                dbContext = new UniversityContext();
                                cD = dbContext.ComposeMessages.SingleOrDefault(x => x.ComposeMessageId == serializedComposeMessage.ComposeMessageId
                                    && x.StatusCode == StatusCodeConstants.ACTIVE && x.TenantId == tenant.TenantId);
                                if (cD != null)
                                {
                                    cD.StatusCode = StatusCodeConstants.INACTIVE;
                                    cD.LastModifiedBy = currentUser.UserId;
                                    cD.LastModifiedOn = DateTime.Now;
                                    dbContext.SaveChanges();

                                    return Serializer.ReturnContent("Message Deleted."
                                    , this.Configuration.Services.GetContentNegotiator()
                                    , this.Configuration.Formatters, this.Request);
                                }
                                else
                                {
                                    _logger.Warn("Message does not exists");
                                    return Serializer.ReturnContent("Message does not exists", this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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
