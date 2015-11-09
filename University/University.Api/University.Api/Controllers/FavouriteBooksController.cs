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
using University.Security.Models.ViewModel;
using University.Utilities;

namespace University.Api.Controllers
{
    public class FavouriteBooksController : UnSecuredController
    {
        public HttpResponseMessage Get([FromUri]int applicationUserId)
        {
            //_logger.Info("FavouriteBooks HttpGet - Called");
            UniversityContext dbContext = null;
            List<BookCorner_vm> lstBookCorner = null;
            Tenant tenant = null;
            int apiDataLogId = 0;
            CurrentUser currentUser = null;
            try
            {
                tenant = CurrentTenant;
                if (tenant.HasValue())
                {
                    dbContext = new UniversityContext();
                    lstBookCorner = (from fB in dbContext.FavouriteBooks.Include("BookCorner")
                                   .Where(x =>
                                       x.CreatedBy.Value == applicationUserId
                                       && x.BookCorner.College.StatusCode == StatusCodeConstants.ACTIVE
                                       && x.BookCorner.Department.StatusCode == StatusCodeConstants.ACTIVE
                                       && x.StatusCode == StatusCodeConstants.ACTIVE
                                       && x.TenantId == tenant.TenantId)
                                     select new BookCorner_vm
                                     {
                                         ApplicationUserId = fB.BookCorner.ApplicationUser.ApplicationUserId,
                                         FavouriteBookId = fB.FavouriteBookId,
                                         BookId = fB.BookCornerId,
                                         PostedType = fB.BookCorner.PostedType,
                                         UserName = fB.BookCorner.ApplicationUser.FirstName + " " + fB.BookCorner.ApplicationUser.LastName,
                                         FirstName = fB.BookCorner.ApplicationUser.FirstName,
                                         LastName = fB.BookCorner.ApplicationUser.LastName,
                                         EmailAddress = fB.BookCorner.ApplicationUser.EmailAddress,
                                         Contact = fB.BookCorner.ApplicationUser.Contact,
                                         CollegeId = fB.BookCorner.CollegeId.Value,
                                         CollegeName = fB.BookCorner.College.CollegeName,
                                         DepartmentId = fB.BookCorner.DepartmentId.Value,
                                         DepartmentName = fB.BookCorner.Department.DepartmentName,
                                         BookName = fB.BookCorner.BookName,
                                         AuthorName = fB.BookCorner.AuthorName,
                                         Description = fB.BookCorner.Description,
                                         BookLanguage = fB.BookCorner.BookLanguage.ToString(),
                                         PostedDate = fB.BookCorner.CreatedOn.ToString(),
                                         IsFavouriteBook = true,
                                         //DaysAgo = DbFunctions.DiffDays(fB.CreatedOn, DateTime.Today).Value,
                                     }).OrderByDescending(x => x.PostedDate).ToList();
                    if (lstBookCorner.HasValue())
                    {
                        lstBookCorner.ForEach(x => Separate(x));
                    }
                    return Serializer.ReturnContent(lstBookCorner, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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

        private object Separate(BookCorner_vm x)
        {
            if (x != null)
            {
                x.DaysAgo = TimeSpanFormat.FormatDaysAgo(x.PostedDate);
            }
            return x;
        }

        public HttpResponseMessage Post(ApiViewModel apiViewModel)
        {
            //_logger.Info("FavouriteBooks HttpPost - Called");
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
                            apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Post, "FavouriteBooks", data);
                            FavouriteBooks_vm serializedFavouriteBooks = JsonConvert
                                .DeserializeObject<FavouriteBooks_vm>(apiViewModel.custom.ToString());
                            if (serializedFavouriteBooks != null)
                            {
                                dbContext = new UniversityContext();
                                if (serializedFavouriteBooks.BookIds.HasValue())
                                {
                                    foreach (var bookId in serializedFavouriteBooks.BookIds)
                                    {
                                        if (bookId > 0)
                                        {
                                            if (dbContext.FavouriteBooks.Count(x => x.BookCornerId == bookId
                                                && x.CreatedBy == currentUser.UserId && x.TenantId == tenant.TenantId) == 0)
                                            {
                                                FavouriteBook fB = new FavouriteBook
                                                {
                                                    BookCornerId = bookId,
                                                    CreatedBy = currentUser.UserId,
                                                    CreatedOn = DateTime.Now,
                                                    TenantId = currentUser.TenantId,
                                                    StatusCode = StatusCodeConstants.ACTIVE,
                                                    Language = Language.English
                                                };
                                                dbContext.FavouriteBooks.Add(fB);
                                            }
                                        }
                                    }
                                }
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


        public HttpResponseMessage Delete(ApiViewModel apiViewModel)
        {
            //_logger.Info("FavouriteBooks Delete - Called");
            CurrentUser currentUser = null;
            UniversityContext dbContext = null;
            int apiDataLogId = 0;
            Tenant tenant = null;
            string data = string.Empty;
            FavouriteBook cD = null;
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
                            apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Delete, "FavouriteBook", data);
                            FavouriteBook serializedFavouriteBook = JsonConvert
                                .DeserializeObject<FavouriteBook>(apiViewModel.custom.ToString());
                            if (serializedFavouriteBook != null)
                            {
                                dbContext = new UniversityContext();
                                cD = dbContext.FavouriteBooks.SingleOrDefault(x => x.FavouriteBookId == serializedFavouriteBook.FavouriteBookId
                                    && x.StatusCode == StatusCodeConstants.ACTIVE && x.TenantId == tenant.TenantId);
                                if (cD != null)
                                {
                                    dbContext.FavouriteBooks.Remove(cD);
                                    //cD.StatusCode = StatusCodeConstants.INACTIVE;
                                    //cD.LastModifiedBy = currentUser.UserId;
                                    //cD.LastModifiedOn = DateTime.Now;
                                    dbContext.SaveChanges();

                                    return Serializer.ReturnContent(HttpConstants.Deleted
                                    , this.Configuration.Services.GetContentNegotiator()
                                    , this.Configuration.Formatters, this.Request);
                                }
                                else
                                {
                                    _logger.Warn("FavouriteBooks does not exists");
                                    return Serializer.ReturnContent("FavouriteBooks does not exists", this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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
