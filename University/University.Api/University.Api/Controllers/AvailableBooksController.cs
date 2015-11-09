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
    public class AvailableBooksController : UnSecuredController
    {
        public HttpResponseMessage Get()
        {
            _logger.Info("AvailableBooks HttpGet All - Called");
            UniversityContext dbcontext = null;
            List<AvailableBooks_vm> lstAvailableBooks = null;
            Tenant tenant = null;
            int apiDataLogId = 0;
            CurrentUser currentUser = null;
            try
            {
                tenant = CurrentTenant;
                if (tenant.HasValue())
                {
                    dbcontext = new UniversityContext();
                    lstAvailableBooks = (from nB in dbContext.AvailableBooks.Include("ApplicationUser")
                                   .Where(x =>
                                       x.StatusCode == StatusCodeConstants.ACTIVE
                                       && x.TenantId == tenant.TenantId)
                                         select new AvailableBooks_vm
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
                    return Serializer.ReturnContent(lstAvailableBooks, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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
            _logger.Info("AvailableBooks HttpGet All - Called");
            UniversityContext dbcontext = null;
            List<AvailableBooks_vm> lstAvailableBooks = null;
            Tenant tenant = null;
            int apiDataLogId = 0;
            CurrentUser currentUser = null;
            try
            {
                tenant = CurrentTenant;
                if (tenant.HasValue())
                {
                    dbcontext = new UniversityContext();
                    lstAvailableBooks = (from nB in dbContext.AvailableBooks.Include("ApplicationUser")
                                   .Where(x => x.ApplicationUserId==applicationUserId
                                       && x.StatusCode == StatusCodeConstants.ACTIVE
                                       && x.TenantId == tenant.TenantId)
                                         select new AvailableBooks_vm
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
                    return Serializer.ReturnContent(lstAvailableBooks, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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
            _logger.Info("AvailableBooks HttpPost - Called");
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
                            apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Post, "AvailableBooks", data);
                            AvailableBooks_vm serializedAvailableBook_vm = JsonConvert
                                .DeserializeObject<AvailableBooks_vm>(apiViewModel.custom.ToString());
                            if (serializedAvailableBook_vm != null)
                            {
                                dbContext = new UniversityContext();
                                Language bookLanguage = Language.English;
                                switch (serializedAvailableBook_vm.BookLanguage)
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
                                AvailableBook aB = new AvailableBook
                                {
                                    ApplicationUserId = currentUser.UserId,
                                    CollegeId = serializedAvailableBook_vm.CollegeId,
                                    DepartmentId = serializedAvailableBook_vm.DepartmentId,
                                    BookName = serializedAvailableBook_vm.BookName,
                                    AuthorName = serializedAvailableBook_vm.AuthorName,
                                    Description = serializedAvailableBook_vm.Description,
                                    IsFavouriteBook = serializedAvailableBook_vm.IsFavouriteBook,
                                    BookLanguage = bookLanguage,
                                    CreatedBy = currentUser.UserId,
                                    CreatedOn = DateTime.Now,
                                    TenantId = currentUser.TenantId,
                                    ImagePath1 = serializedAvailableBook_vm.ImageUpload1,
                                    ImagePath2 = serializedAvailableBook_vm.ImageUpload2,
                                    ImagePath3 = serializedAvailableBook_vm.ImageUpload3,
                                    ImagePath4 = serializedAvailableBook_vm.ImageUpload4,
                                    StatusCode = StatusCodeConstants.ACTIVE,
                                    Language = Language.English
                                };
                                dbContext.AvailableBooks.Add(aB);
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
