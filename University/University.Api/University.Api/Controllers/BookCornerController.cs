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
using University.Security.Models;
using University.Security.Models.ViewModel;
using University.Utilities;

namespace University.Api.Controllers
{
    public class BookCornerController : UnSecuredController
    {
        public HttpResponseMessage Get([FromUri]BookCorner_vm bookCorner)
        {
            //_logger.Info("BookCorner HttpGet All - Called");
            UniversityContext dbContext = null;
            List<BookCorner_vm> lstAvailableBooks = null;
            Tenant tenant = null;
            int apiDataLogId = 0;
            CurrentUser currentUser = null;
            try
            {
                tenant = CurrentTenant;
                if (tenant.HasValue())
                {
                    dbContext = new UniversityContext();
                    //_logger.Info("book corner " + bookCorner.PostedType);
                    if (bookCorner.ApplicationUserId.HasValue())
                    {
                        lstAvailableBooks = (from nB in dbContext.BookCorners.Include("ApplicationUser").Include("College").Include("Department")
                                       .Where(x =>
                                           (x.PostedType == bookCorner.PostedType && x.College.StatusCode == StatusCodeConstants.ACTIVE && x.Department.StatusCode == StatusCodeConstants.ACTIVE)
                                           //&&
                                           //(
                                           //     x.ApplicationUserId == bookCorner.ApplicationUserId.Value
                                           //) 
                                           &&
                                           x.StatusCode == StatusCodeConstants.ACTIVE &&
                                           x.TenantId == tenant.TenantId)
                                             select new BookCorner_vm
                                             {
                                                 ApplicationUserId = nB.ApplicationUser.ApplicationUserId,
                                                 BookId = nB.BookCornerId,
                                                 PostedType = nB.PostedType,
                                                 UserName = nB.ApplicationUser.FirstName + " " + nB.ApplicationUser.LastName,
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
                                                 ImageUpload1 = nB.ImagePath1,
                                                 ImageUpload2 = nB.ImagePath2,
                                                 ImageUpload3 = nB.ImagePath3,
                                                 ImageUpload4 = nB.ImagePath4,
                                                 PostedDate = nB.CreatedOn.ToString(),
                                                 //DaysAgo = DbFunctions.DiffDays(nB.CreatedOn, DateTime.Today).Value,
                                                 IsFavouriteBook = (from a in dbContext.FavouriteBooks
                                                                       .Where(x => x.CreatedBy == bookCorner.ApplicationUserId.Value
                                                                           && x.TenantId == tenant.TenantId
                                                                           && x.StatusCode == StatusCodeConstants.ACTIVE
                                                                           && x.BookCornerId == nB.BookCornerId)
                                                                    select new { b = true }).Any()
                                             }).OrderByDescending(x => x.PostedDate).ToList();
                    }
                    else
                    {
                        lstAvailableBooks = (from nB in dbContext.BookCorners.Include("ApplicationUser").Include("College").Include("Department")
                                   .Where(x =>
                                       (x.PostedType == bookCorner.PostedType)
                                       && x.College.StatusCode == StatusCodeConstants.ACTIVE && x.Department.StatusCode == StatusCodeConstants.ACTIVE &&
                                       x.StatusCode == StatusCodeConstants.ACTIVE &&
                                       x.TenantId == tenant.TenantId)
                                             select new BookCorner_vm
                                             {
                                                 ApplicationUserId = nB.ApplicationUser.ApplicationUserId,
                                                 BookId = nB.BookCornerId,
                                                 PostedType = nB.PostedType,
                                                 UserName = nB.ApplicationUser.FirstName + " " + nB.ApplicationUser.LastName,
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
                                                 ImageUpload1 = nB.ImagePath1,
                                                 ImageUpload2 = nB.ImagePath2,
                                                 ImageUpload3 = nB.ImagePath3,
                                                 ImageUpload4 = nB.ImagePath4,
                                                 PostedDate = nB.CreatedOn.ToString(),
                                                 //DaysAgo = DbFunctions.DiffDays(nB.CreatedOn, DateTime.Today).Value,
                                                 IsFavouriteBook = (from a in dbContext.FavouriteBooks
                                                                       .Where(x => x.TenantId == tenant.TenantId &&
                                                                           x.BookCornerId == nB.BookCornerId)
                                                                    select new { b = true }).Any()
                                             }).OrderByDescending(x => x.PostedDate).ToList();
                    }

                    if (lstAvailableBooks.HasValue())
                    {
                        lstAvailableBooks.ForEach(x => Separate(x));
                    }
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

        private object Separate(BookCorner_vm x)
        {
            if (x != null)
            {
                x.ImageUpload1s = x.ImageUpload1.SplitIntoStringList(TechConstants.Separator);
                x.ImageUpload2s = x.ImageUpload2.SplitIntoStringList(TechConstants.Separator);
                x.ImageUpload3s = x.ImageUpload3.SplitIntoStringList(TechConstants.Separator);
                x.ImageUpload4s = x.ImageUpload4.SplitIntoStringList(TechConstants.Separator);
                x.DaysAgo = TimeSpanFormat.FormatDaysAgo(x.PostedDate);
            }
            return x;
        }

        //public HttpResponseMessage Get([FromUri]string postedType)
        //{
        //    _logger.Info("BookCorner HttpGet - Called");
        //    UniversityContext dbcontext = null;
        //    List<BookCorner_vm> lstAvailableBooks = null;
        //    Tenant tenant = null;
        //    int apiDataLogId = 0;
        //    CurrentUser currentUser = null;
        //    try
        //    {
        //        tenant = CurrentTenant;
        //        if (tenant.HasValue())
        //        {
        //            dbcontext = new UniversityContext();
        //            lstAvailableBooks = (from nB in dbContext.BookCorners.Include("ApplicationUser")
        //                           .Where(x =>
        //                               x.PostedType == postedType
        //                               && x.StatusCode == StatusCodeConstants.ACTIVE
        //                               && x.TenantId == tenant.TenantId)
        //                                 select new BookCorner_vm
        //                                 {
        //                                     ApplicationUserId = nB.ApplicationUser.ApplicationUserId,
        //                                     PostedType = nB.PostedType,
        //                                     UserName = nB.ApplicationUser.UserName,
        //                                     FirstName = nB.ApplicationUser.FirstName,
        //                                     LastName = nB.ApplicationUser.LastName,
        //                                     EmailAddress = nB.ApplicationUser.EmailAddress,
        //                                     Contact = nB.ApplicationUser.Contact,
        //                                     CollegeId = nB.CollegeId.Value,
        //                                     CollegeName = nB.College.CollegeName,
        //                                     DepartmentId = nB.DepartmentId.Value,
        //                                     DepartmentName = nB.Department.DepartmentName,
        //                                     BookName = nB.BookName,
        //                                     AuthorName = nB.AuthorName,
        //                                     Description = nB.Description,
        //                                     BookLanguage = nB.BookLanguage.ToString(),
        //                                     CreatedOn=nB.CreatedOn.ToString()
        //                                 }).ToList();
        //            return Serializer.ReturnContent(lstAvailableBooks, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
        //        }
        //        else
        //        {
        //            _logger.Warn(HttpConstants.InvalidTenant);
        //            return Serializer.ReturnContent(HttpConstants.InvalidTenant, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Error(ex.Message);
        //        if (!currentUser.HasValue())
        //        {
        //            currentUser = new CurrentUser { TenantId = tenant.TenantId };
        //        }
        //        ErrorLog.LogCustomError(currentUser, ex, apiDataLogId);
        //        return new HttpResponseMessage(HttpStatusCode.InternalServerError);
        //    }
        //}

        public HttpResponseMessage Post(ApiViewModel apiViewModel)
        {
            //_logger.Info("BookCorner HttpPost - Called");
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
                            apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Post, "BookCorner", data);
                            BookCorner_vm serializedBookCorner = JsonConvert
                                .DeserializeObject<BookCorner_vm>(apiViewModel.custom.ToString());
                            if (serializedBookCorner != null)
                            {
                                dbContext = new UniversityContext();
                                Language bookLanguage = Language.English;
                                switch (serializedBookCorner.BookLanguage)
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
                                BookCorner bCorner = new BookCorner
                                {
                                    PostedType = serializedBookCorner.PostedType,
                                    ApplicationUserId = currentUser.UserId,
                                    CollegeId = serializedBookCorner.CollegeId,
                                    DepartmentId = serializedBookCorner.DepartmentId,
                                    BookName = serializedBookCorner.BookName,
                                    AuthorName = serializedBookCorner.AuthorName,
                                    Description = serializedBookCorner.Description,
                                    BookLanguage = bookLanguage,
                                    CreatedBy = currentUser.UserId,
                                    CreatedOn = DateTime.Now,
                                    TenantId = currentUser.TenantId,
                                    ImagePath1 = serializedBookCorner.ImageUpload1,
                                    ImagePath2 = serializedBookCorner.ImageUpload2,
                                    ImagePath3 = serializedBookCorner.ImageUpload3,
                                    ImagePath4 = serializedBookCorner.ImageUpload4,
                                    StatusCode = StatusCodeConstants.ACTIVE,
                                    Language = Language.English
                                };
                                dbContext.BookCorners.Add(bCorner);
                                dbContext.SaveChanges();

                                var lstIds = dbContext.ApplicationUsers.Where(x => x.TenantId == tenant.TenantId && x.StatusCode == StatusCodeConstants.ACTIVE
                                    && x.DepartmentId == serializedBookCorner.DepartmentId).Select(x => x.ApplicationUserId).ToList();
                                if (lstIds.HasValue())
                                {
                                    //_logger.Info("count : " + lstIds.Count);
                                    foreach (var id in lstIds)
                                    {
                                        if (currentUser.UserId != id)
                                        {
                                            Notify.LogData(currentUser, dbContext, id, Module.BookCorner,
                                            serializedBookCorner.BookName + " has been posted in BookCorner.",
                                            bCorner.BookCornerId, null, serializedBookCorner.PostedType);
                                        }
                                    }
                                    dbContext.SaveChanges();
                                }

                                return Serializer.ReturnContent("Book Posted Successfully."
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
            //_logger.Info("Book Delete - Called");
            CurrentUser currentUser = null;
            UniversityContext dbContext = null;
            int apiDataLogId = 0;
            Tenant tenant = null;
            string data = string.Empty;
            BookCorner bC = null;
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
                            apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Delete, "BookCorner", data);
                            BookCorner_vm serializedBookCorner = JsonConvert
                                .DeserializeObject<BookCorner_vm>(apiViewModel.custom.ToString());
                            if (serializedBookCorner != null)
                            {
                                dbContext = new UniversityContext();
                                bC = dbContext.BookCorners.SingleOrDefault(x => x.BookCornerId == serializedBookCorner.BookId
                                    && x.ApplicationUserId == currentUser.UserId
                                    && x.StatusCode == StatusCodeConstants.ACTIVE && x.TenantId == tenant.TenantId);
                                if (bC != null)
                                {
                                    bC.StatusCode = StatusCodeConstants.INACTIVE;
                                    bC.LastModifiedBy = currentUser.UserId;
                                    bC.LastModifiedOn = DateTime.Now;
                                    string tmp = bC.BookCornerId.ToString();
                                    var lstCB = dbContext.ContactBookOwners.Where(x => x.TenantId == tenant.TenantId && x.CustomField04 == tmp).ToList();
                                    if (lstCB.HasValue())
                                    {
                                        foreach (var item in lstCB)
                                        {
                                            item.CustomField06 = HttpConstants.Deleted;
                                        }
                                    }
                                    dbContext.SaveChanges();

                                    var lstIds = dbContext.ApplicationUsers.Where(x => x.TenantId == tenant.TenantId && x.StatusCode == StatusCodeConstants.ACTIVE
                                    && x.DepartmentId == bC.DepartmentId).Select(x => x.ApplicationUserId).ToList();
                                    if (lstIds.HasValue())
                                    {
                                        foreach (var id in lstIds)
                                        {
                                            if (currentUser.UserId != id)
                                            {
                                                Notify.LogData(currentUser, dbContext, id, Module.BookCorner,
                                                currentUser.FullName + " has deleted book - " + bC.BookName, bC.BookCornerId, null, bC.PostedType);
                                            }
                                        }
                                        dbContext.SaveChanges();
                                    }

                                    return Serializer.ReturnContent("Book Post deleted Successfully."
                                    , this.Configuration.Services.GetContentNegotiator()
                                    , this.Configuration.Formatters, this.Request);
                                }
                                else
                                {
                                    _logger.Warn(HttpConstants.BOOKNOTFOUND);
                                    return Serializer.ReturnContent(HttpConstants.BOOKNOTFOUND, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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

        public HttpResponseMessage Put(ApiViewModel apiViewModel)
        {
            //_logger.Info("BookCorner HttpPut - Called");
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
                            apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Put, "BookCorner", data);
                            BookCorner_vm serializedBookCorner = JsonConvert
                                .DeserializeObject<BookCorner_vm>(apiViewModel.custom.ToString());
                            if (serializedBookCorner != null)
                            {
                                dbContext = new UniversityContext();
                                BookCorner bC = dbContext.BookCorners.SingleOrDefault(x => x.BookCornerId == serializedBookCorner.BookId
                                    && x.StatusCode == StatusCodeConstants.ACTIVE
                                   && x.TenantId == tenant.TenantId);
                                if (bC != null)
                                {
                                    Language bookLanguage = Language.English;
                                    switch (serializedBookCorner.BookLanguage)
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
                                    bC.BookName = serializedBookCorner.BookName;
                                    bC.AuthorName = serializedBookCorner.AuthorName;
                                    bC.Description = serializedBookCorner.Description;
                                    bC.ImagePath1 = serializedBookCorner.ImageUpload1;
                                    bC.ImagePath2 = serializedBookCorner.ImageUpload2;
                                    bC.ImagePath3 = serializedBookCorner.ImageUpload3;
                                    bC.ImagePath4 = serializedBookCorner.ImageUpload4;
                                    bC.BookLanguage = bookLanguage;
                                    bC.LastModifiedBy = currentUser.UserId;
                                    bC.LastModifiedOn = DateTime.Now;
                                }
                                dbContext.SaveChanges();

                                var lstIds = dbContext.ApplicationUsers.Where(x => x.TenantId == tenant.TenantId && x.StatusCode == StatusCodeConstants.ACTIVE
                                    && x.DepartmentId == bC.DepartmentId).Select(x => x.ApplicationUserId).ToList();
                                if (lstIds.HasValue())
                                {
                                    foreach (var id in lstIds)
                                    {
                                        if (currentUser.UserId != id)
                                        {
                                            Notify.LogData(currentUser, dbContext, id, Module.BookCorner,
                                            currentUser.FullName + " has updated details of book - " + serializedBookCorner.BookName, bC.BookCornerId, null, bC.PostedType);
                                        }
                                    }
                                    dbContext.SaveChanges();
                                }




                                return Serializer.ReturnContent("Book Post updated Successfully."
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
