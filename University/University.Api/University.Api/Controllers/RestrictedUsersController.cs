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
using University.Security.Models;
using University.Security.Models.ViewModel;
using University.Utilities;

namespace University.Api.Controllers
{
    public class RestrictedUsersController : UnSecuredController
    {

        public HttpResponseMessage Get([FromUri]RestrictedUser_vm restrictedUser)
        {
            //_logger.Info("RestrictedUsers HttpGet - Called");
            UniversityContext dbContext = null;
            List<RestrictedUser_vm> lstRestrictedUser = null;
            int apiDataLogId = 0;
            Tenant tenant = null;
            CurrentUser currentUser = null;
            IQueryable<RestrictedUser> resusers = null;
            try
            {
                tenant = CurrentTenant;
                if (tenant.HasValue())
                {
                    dbContext = new UniversityContext();
                    resusers = dbContext.RestrictedUsers
                                       .Where(x =>
                                           x.StatusCode == StatusCodeConstants.ACTIVE
                                           //&& x.ApplicationUserId == restrictedUser.ApplicationUserId
                                           && x.TenantId == tenant.TenantId);
                    if (restrictedUser.ClassDetailId.HasValue() && restrictedUser.ClassDetailId > 0)
                    {
                        resusers = resusers.Where(x => x.ClassDetailId == restrictedUser.ClassDetailId);
                    }
                    if (!string.IsNullOrEmpty(restrictedUser.ModuleName))
                    {
                        switch (restrictedUser.ModuleName)
                        {
                            case "BroadCast":
                                restrictedUser.Module = Module.BroadCast;
                                break;
                            case "Message":
                                restrictedUser.Module = Module.Message;
                                break;
                            case "All":
                                restrictedUser.Module = Module.All;
                                break;
                            default:
                                break;
                        }
                        if (restrictedUser.Module != null)
                        {
                            resusers = resusers.Where(x => x.Module == restrictedUser.Module);
                        }
                    }
                    lstRestrictedUser = (from item in resusers
                                         select new RestrictedUser_vm
                                         {
                                             Id = item.RestrictedUserId,
                                             ApplicationUserId = item.ApplicationUserId,
                                             ClassDetailId = item.ClassDetailId,
                                             ModuleName = item.Module.ToString()
                                         }).ToList();
                    return Serializer.ReturnContent(lstRestrictedUser, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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


        public HttpResponseMessage Post(ApiViewModel apiViewModel)
        {
            //_logger.Info("RestrictedUsers HttpPost - Called");
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
                                .LogData(currentUser, VerbConstants.Post, "RestrictedUsers", data);
                            List<RestrictedUser> lstRestrictedUser = JsonConvert
                                .DeserializeObject<List<RestrictedUser>>(apiViewModel.custom.ToString());
                            if (currentUser.AccountType == AccountType.Faculty)
                            {
                                if (lstRestrictedUser.HasValue())
                                {
                                    dbContext = new UniversityContext();
                                    foreach (var serializeduser in lstRestrictedUser)
                                    {
                                        if (serializeduser != null)
                                        {
                                            if (dbContext.RestrictedUsers.Count(x => x.Module == Module.Message
                                            && x.ApplicationUserId == serializeduser.ApplicationUserId && x.CreatedBy == currentUser.UserId) == 0)
                                            {
                                                RestrictedUser restrictedUser = new RestrictedUser
                                                {
                                                    ApplicationUserId = serializeduser.ApplicationUserId,
                                                    ClassDetailId = serializeduser.ClassDetailId,
                                                    Module = serializeduser.Module,
                                                    CreatedBy = currentUser.UserId,
                                                    CreatedOn = DateTime.Now,
                                                    TenantId = currentUser.TenantId,
                                                    StatusCode = StatusCodeConstants.ACTIVE,
                                                    Language = Language.English
                                                };
                                                dbContext.RestrictedUsers.Add(restrictedUser);
                                                dbContext.SaveChanges();
                                            }
                                        }
                                        else
                                        {
                                            _logger.Warn(HttpConstants.InvalidInput);
                                            return Serializer.ReturnContent(HttpConstants.InvalidInput, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                                        }
                                    }

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
            //_logger.Info("RestrictedUsers HttpPut - Called");
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
                                .LogData(currentUser, VerbConstants.Put, "RestrictedUsers", data);
                            List<RestrictedUser> lstRestrictedUser = JsonConvert
                                .DeserializeObject<List<RestrictedUser>>(apiViewModel.custom.ToString());
                            if (currentUser.AccountType == AccountType.Faculty)
                            {
                                if (lstRestrictedUser.HasValue())
                                {
                                    dbContext = new UniversityContext();
                                    foreach (var serializeduser in lstRestrictedUser)
                                    {
                                        if (serializeduser != null)
                                        {
                                            var restrictedUser = dbContext.RestrictedUsers.SingleOrDefault(x => x.RestrictedUserId == serializeduser.RestrictedUserId && x.TenantId == tenant.TenantId);
                                            if (restrictedUser != null)
                                            {
                                                restrictedUser.ClassDetailId = serializeduser.ClassDetailId;
                                                restrictedUser.Module = serializeduser.Module;
                                                restrictedUser.LastModifiedBy = currentUser.UserId;
                                                restrictedUser.LastModifiedOn = DateTime.Now;
                                            }
                                            dbContext.Entry<RestrictedUser>(restrictedUser).State = EntityState.Modified;
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

        public HttpResponseMessage Delete(ApiViewModel apiViewModel)
        {
            //_logger.Info("RestrictedUsers HttpDelete - Called");
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
                                .LogData(currentUser, VerbConstants.Delete, "RestrictedUsers", data);
                            List<RestrictedUser> lstRestrictedUser = JsonConvert
                                .DeserializeObject<List<RestrictedUser>>(apiViewModel.custom.ToString());
                            if (lstRestrictedUser.HasValue())
                            {
                                dbContext = new UniversityContext();
                                foreach (var serializeduser in lstRestrictedUser)
                                {
                                    if (serializeduser != null)
                                    {
                                        var restrictedUser = dbContext.RestrictedUsers.SingleOrDefault(x => x.RestrictedUserId == serializeduser.RestrictedUserId && x.TenantId == tenant.TenantId);
                                        if (restrictedUser != null)
                                        {
                                            dbContext.RestrictedUsers.Remove(restrictedUser);
                                        }
                                    }
                                    else
                                    {
                                        _logger.Warn(HttpConstants.InvalidInput);
                                        return Serializer.ReturnContent(HttpConstants.InvalidInput, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                                    }
                                }
                                dbContext.SaveChanges();
                                return Serializer.ReturnContent(HttpConstants.Deleted
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
