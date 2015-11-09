using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
using System.Linq;
using System.Data.Entity;
using University.Utilities;
using University.Api.Utilities;

namespace University.Api.Controllers
{
    public class BroadCastMessageController : UnSecuredController
    {

        public HttpResponseMessage Get([FromUri]int broadCastId)
        {
            //_logger.Info("BroadCastMessage HttpGet - Called");
            UniversityContext dbcontext = null;
            List<BroadCastMessage_vm> lstBroadCastMessage_vm = null;
            Tenant tenant = null;
            int apiDataLogId = 0;
            CurrentUser currentUser = null;
            try
            {
                tenant = CurrentTenant;
                if (tenant.HasValue())
                {
                    dbcontext = new UniversityContext();
                    lstBroadCastMessage_vm = (from msg in dbcontext.BroadCastMessages.Include("BroadCast").Where(x => x.BroadCastId == broadCastId &&
                        x.TenantId == tenant.TenantId && x.StatusCode == StatusCodeConstants.ACTIVE)
                                              select new BroadCastMessage_vm
                                              {
                                                  BroadCastMessageId = msg.BroadCastMessageId,
                                                  BroadCastIdId = msg.BroadCastId.Value,
                                                  classId = msg.BroadCast.ClassDetailId.Value,
                                                  Message = msg.Message,
                                                  UserName = msg.ApplicationUser.FirstName + " " + msg.ApplicationUser.LastName,
                                                  PostedDate = msg.CreatedOn.ToString(),
                                                  //DaysAgo = DbFunctions.DiffDays(msg.CreatedOn, DateTime.Today).Value,
                                              }).OrderByDescending(x => x.PostedDate).ToList();
                    if (lstBroadCastMessage_vm.HasValue())
                    {
                        lstBroadCastMessage_vm.ForEach(x => Separate(x));
                    }
                    return Serializer.ReturnContent(lstBroadCastMessage_vm, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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

        private object Separate(BroadCastMessage_vm x)
        {
            if (x != null)
            {
                x.DaysAgo = TimeSpanFormat.FormatDaysAgo(x.PostedDate);
            }
            return x;

        }

        public HttpResponseMessage Post(ApiViewModel apiViewModel)
        {
            //_logger.Info("BroadCastMessage HttpPost - Called");
            CurrentUser currentUser = null;
            int apiDataLogId = 0;
            Tenant tenant = null;
            string data = string.Empty;
            UniversityContext dbContext = null;
            List<int> lstUserId = new List<int>();
            IQueryable<StudentSubscription> queryableUser = null;
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
                            apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Post, "BroadCast Message", data);
                            BroadCastMessage_vm serializedMessage = JsonConvert
                                .DeserializeObject<BroadCastMessage_vm>(apiViewModel.custom.ToString());
                            if (serializedMessage != null)
                            {
                                dbContext = new UniversityContext();
                                BroadCast bC = dbContext.BroadCasts.Include("RestrictedUsers").SingleOrDefault(x => x.BroadCastId == serializedMessage.BroadCastIdId
                                    && x.StatusCode == StatusCodeConstants.ACTIVE);
                                if (bC != null)
                                {
                                    queryableUser = dbContext.StudentSubscriptions.Where(x => x.ClassDetailId == bC.ClassDetailId
                                            && x.StatusCode == StatusCodeConstants.ACTIVE);
                                    int tmp = dbContext.RestrictedUsers.Count(x => x.ApplicationUserId == currentUser.UserId
                                    && x.ClassDetailId == bC.ClassDetailId && x.Module == Module.BroadCast && x.TenantId == tenant.TenantId && x.StatusCode == StatusCodeConstants.ACTIVE);
                                    if (bC.RestrictedUsers.HasValue() && bC.RestrictedUsers
                                        .Count(x => x.ApplicationUserId == currentUser.UserId) > 0 || tmp > 0)
                                    {
                                        List<int> res = bC.RestrictedUsers.Select(x => x.ApplicationUserId).ToList();
                                        queryableUser = queryableUser.Where(x => !res.Contains(x.ApplicationUserId));

                                        _logger.Warn("You are not allowed to reply by faculty order.");
                                        return Serializer.ReturnContent("You are not allowed to reply by faculty order.", this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                                    }
                                    var students = queryableUser.Select(x => x.ApplicationUserId).ToList();
                                    lstUserId = students;
                                    BroadCastMessage bM = new BroadCastMessage
                                    {
                                        BroadCastId = serializedMessage.BroadCastIdId,
                                        Message = serializedMessage.Message,
                                        StatusCode = StatusCodeConstants.ACTIVE,
                                        TenantId = tenant.TenantId,
                                        ApplicationUserId = currentUser.UserId,
                                        CreatedBy = currentUser.UserId,
                                        CreatedOn = DateTime.Now,
                                        Language = Language.English
                                    };
                                    dbContext.BroadCastMessages.Add(bM);
                                    dbContext.SaveChanges();
                                    if (lstUserId.HasValue())
                                    {
                                        //_logger.Info("BroadCast cu userid." + currentUser.UserId);
                                        foreach (var item in lstUserId)
                                        {
                                            //_logger.Info("BroadCast userid." + item);
                                            if (currentUser.UserId != item && item > 0)
                                            {
                                                Notify.LogData(currentUser, dbContext, item, Module.Message,
                                                currentUser.FullName + " has posted message ",
                                                bM.BroadCastMessageId, bC.ClassDetailId,
                                                "BroadCastMessage", bM.BroadCastId.ToString());
                                            }
                                        }
                                        //dbContext.SaveChanges();
                                    }
                                    Notify.LogData(currentUser, dbContext, bC.ApplicationUserId.Value, Module.Message,
                                                currentUser.FullName + " has posted message ",
                                                bM.BroadCastMessageId, bC.ClassDetailId,
                                                "BroadCastMessage", bM.BroadCastId.ToString());
                                    dbContext.SaveChanges();
                                    return Serializer.ReturnContent("Broadcast Message Sent.", this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                                }
                                else
                                {
                                    _logger.Warn("BroadCast not found.");
                                    return Serializer.ReturnContent("BroadCast not found.", this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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
            //_logger.Info("BroadCastMessage Delete - Called");
            CurrentUser currentUser = null;
            UniversityContext dbContext = null;
            int apiDataLogId = 0;
            Tenant tenant = null;
            string data = string.Empty;
            BroadCastMessage cD = null;
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
                            apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Delete, "BroadCastMessage", data);
                            BroadCastMessage serializedBroadCastMessage = JsonConvert
                                .DeserializeObject<BroadCastMessage>(apiViewModel.custom.ToString());
                            if (serializedBroadCastMessage != null)
                            {
                                dbContext = new UniversityContext();
                                cD = dbContext.BroadCastMessages.SingleOrDefault(x => x.BroadCastMessageId == serializedBroadCastMessage.BroadCastMessageId
                                    && x.StatusCode == StatusCodeConstants.ACTIVE && x.TenantId == tenant.TenantId);
                                if (cD != null)
                                {
                                    cD.StatusCode = StatusCodeConstants.INACTIVE;
                                    cD.LastModifiedBy = currentUser.UserId;
                                    cD.LastModifiedOn = DateTime.Now;
                                    dbContext.SaveChanges();

                                    return Serializer.ReturnContent("Broadcast Message deleted successfully."
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
