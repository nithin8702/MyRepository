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
using University.Bussiness.Models;
using University.Bussiness.Models.ViewModel;
using University.Common.Models;
using University.Common.Models.Enums;
using University.Common.Models.Security;
using University.Constants;
using University.Context;
using University.Security.Models.ViewModel;

namespace University.Api.Controllers
{
    public class NeedBooksController : UnSecuredController
    {
        public HttpResponseMessage Get()
        {
            _logger.Info("NeedBook HttpGet All - Called");
            UniversityContext dbcontext = null;
            List<NeedBooks_vm> lstNeedBook = null;
            Tenant tenant = null;
            int apiDataLogId = 0;
            CurrentUser currentUser = null;
            try
            {
                tenant = CurrentTenant;
                if (tenant.HasValue())
                {
                    dbcontext = new UniversityContext();
                    lstNeedBook = (from nB in dbContext.NeedBooks.Include("ApplicationUser")
                                   .Where(x =>
                                       x.StatusCode == StatusCodeConstants.ACTIVE
                                       && x.TenantId == tenant.TenantId)
                                   select new NeedBooks_vm
                                   {
                                       ApplicationUserId = nB.ApplicationUser.ApplicationUserId,
                                       UserName = nB.ApplicationUser.UserName,
                                       FirstName = nB.ApplicationUser.FirstName,
                                       LastName = nB.ApplicationUser.LastName,
                                       EmailAddress = nB.ApplicationUser.EmailAddress,
                                       Contact = nB.ApplicationUser.Contact,
                                       CollegeId = nB.CollegeId.Value,
                                       CollegeName = nB.College.CollegeName,
                                       DepartmentId = nB.DepartmentId.Value,
                                       DepartmentName = nB.Department.DepartmentName,
                                       BookName = nB.BookName,
                                       AuthorName = nB.AuthorName,
                                       Description = nB.Description,
                                       BookLanguage = nB.BookLanguage.ToString(),
                                   }).ToList();
                    return Serializer.ReturnContent(lstNeedBook, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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

        public HttpResponseMessage Get([FromUri]int applicationUserId)
        {
            _logger.Info("NeedBook HttpGet - Called");
            UniversityContext dbcontext = null;
            List<NeedBooks_vm> lstNeedBook = null;
            Tenant tenant = null;
            int apiDataLogId = 0;
            CurrentUser currentUser = null;
            try
            {
                _logger.Info("aid" + applicationUserId);
                tenant = CurrentTenant;
                if (tenant.HasValue())
                {
                    dbcontext = new UniversityContext();
                    lstNeedBook = (from nB in dbContext.NeedBooks.Include("ApplicationUser")
                                   .Where(x =>
                                       x.ApplicationUserId == applicationUserId
                                       && x.StatusCode == StatusCodeConstants.ACTIVE
                                       && x.TenantId == tenant.TenantId)
                                   select new NeedBooks_vm
                                   {
                                       ApplicationUserId = nB.ApplicationUser.ApplicationUserId,
                                       UserName = nB.ApplicationUser.UserName,
                                       FirstName = nB.ApplicationUser.FirstName,
                                       LastName = nB.ApplicationUser.LastName,
                                       EmailAddress = nB.ApplicationUser.EmailAddress,
                                       Contact = nB.ApplicationUser.Contact,
                                       CollegeId = nB.CollegeId.Value,
                                       CollegeName=nB.College.CollegeName,
                                       DepartmentId = nB.DepartmentId.Value,
                                       DepartmentName=nB.Department.DepartmentName,
                                       BookName = nB.BookName,
                                       AuthorName = nB.AuthorName,
                                       Description = nB.Description,
                                       BookLanguage = nB.BookLanguage.ToString(),
                                   }).ToList();
                    return Serializer.ReturnContent(lstNeedBook, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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
            _logger.Info("NeedBook HttpPost - Called");
            CurrentUser currentUser = null;
            UniversityContext dbContext = null;
            int apiDataLogId = 0;
            Tenant tenant = null;
            string data = string.Empty;
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
                            apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Post, "NeedBook", data);
                            NeedBooks_vm serializedNeedBook_vm = JsonConvert
                                .DeserializeObject<NeedBooks_vm>(apiViewModel.custom.ToString());
                            if (serializedNeedBook_vm != null)
                            {
                                dbContext = new UniversityContext();
                                Language bookLanguage = Language.English;
                                switch (serializedNeedBook_vm.BookLanguage)
                                {
                                    case "English":
                                        bookLanguage = Language.English;
                                        break;
                                    case "Arabic":
                                        bookLanguage = Language.Arabic;
                                        break;
                                    case "French":
                                        bookLanguage = Language.French;
                                        break;
                                    case "German":
                                        bookLanguage = Language.German;
                                        break;
                                    case "Tamil":
                                        bookLanguage = Language.Tamil;
                                        break;
                                    case "Malayalam":
                                        bookLanguage = Language.Malayalam;
                                        break;
                                    case "Hindi":
                                        bookLanguage = Language.Hindi;
                                        break;
                                    default:
                                        bookLanguage = Language.English;
                                        break;
                                }
                                NeedBook nB = new NeedBook
                                {
                                    ApplicationUserId = currentUser.UserId,
                                    CollegeId = serializedNeedBook_vm.CollegeId,
                                    DepartmentId = serializedNeedBook_vm.DepartmentId,
                                    BookName = serializedNeedBook_vm.BookName,
                                    AuthorName = serializedNeedBook_vm.AuthorName,
                                    Description = serializedNeedBook_vm.Description,
                                    IsFavouriteBook=serializedNeedBook_vm.IsFavouriteBook,
                                    BookLanguage = bookLanguage,
                                    CreatedBy = currentUser.UserId,
                                    CreatedOn = DateTime.Now,
                                    TenantId = currentUser.TenantId,
                                    StatusCode = StatusCodeConstants.ACTIVE,
                                    Language = Language.English
                                };
                                dbContext.NeedBooks.Add(nB);
                                dbContext.SaveChanges();
                                return Serializer.ReturnContent(HttpConstants.Inserted
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

    }
}
