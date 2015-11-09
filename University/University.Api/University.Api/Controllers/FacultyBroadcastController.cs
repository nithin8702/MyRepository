using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
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
using University.Security.Models.ViewModel;
using University.Utilities;

namespace University.Api.Controllers
{
    public class FacultyBroadcastController : UnSecuredController
    {
        public HttpResponseMessage Get()
        {
            //_logger.Info("Faculty BroadCast HttpGet All - Called");
            UniversityContext dbcontext = null;
            List<Broadcast_vm> lstBroadCast = null;
            int apiDataLogId = 0;
            Tenant tenant = null;
            CurrentUser currentUser = null;
            try
            {
                tenant = CurrentTenant;
                if (tenant.HasValue())
                {
                    dbcontext = new UniversityContext();
                    lstBroadCast = (from bC in dbContext.BroadCasts.Include("ClassDetail")
                                   .Where(x =>
                                       x.StatusCode == StatusCodeConstants.ACTIVE
                                       && x.ClassDetail.StatusCode == StatusCodeConstants.ACTIVE
                                       && x.ClassDetail.College.StatusCode == StatusCodeConstants.ACTIVE
                                       && x.ClassDetail.Department.StatusCode == StatusCodeConstants.ACTIVE
                                       && x.TenantId == tenant.TenantId)
                                    join
                          user in dbContext.ApplicationUsers on bC.ApplicationUserId equals user.ApplicationUserId
                                    select new Broadcast_vm
                                    {
                                        CollegeId = bC.ClassDetail.CollegeId,
                                        CollegeName = bC.ClassDetail.College.CollegeName,
                                        DepartmentId = bC.ClassDetail.DepartmentId,
                                        DepartmentName = bC.ClassDetail.Department.DepartmentName,
                                        BroadCastId = bC.BroadCastId,
                                        ClassId = bC.ClassDetailId.Value,
                                        ClassName = bC.ClassDetail.ClassName,
                                        ScheduleDate = bC.ScheduleDate,
                                        FacultyId = bC.ApplicationUserId.Value,
                                        FacultyUserName = user.FirstName + " " + user.LastName,
                                        Sub = bC.Sub,
                                        Message = bC.Message,
                                        Path_Picture = bC.Path_Picture,
                                        Path_Doc = bC.Path_Doc,
                                        Path_Video = bC.Path_Video,
                                        Path_Voice = bC.Path_Voice,
                                        PostedDate = bC.CreatedOn.ToString(),
                                        ProfilePicturePath=user.ProfilePicturePath,
                                    }).OrderByDescending(x => x.PostedDate).ToList();
                    if (lstBroadCast.HasValue())
                    {
                        lstBroadCast.ForEach(x => Separate(x));
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
                if (!currentUser.HasValue())
                {
                    currentUser = new CurrentUser { TenantId = tenant.TenantId };
                }
                ErrorLog.LogCustomError(currentUser, ex, apiDataLogId);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
            return Serializer.ReturnContent(lstBroadCast, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
        }

        private object Separate(Broadcast_vm x)
        {
            if (x != null)
            {
                x.Path_Pictures = x.Path_Picture.SplitIntoStringList(TechConstants.Separator);
                x.Path_Docs = x.Path_Doc.SplitIntoStringList(TechConstants.Separator);
                x.Path_Videos = x.Path_Video.SplitIntoStringList(TechConstants.Separator);
                x.Path_Voices = x.Path_Voice.SplitIntoStringList(TechConstants.Separator);
                x.DaysAgo = TimeSpanFormat.FormatDaysAgo(x.PostedDate);
            }
            return x;
        }


        public HttpResponseMessage Get([FromUri]int facultyId)
        {
            //_logger.Info("Faculty BroadCast HttpGet - Called");
            UniversityContext dbcontext = null;
            List<Broadcast_vm> lstBroadCast = null;
            int apiDataLogId = 0;
            Tenant tenant = null;
            CurrentUser currentUser = null;
            try
            {
                tenant = CurrentTenant;
                if (tenant.HasValue())
                {
                    dbcontext = new UniversityContext();
                    lstBroadCast = (from bC in dbContext.BroadCasts.Include("ClassDetail")
                                       .Where(x =>
                                           x.StatusCode == StatusCodeConstants.ACTIVE
                                           && x.ClassDetail.StatusCode == StatusCodeConstants.ACTIVE
                                           && x.ClassDetail.College.StatusCode == StatusCodeConstants.ACTIVE
                                           && x.ClassDetail.Department.StatusCode == StatusCodeConstants.ACTIVE
                                           && x.ApplicationUserId == facultyId
                                           && x.TenantId == tenant.TenantId
                                           )
                                    join
                             user in dbContext.ApplicationUsers on bC.ApplicationUserId equals user.ApplicationUserId
                                    select new Broadcast_vm
                                    {
                                        CollegeId = bC.ClassDetail.CollegeId,
                                        CollegeName = bC.ClassDetail.College.CollegeName,
                                        DepartmentId = bC.ClassDetail.DepartmentId,
                                        DepartmentName = bC.ClassDetail.Department.DepartmentName,
                                        BroadCastId = bC.BroadCastId,
                                        ClassId = bC.ClassDetailId.Value,
                                        ClassName = bC.ClassDetail.ClassName,
                                        ScheduleDate = bC.ScheduleDate,
                                        FacultyId = bC.ApplicationUserId.Value,
                                        FacultyUserName = user.FirstName + " " + user.LastName,
                                        Sub = bC.Sub,
                                        Message = bC.Message,
                                        Path_Picture = bC.Path_Picture,
                                        Path_Doc = bC.Path_Doc,
                                        Path_Video = bC.Path_Video,
                                        Path_Voice = bC.Path_Voice,
                                        PostedDate = bC.CreatedOn.ToString(),
                                        ProfilePicturePath=user.ProfilePicturePath,
                                        //DaysAgo = (DateTime.Today - bC.CreatedOn.Value),
                                    }).OrderByDescending(x => x.PostedDate).ToList();
                    if (lstBroadCast.HasValue())
                    {
                        lstBroadCast.ForEach(x => Separate(x));
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
            return Serializer.ReturnContent(lstBroadCast, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
        }

        public HttpResponseMessage Post(ApiViewModel apiViewModel)
        {
            //_logger.Info("Faculty BroadCast HttpPost - Called");
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
                                .LogData(currentUser, VerbConstants.Post, "Faculty BroadCast", data);
                            List<Broadcast_vm> lstBroadcast = JsonConvert
                                .DeserializeObject<List<Broadcast_vm>>(apiViewModel.custom.ToString());
                            if (lstBroadcast.HasValue())
                            {
                                dbContext = new UniversityContext();
                                foreach (var serializedBroadcast in lstBroadcast)
                                {
                                    if (serializedBroadcast != null)
                                    {
                                        var cD = dbContext.ClassDetails
                                            .SingleOrDefault(x => x.ClassDetailId == serializedBroadcast.ClassId
                                            && x.ApplicationUserId == currentUser.UserId && x.StatusCode == StatusCodeConstants.ACTIVE && x.TenantId == tenant.TenantId);
                                        if (cD != null)
                                        {
                                            if (serializedBroadcast.ScheduleDate!=null && serializedBroadcast.ScheduleDate.HasValue)
                                            {
                                                serializedBroadcast.ScheduleDate = serializedBroadcast.ScheduleDate.Value.ToUniversalTime();
                                            }
                                            BroadCast broadcast = new BroadCast
                                            {
                                                ApplicationUserId = currentUser.UserId,
                                                ClassDetailId = serializedBroadcast.ClassId,
                                                Sub = serializedBroadcast.Sub,
                                                Message = serializedBroadcast.Message,
                                                Path_Picture = serializedBroadcast.Path_Picture,
                                                Path_Doc = serializedBroadcast.Path_Doc,
                                                Path_Video = serializedBroadcast.Path_Video,
                                                Path_Voice = serializedBroadcast.Path_Voice,
                                                ScheduleDate = serializedBroadcast.ScheduleDate,
                                                CreatedBy = currentUser.UserId,
                                                CreatedOn = DateTime.Now,
                                                TenantId = currentUser.TenantId,
                                                StatusCode = StatusCodeConstants.ACTIVE,
                                                Language = Language.English
                                            };
                                            if (serializedBroadcast.RestrictedUsers.HasValue())
                                            {
                                                foreach (var item in serializedBroadcast.RestrictedUsers)
                                                {
                                                    if (item > 0)
                                                    {
                                                        var subscription = dbContext.StudentSubscriptions.Include("ApplicationUser")
                                                            .SingleOrDefault(x => x.ApplicationUserId == item && x.ClassDetailId == serializedBroadcast.ClassId);
                                                        if (subscription != null)
                                                        {
                                                            broadcast.RestrictedUsers.Add(subscription.ApplicationUser);
                                                        }
                                                        else
                                                        {
                                                            _logger.Warn(HttpConstants.RESTRICTEDUSERISNOTVALID);
                                                            return Serializer.ReturnContent(HttpConstants.RESTRICTEDUSERISNOTVALID
                                                            , this.Configuration.Services.GetContentNegotiator()
                                                            , this.Configuration.Formatters, this.Request);
                                                        }
                                                    }
                                                }
                                            }
                                            dbContext.BroadCasts.Add(broadcast);
                                            dbContext.SaveChanges();
                                            var subs = dbContext.StudentSubscriptions
                                                .Where(x => x.ClassDetailId == serializedBroadcast.ClassId && x.TenantId == tenant.TenantId)
                                                .Select(y => y.ApplicationUserId).ToList();

                                            if (subs.HasValue())
                                            {
                                                foreach (var student in subs)
                                                {
                                                    Notify.LogData(currentUser, dbContext, student, Module.BroadCast,
                                                        currentUser.FullName + " has posted new broadcast for " + cD.ClassName,
                                                        broadcast.BroadCastId, cD.ClassDetailId);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            _logger.Warn(HttpConstants.CLASSNOTPOSTEDBYTHISUSER);
                                            return Serializer.ReturnContent(HttpConstants.CLASSNOTPOSTEDBYTHISUSER, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                                        }
                                    }
                                    else
                                    {
                                        _logger.Warn(HttpConstants.InvalidInput);
                                        return Serializer.ReturnContent(HttpConstants.InvalidCurrentUser, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                                    }
                                }
                                dbContext.SaveChanges();
                                return Serializer.ReturnContent("Broadcast Sent Successfully."
                                                , this.Configuration.Services.GetContentNegotiator()
                                                , this.Configuration.Formatters, this.Request);
                            }
                            else
                            {
                                _logger.Warn(HttpConstants.NOBROADCASTFOUND);
                                return Serializer.ReturnContent(HttpConstants.NOBROADCASTFOUND, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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
            //_logger.Info("Faculty BroadCast HttpPut - Called");
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
                                .LogData(currentUser, VerbConstants.Put, "Faculty BroadCast", data);
                            List<Broadcast_vm> lstBroadcast = JsonConvert
                                .DeserializeObject<List<Broadcast_vm>>(apiViewModel.custom.ToString());
                            if (lstBroadcast.HasValue())
                            {
                                dbContext = new UniversityContext();
                                foreach (var serializedBroadcast in lstBroadcast)
                                {
                                    if (serializedBroadcast != null)
                                    {
                                        var bC = dbContext.BroadCasts.Include("ClassDetail")
                                            .SingleOrDefault(x => x.BroadCastId == serializedBroadcast.BroadCastId
                                            && x.ApplicationUserId == currentUser.UserId && x.StatusCode == StatusCodeConstants.ACTIVE
                                            && x.TenantId == tenant.TenantId);
                                        if (bC != null)
                                        {
                                            bC.Sub = serializedBroadcast.Sub;
                                            bC.Message = serializedBroadcast.Message;
                                            bC.Path_Picture = serializedBroadcast.Path_Picture;
                                            bC.Path_Doc = serializedBroadcast.Path_Doc;
                                            bC.Path_Video = serializedBroadcast.Path_Video;
                                            bC.Path_Voice = serializedBroadcast.Path_Voice;
                                            bC.ScheduleDate = serializedBroadcast.ScheduleDate;
                                            bC.LastModifiedBy = currentUser.UserId;
                                            bC.LastModifiedOn = DateTime.Now;

                                            var subs = dbContext.StudentSubscriptions
                                                .Where(x => x.ClassDetailId == bC.ClassDetailId
                                                    && x.TenantId == tenant.TenantId)
                                                .Select(y => y.ApplicationUserId).ToList();

                                            if (subs.HasValue())
                                            {
                                                foreach (var student in subs)
                                                {
                                                    Notify.LogData(currentUser, dbContext, student, Module.BroadCast,
                                                        currentUser.FullName + " has updated broadcast" + bC.Sub + " for " + bC.ClassDetail.ClassName, bC.BroadCastId, bC.ClassDetailId);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            _logger.Warn(HttpConstants.CLASSNOTPOSTEDBYTHISUSER);
                                            return Serializer.ReturnContent(HttpConstants.CLASSNOTPOSTEDBYTHISUSER, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                                        }
                                    }
                                    else
                                    {
                                        _logger.Warn(HttpConstants.InvalidInput);
                                        return Serializer.ReturnContent(HttpConstants.InvalidInput, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                                    }
                                }
                                dbContext.SaveChanges();
                                return Serializer.ReturnContent("Broadcast updated Successfully."
                                                , this.Configuration.Services.GetContentNegotiator()
                                                , this.Configuration.Formatters, this.Request);
                            }
                            else
                            {
                                _logger.Warn(HttpConstants.NOBROADCASTFOUND);
                                return Serializer.ReturnContent(HttpConstants.NOBROADCASTFOUND, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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
            //_logger.Info("Faculty BroadCast HttpDelete - Called");
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
                                .LogData(currentUser, VerbConstants.Delete, "Faculty BroadCast", data);
                            List<Broadcast_vm> lstBroadcast = JsonConvert
                                .DeserializeObject<List<Broadcast_vm>>(apiViewModel.custom.ToString());
                            if (lstBroadcast.HasValue())
                            {
                                dbContext = new UniversityContext();
                                foreach (var serializedBroadcast in lstBroadcast)
                                {
                                    if (serializedBroadcast != null)
                                    {
                                        var bC = dbContext.BroadCasts.Include("ClassDetail")
                                            .SingleOrDefault(x => x.BroadCastId == serializedBroadcast.BroadCastId
                                            && x.ApplicationUserId == currentUser.UserId && x.StatusCode == StatusCodeConstants.ACTIVE
                                            && x.TenantId == tenant.TenantId);
                                        if (bC != null)
                                        {
                                            bC.StatusCode = StatusCodeConstants.INACTIVE;
                                            bC.LastModifiedBy = currentUser.UserId;
                                            bC.LastModifiedOn = DateTime.Now;

                                            var subs = dbContext.StudentSubscriptions
                                                .Where(x => x.ClassDetailId == bC.ClassDetailId
                                                    && x.TenantId == tenant.TenantId)
                                                .Select(y => y.ApplicationUserId).ToList();

                                            if (subs.HasValue())
                                            {
                                                foreach (var student in subs)
                                                {
                                                    Notify.LogData(currentUser, dbContext, student, Module.BroadCast,
                                                        currentUser.FullName + " has deleted broadcast " + bC.Sub + " for " + bC.ClassDetail.ClassName, bC.BroadCastId, bC.ClassDetailId);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            _logger.Warn(HttpConstants.CLASSNOTPOSTEDBYTHISUSER);
                                            return Serializer.ReturnContent(HttpConstants.CLASSNOTPOSTEDBYTHISUSER, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                                        }
                                    }
                                    else
                                    {
                                        _logger.Warn(HttpConstants.InvalidInput);
                                        return Serializer.ReturnContent(HttpConstants.InvalidCurrentUser, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                                    }
                                }
                                dbContext.SaveChanges();
                                return Serializer.ReturnContent("Broadcast deleted Successfully."
                                                , this.Configuration.Services.GetContentNegotiator()
                                                , this.Configuration.Formatters, this.Request);
                            }
                            else
                            {
                                _logger.Warn(HttpConstants.NOBROADCASTFOUND);
                                return Serializer.ReturnContent(HttpConstants.NOBROADCASTFOUND, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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
