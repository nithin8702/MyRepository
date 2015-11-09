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
    public class StatisticsController : UnSecuredController
    {

        public HttpResponseMessage Post(ApiViewModel apiViewModel)
        {
            //_logger.Info("Statistics HttpPost - Called");
            CurrentUser currentUser = null;
            UniversityContext dbContext = null;
            int apiDataLogId = 0;
            Tenant tenant = null;
            string data = string.Empty;
            List<ProfileVisit> lstProfileVisit = null;
            Statictics_vm statictics_vm = null;
            int NumberOfClasses = 0;
            int NumberOfStudents = 0;
            int NumberOfBroadCastsSends = 0;
            int NumberOfBroadCastsReceived = 0;
            int NumberOfMessageSends = 0;
            int NumberOfMessageReceived = 0;
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

                        var dbuser = dbContext.ApplicationUsers.Include("ProfileVisits")
                                   .SingleOrDefault(x => x.ApplicationUserId == currentUser.UserId
                                       && x.TenantId == tenant.TenantId
                                       && x.StatusCode == StatusCodeConstants.ACTIVE);
                        if (dbuser != null)
                        {
                            lstProfileVisit = dbuser.ProfileVisits;
                            if (lstProfileVisit.HasValue())
                            {
                                lstProfileVisit = lstProfileVisit.OrderByDescending(x => x.CreatedOn).ToList();
                            }

                            var clsIdTmp = dbContext.ClassDetails.Where(x => x.ApplicationUserId == currentUser.UserId
                                && x.TenantId == tenant.TenantId).ToList();
                            if (clsIdTmp.HasValue())
                            {
                                NumberOfClasses = clsIdTmp.Count;
                                if (NumberOfClasses > 0)
                                {
                                    var clsId = clsIdTmp.Select(x => x.ClassDetailId).ToList();
                                    NumberOfStudents = dbContext.StudentSubscriptions.Where(x => clsId.Contains(x.ClassDetailId.Value)).Count();
                                }
                            }
                            NumberOfBroadCastsReceived = (from sub in dbContext.StudentSubscriptions.Where(x => x.ApplicationUserId
                                  == currentUser.UserId
                                  && x.TenantId == tenant.TenantId
                       && x.StatusCode == StatusCodeConstants.ACTIVE)
                                                          join
                                                          bC in dbContext.BroadCasts.Where(x => x.ScheduleDate == null || x.ScheduleDate <= DateTime.Now)
                                                                              on sub.ClassDetailId equals bC.ClassDetailId
                                                          select bC.BroadCastId
                            ).Count();
                            NumberOfBroadCastsSends = dbContext.BroadCasts.Where(x => x.CreatedBy == currentUser.UserId).Count();

                            NumberOfMessageSends = dbContext.BroadCastMessages.Where(x => x.CreatedBy == currentUser.UserId).Count()
                                + dbContext.ComposeMessages.Where(x => x.FromUserId == currentUser.UserId).Count()
                                + dbContext.ContactBookOwners.Where(x => x.FromUserId == currentUser.UserId).Count()
                                + dbContext.TechnicalSupports.Where(x => x.CreatedBy == currentUser.UserId).Count()
                                + dbContext.AdminMessages.Where(x => x.CreatedBy == currentUser.UserId).Count();
                            NumberOfMessageReceived = dbContext.BroadCastMessages.Where(x => x.ApplicationUserId == currentUser.UserId).Count()
                                + dbContext.ComposeMessages.Where(x => x.ToUserId == currentUser.UserId).Count()
                                + dbContext.ContactBookOwners.Where(x => x.ToUserId == currentUser.UserId).Count()
                                + dbContext.AdminMessages.Where(x => x.ApplicationUserId == currentUser.UserId).Count();



                            statictics_vm = new Statictics_vm
                            {
                                ProfileVisits = lstProfileVisit,
                                NumberOfClasses = NumberOfClasses,
                                NumberOfStudents = NumberOfStudents,
                                NumberOfBroadCastsSends = NumberOfBroadCastsSends,
                                NumberOfBroadCastsReceived = NumberOfBroadCastsReceived,
                                NumberOfMessageSends = NumberOfMessageSends,
                                NumberOfMessageReceived = NumberOfMessageReceived
                            };

                            if (currentUser.AccountType == AccountType.Admin)
                            {
                                statictics_vm.NumberOfColleges = dbContext.Colleges.Where(x => x.TenantId == tenant.TenantId && x.StatusCode == StatusCodeConstants.ACTIVE).Count();
                                statictics_vm.NumberOfDepartments = dbContext.Departments.Where(x => x.TenantId == tenant.TenantId && x.StatusCode == StatusCodeConstants.ACTIVE).Count();
                                statictics_vm.NumberOfMaleFaculty = dbContext.ApplicationUsers.Where(x => x.TenantId == tenant.TenantId
                                    && x.AccountType == AccountType.Faculty && x.Gender == Gender.Male && x.StatusCode == StatusCodeConstants.ACTIVE).Count();
                                statictics_vm.NumberOfFemaleFaculty = dbContext.ApplicationUsers.Where(x => x.TenantId == tenant.TenantId
                                    && x.AccountType == AccountType.Faculty && x.Gender == Gender.Female && x.StatusCode == StatusCodeConstants.ACTIVE).Count();
                                statictics_vm.NumberOfMaleStudents = dbContext.ApplicationUsers.Where(x => x.TenantId == tenant.TenantId
                                    && x.AccountType == AccountType.Student && x.Gender == Gender.Male && x.StatusCode == StatusCodeConstants.ACTIVE).Count();
                                statictics_vm.NumberOfFemaleStudents = dbContext.ApplicationUsers.Where(x => x.TenantId == tenant.TenantId
                                    && x.AccountType == AccountType.Student && x.Gender == Gender.Female && x.StatusCode == StatusCodeConstants.ACTIVE).Count();
                                statictics_vm.NumberOfAdvertisements = dbContext.Advertisements.Where(x => x.TenantId == tenant.TenantId && x.StatusCode == StatusCodeConstants.ACTIVE).Count();
                            }


                            return Serializer.ReturnContent(statictics_vm
                                , this.Configuration.Services.GetContentNegotiator()
                                , this.Configuration.Formatters, this.Request);
                        }
                        else
                        {
                            _logger.Warn(HttpConstants.UserNotExists);
                            return Serializer.ReturnContent(HttpConstants.UserNotExists, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                ErrorLog.LogCustomError(currentUser, ex, apiDataLogId);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

    }
}
