using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using University.Bussiness.Models.ViewModel;
using University.Common.Models;
using University.Common.Models.Security;
using University.Context;
using University.Api.Extensions;
using University.Utilities;
using University.Constants;
using University.Api.Controllers.Serialize;
using University.Api.Controllers.Log;
using System.Net;
using University.Security.Models.ViewModel;
using University.Security.Models;

namespace University.Api.Controllers
{
    public class DepartmentFacultiesController : UnSecuredController
    {
        public HttpResponseMessage Get([FromUri]int departmentId)
        {
            //_logger.Info("DepartmentFaculties HttpGet - Called");
            UniversityContext dbcontext = null;
            Tenant tenant = null;
            int apiDataLogId = 0;
            CurrentUser currentUser = null;
            List<ApplicationUser_vm> lstApplicationUser_vm = null;
            try
            {
                tenant = CurrentTenant;
                if (tenant.HasValue())
                {
                    dbcontext = new UniversityContext();
                    lstApplicationUser_vm = (from dbuser in dbcontext.ClassDetails.Include("ApplicationUser").Include("College")
                                            .Where(x => x.DepartmentId == departmentId && x.TenantId == tenant.TenantId && x.StatusCode == StatusCodeConstants.ACTIVE)
                                            .Select(x => x.ApplicationUser)
                                            .Where(x => x.TenantId == tenant.TenantId && x.StatusCode == StatusCodeConstants.ACTIVE)
                                             select new ApplicationUser_vm
                                             {
                                                 UserName = dbuser.FirstName + " " + dbuser.LastName,
                                                 ApplicationUserId = dbuser.ApplicationUserId,
                                                 FirstName = dbuser.FirstName,
                                                 LastName = dbuser.LastName,
                                                 EmailAddress = dbuser.EmailAddress,
                                                 Gender = dbuser.Gender,
                                                 RefId = dbuser.RefId,
                                                 Contact = dbuser.Contact,
                                                 DOB = dbuser.DOB,
                                                 AccountType = dbuser.AccountType,
                                                 CollegeId = dbuser.CollegeId.Value,
                                                 //CollegeName=dbuser.College.CollegeName,
                                                 DepartmentId = dbuser.DepartmentId.Value,
                                                 WorkIdPicturePath = dbuser.WorkIdPicturePath,
                                                 ProfilePicturePath = dbuser.ProfilePicturePath,
                                                 CreatedBy = dbuser.CreatedBy,
                                                 CreatedOn = dbuser.CreatedOn,
                                             })
                                             .GroupBy(g => new { g.ApplicationUserId })
                                             .Select(s => s.FirstOrDefault())
                                             .ToList();
                    return Serializer.ReturnContent(lstApplicationUser_vm, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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
    }
}
