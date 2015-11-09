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
    public class AdminMessageController : UnSecuredController
    {

        public HttpResponseMessage Get([FromUri]int applicationUserId)
        {
            //_logger.Info("AdminMessage HttpGet - Called");
            UniversityContext dbcontext = null;
            List<AdminMessage_vm> lstAdminMessage = null;
            int apiDataLogId = 0;
            Tenant tenant = null;
            CurrentUser currentUser = null;
            try
            {
                tenant = CurrentTenant;
                if (tenant.HasValue())
                {
                    dbcontext = new UniversityContext();
                    lstAdminMessage = (from bC in dbContext.AdminMessageUsers.Include("AdminMessage").Include("ApplicationUser")
                                       .Where(x =>
                                           x.StatusCode == StatusCodeConstants.ACTIVE
                                           && x.ApplicationUserId == applicationUserId
                                           && x.TenantId == tenant.TenantId)
                                       select new AdminMessage_vm
                                       {
                                           AdminMessageId = bC.AdminMessageId,
                                           Message = bC.AdminMessage.Message,
                                           UserName = bC.AdminMessage.ApplicationUser.FirstName + " " + bC.AdminMessage.ApplicationUser.LastName,
                                           FirstName = bC.AdminMessage.ApplicationUser.FirstName,
                                           LastName = bC.AdminMessage.ApplicationUser.LastName,
                                           EmailAddress = bC.AdminMessage.ApplicationUser.EmailAddress,
                                           PostedDate = bC.AdminMessage.CreatedOn.ToString(),
                                           CustomField01 = bC.AdminMessage.CustomField01,
                                           CustomField02 = bC.AdminMessage.CustomField02,
                                           CustomField03 = bC.AdminMessage.CustomField03,
                                           CustomField04 = bC.AdminMessage.CustomField04,
                                           //DaysAgo = DbFunctions.DiffDays(bC.CreatedOn, DateTime.Today).Value
                                       }).OrderByDescending(x => x.PostedDate).ToList();
                    if (lstAdminMessage.HasValue())
                    {
                        lstAdminMessage.ForEach(x => Separate(x));
                    }
                }
                else
                    _logger.Warn("current user not satisfied");
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
            return Serializer.ReturnContent(lstAdminMessage, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
        }

        private object Separate(AdminMessage_vm x)
        {
            if (x != null)
            {
                x.DaysAgo = TimeSpanFormat.FormatDaysAgo(x.PostedDate);
            }
            return x;
        }



        public HttpResponseMessage Post(ApiViewModel apiViewModel)
        {
            //_logger.Info("Admin Message HttpPost - Called");
            string data = string.Empty;
            CurrentUser currentUser = null;
            UniversityContext dbContext = null;
            int apiDataLogId = 0;
            Tenant tenant = null;
            List<int> lstIds = null;
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
                            AdminMessage_vm serializedAdminMessage = JsonConvert.DeserializeObject<AdminMessage_vm>(apiViewModel.custom.ToString());
                            if (serializedAdminMessage != null)
                            {
                                dbContext = new UniversityContext();
                                MessageType msgType = serializedAdminMessage.MessageType;
                                AdminMessage msg = new AdminMessage
                                {
                                    ApplicationUserId = currentUser.UserId,
                                    Message = serializedAdminMessage.Message,
                                    MessageType = msgType,
                                    CustomField01 = serializedAdminMessage.CustomField01,
                                    CustomField02 = serializedAdminMessage.CustomField02,
                                    CustomField03 = serializedAdminMessage.CustomField03,
                                    CustomField04 = serializedAdminMessage.CustomField04,
                                    TenantId = tenant.TenantId,
                                    CreatedBy = currentUser.UserId,
                                    CreatedOn = DateTime.Now,
                                    Language = Language.English,
                                    StatusCode = StatusCodeConstants.ACTIVE
                                };
                                dbContext.AdminMessages.Add(msg);
                                lstIds = GetUserId(msgType, dbContext, tenant, serializedAdminMessage.Department);
                                if (lstIds.HasValue())
                                {
                                    foreach (var item in lstIds)
                                    {
                                        AdminMessageUser use = new AdminMessageUser
                                        {
                                            AdminMessageId = msg.AdminMessageId,
                                            ApplicationUserId = item,
                                            TenantId = tenant.TenantId,
                                            CreatedBy = currentUser.UserId,
                                            CreatedOn = DateTime.Now,
                                            Language = Language.English,
                                            StatusCode = StatusCodeConstants.NEW
                                        };
                                        dbContext.AdminMessageUsers.Add(use);
                                    }
                                }
                                dbContext.SaveChanges();
                                if (lstIds.HasValue())
                                {
                                    var tmp = dbContext.AdminMessageUsers.Where(x => x.AdminMessageId == msg.AdminMessageId
                                        && x.StatusCode == StatusCodeConstants.NEW && x.TenantId == tenant.TenantId).ToList();
                                    if (tmp != null)
                                    {
                                        foreach (var item in tmp)
                                        {
                                            if (item != null && item.ApplicationUserId != currentUser.UserId)
                                            {
                                                Notify.LogData(currentUser, dbContext, item.ApplicationUserId, Module.Message,
                                            currentUser.FullName + " has posted message ",
                                            item.AdminMessageUserId, null, "AdminMessage");
                                                item.StatusCode = StatusCodeConstants.ACTIVE;
                                            }
                                        }
                                        dbContext.SaveChanges();
                                    }
                                }
                                //_logger.Warn(HttpConstants.Inserted);
                                return Serializer.ReturnContent("Message Sent.", this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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



        private List<int> GetUserId(MessageType msgType, UniversityContext dbContext, Tenant tenant, Department department)
        {
            List<int> lstIds = null;
            IQueryable<ApplicationUser> user = null;
            try
            {
                lstIds = new List<int>();
                user = dbContext.ApplicationUsers.Where(x => x.TenantId == tenant.TenantId
                    && x.StatusCode == StatusCodeConstants.ACTIVE);
                if (department != null)
                {
                    if (department.CollegeId.HasValue() && department.CollegeId > 0)
                    {
                        user = user.Where(x => x.CollegeId == department.CollegeId);
                    }
                    if (department.DepartmentId > 0)
                    {
                        user = user.Where(x => x.DepartmentId == department.DepartmentId);
                    }
                }
                switch (msgType)
                {
                    case MessageType.All:
                        lstIds = user.Select(y => y.ApplicationUserId).ToList();
                        break;
                    case MessageType.AllFaculty:
                        lstIds = user.Where(x => x.AccountType == AccountType.Faculty)
                            .Select(y => y.ApplicationUserId).ToList();
                        break;
                    case MessageType.AllStudent:
                        lstIds = user.Where(x => x.AccountType == AccountType.Student)
                            .Select(y => y.ApplicationUserId).ToList();
                        break;
                    case MessageType.OnlyFemaleFaculty:
                        lstIds = user.Where(x => x.AccountType == AccountType.Faculty && x.Gender == Gender.Female)
                            .Select(y => y.ApplicationUserId).ToList();
                        break;
                    case MessageType.OnlyFemaleStudent:
                        lstIds = user.Where(x => x.AccountType == AccountType.Student && x.Gender == Gender.Female)
                            .Select(y => y.ApplicationUserId).ToList();
                        break;
                    case MessageType.OnlyMaleFaculty:
                        lstIds = user.Where(x => x.AccountType == AccountType.Faculty && x.Gender == Gender.Male)
                            .Select(y => y.ApplicationUserId).ToList();
                        break;
                    case MessageType.OnlyMaleStudent:
                        lstIds = user.Where(x => x.AccountType == AccountType.Student && x.Gender == Gender.Male)
                            .Select(y => y.ApplicationUserId).ToList();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {

                _logger.Error("ex.Message " + ex.Message);
                string message = (ex.InnerException != null) ? ((ex.InnerException.InnerException != null) ? ex.InnerException.InnerException.Message : ex.InnerException.ToString()) : ex.Message;
                _logger.Error("message " + message);
                string stackTrace = ex.StackTrace;
                _logger.Error("stackTrace : " + stackTrace);
            }
            return lstIds;
        }

        public HttpResponseMessage Delete(ApiViewModel apiViewModel)
        {
            //_logger.Info("AdminMessage Delete - Called");
            CurrentUser currentUser = null;
            UniversityContext dbContext = null;
            int apiDataLogId = 0;
            Tenant tenant = null;
            string data = string.Empty;
            AdminMessage cD = null;
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
                            apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Delete, "AdminMessage", data);
                            if (currentUser.AccountType != AccountType.Admin)
                            {
                                _logger.Warn(HttpConstants.NotAnAdminUser);
                                return Serializer.ReturnContent(HttpConstants.NotAnAdminUser, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                            }
                            AdminMessage_vm serializedAdminMessage = JsonConvert
                                .DeserializeObject<AdminMessage_vm>(apiViewModel.custom.ToString());
                            if (serializedAdminMessage != null)
                            {
                                dbContext = new UniversityContext();
                                cD = dbContext.AdminMessages.SingleOrDefault(x => x.AdminMessageId == serializedAdminMessage.AdminMessageId
                                    && x.StatusCode == StatusCodeConstants.ACTIVE && x.TenantId == tenant.TenantId);
                                if (cD != null)
                                {
                                    //cD.StatusCode = StatusCodeConstants.INACTIVE;
                                    //cD.LastModifiedBy = currentUser.UserId;
                                    //cD.LastModifiedOn = DateTime.Now;
                                    dbContext.AdminMessages.Remove(cD);
                                    dbContext.SaveChanges();

                                    return Serializer.ReturnContent("Message deleted."
                                    , this.Configuration.Services.GetContentNegotiator()
                                    , this.Configuration.Formatters, this.Request);
                                }
                                else
                                {
                                    _logger.Warn("AdminMessage does not exists");
                                    return Serializer.ReturnContent("AdminMessage does not exists", this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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
