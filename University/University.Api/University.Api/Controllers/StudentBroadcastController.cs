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
using University.Utilities;

namespace University.Api.Controllers
{
    public class StudentBroadcastController : UnSecuredController
    {
        public HttpResponseMessage Get([FromUri]int studentId)
        {
            //_logger.Info("Student BroadCast HttpGet - Called");
            UniversityContext dbcontext = null;
            List<Broadcast_vm> lstBroadCast = null;
            int apiDataLogId = 0;
            Tenant tenant = null;
            CurrentUser currentUser = null;
            try
            {
                tenant = CurrentTenant;
                if (tenant != null)
                {
                    dbcontext = new UniversityContext();
                    var map = dbcontext.StudentSubscriptions
                        .Where(x => x.ApplicationUserId == studentId && x.TenantId == tenant.TenantId
                            && x.StatusCode == StatusCodeConstants.ACTIVE).ToList();
                    lstBroadCast = (from sub in dbcontext.StudentSubscriptions.Where(x => x.ApplicationUserId == studentId && x.TenantId == tenant.TenantId
                             && x.StatusCode == StatusCodeConstants.ACTIVE)
                                    join
                                    bC in dbcontext.BroadCasts.Include("ApplicationUser").Include("ClassDetail")
                                    .Where(x => x.ClassDetail.StatusCode == StatusCodeConstants.ACTIVE && x.ClassDetail.College.StatusCode == StatusCodeConstants.ACTIVE
                                    && x.ClassDetail.Department.StatusCode == StatusCodeConstants.ACTIVE
                                    //&& (x.ScheduleDate == null || x.ScheduleDate.Value <= DateTime.Now)
                                    )
                                    on sub.ClassDetailId equals bC.ClassDetailId
                                    join
                                    user in dbcontext.ApplicationUsers.Where(x => x.StatusCode == StatusCodeConstants.ACTIVE) on bC.ApplicationUserId equals user.ApplicationUserId
                                    select new Broadcast_vm
                                    {
                                        BroadCastId = bC.BroadCastId,
                                        CanReply = ((bC.RestrictedUsers.Count(x => x.ApplicationUserId == studentId && x.TenantId == tenant.TenantId && x.StatusCode == StatusCodeConstants.ACTIVE)
                                        == 0)
                                        ?
                                        (
                                        dbcontext.RestrictedUsers
                                        .Count(x =>
                                            x.ApplicationUserId == studentId && x.TenantId == tenant.TenantId && x.StatusCode == StatusCodeConstants.ACTIVE
                                            && (x.Module == Module.All || (x.Module == Module.BroadCast && x.ClassDetailId == bC.ClassDetailId.Value))
                                            )
                                        > 0 ? false : true
                                        )
                                        :
                                        false),
                                        ClassId = bC.ClassDetailId.Value,
                                        ClassName = bC.ClassDetail.ClassName,
                                        DepartmentId = bC.ClassDetail.DepartmentId,
                                        DepartmentName = bC.ClassDetail.Department.DepartmentName,
                                        FacultyId = bC.ApplicationUserId.Value,
                                        FacultyUserName = user.FirstName + " " + user.LastName,
                                        CollegeId = bC.ClassDetail.CollegeId,
                                        CollegeName = bC.ClassDetail.College.CollegeName,
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
                    return Serializer.ReturnContent(lstBroadCast.Where(x => x.ScheduleDate == null ||
                    x.ScheduleDate.Value.ToUniversalTime() >= DateTime.UtcNow), this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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

        //public HttpResponseMessage Post(ApiViewModel apiViewModel)
        //{
        //    _logger.Info("Student BroadCast HttpPost - Called");
        //    CurrentUser currentUser = null;
        //    int apiDataLogId = 0;
        //    Tenant tenant = null;
        //    string data = string.Empty;
        //    try
        //    {
        //        if (apiViewModel.HasValue())
        //        {
        //            tenant = CurrentTenant;
        //            if (tenant.HasValue())
        //            {
        //                Token = apiViewModel.Token;
        //                currentUser = ApiUser;
        //                if (currentUser.HasValue())
        //                {
        //                    data = JsonConvert.SerializeObject(apiViewModel.custom);
        //                    apiDataLogId = DataLog
        //                        .LogData(currentUser, VerbConstants.Post, "Student BroadCast", data);
        //                    StudentBroadcast_vm serializedStudentBroadcast = JsonConvert
        //                        .DeserializeObject<StudentBroadcast_vm>(apiViewModel.custom.ToString());
        //                    if (serializedStudentBroadcast != null)
        //                    {
        //                        dbContext = new UniversityContext();
        //                        var cD = dbContext.ClassDetails
        //                            .SingleOrDefault(x => x.ClassDetailId == serializedStudentBroadcast.ClassId
        //                           && x.TenantId == tenant.TenantId && x.StatusCode == StatusCodeConstants.ACTIVE);
        //                        if (cD != null)
        //                        {
        //                            if (cD.IsPasswordProtected)
        //                            {
        //                                byte[] strSalt = cD.PasswordSalt;
        //                                string salt = Convert.ToBase64String(strSalt);
        //                                byte[] dbPasswordHash = cD.PasswordHash;
        //                                byte[] userPasswordHash = Encryptor.GenerateHash(serializedStudentBroadcast.Password, salt);
        //                                bool chkPassword = Encryptor.CompareByteArray(dbPasswordHash, userPasswordHash);
        //                                if (!chkPassword)
        //                                {
        //                                    _logger.Warn(HttpConstants.PasswordMismatch);
        //                                    return Serializer.ReturnContent(HttpConstants.PasswordMismatch, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
        //                                }
        //                            }
        //                            var bC = dbContext.BroadCasts.SingleOrDefault(x => x.ClassDetailId == cD.ClassDetailId
        //                                && x.StatusCode == StatusCodeConstants.ACTIVE && x.TenantId == tenant.TenantId);
        //                            if (bC != null)
        //                            {
        //                                BroadcastMap map = new BroadcastMap
        //                                {
        //                                    ApplicationUserId = currentUser.UserId,
        //                                    TenantId = tenant.TenantId,
        //                                    Language = Language.English,
        //                                    CreatedBy = currentUser.UserId,
        //                                    CreatedOn = DateTime.Now,
        //                                    StatusCode = StatusCodeConstants.ACTIVE
        //                                };
        //                                bC.BroadcastMaps.Add(map);
        //                                //map.BroadCasts.Add(bC);
        //                                //dbContext.BroadcastMaps.Add(map);
        //                                dbContext.SaveChanges();
        //                                return Serializer.ReturnContent(HttpConstants.Inserted
        //                                , this.Configuration.Services.GetContentNegotiator()
        //                                , this.Configuration.Formatters, this.Request);
        //                            }
        //                            else
        //                            {
        //                                _logger.Warn("BroadCast does not exists");
        //                                return Serializer.ReturnContent("BroadCast does not exists", this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            _logger.Warn("Class does not exists");
        //                            return Serializer.ReturnContent("Class does not exists", this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        _logger.Warn(HttpConstants.InvalidInput);
        //                        return Serializer.ReturnContent(HttpConstants.InvalidCurrentUser, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
        //                    }
        //                }
        //                else
        //                {
        //                    _logger.Warn(HttpConstants.InvalidCurrentUser);
        //                    return Serializer.ReturnContent(HttpConstants.InvalidCurrentUser, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
        //                }
        //            }
        //            else
        //            {
        //                _logger.Warn(HttpConstants.InvalidTenant);
        //                return Serializer.ReturnContent(HttpConstants.InvalidTenant, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
        //            }
        //        }
        //        else
        //        {
        //            _logger.Warn(HttpConstants.InvalidApiViewModel);
        //            return Serializer.ReturnContent(HttpConstants.InvalidApiViewModel, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Error(ex.Message);
        //        ErrorLog.LogCustomError(currentUser, ex, apiDataLogId);
        //        return new HttpResponseMessage(HttpStatusCode.InternalServerError);
        //    }
        //}
    }
}
