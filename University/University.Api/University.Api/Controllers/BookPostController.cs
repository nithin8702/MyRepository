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
    public class BookPostController : UnSecuredController
    {
        public HttpResponseMessage Get()
        {
            //_logger.Info("BookCorners HttpGet All - Called");
            UniversityContext dbContext = null;
            List<BookCorner_vm> lstAvailableBook = null;
            Tenant tenant = null;
            int apiDataLogId = 0;
            CurrentUser currentUser = null;
            try
            {
                tenant = CurrentTenant;
                if (tenant.HasValue())
                {
                    dbContext = new UniversityContext();
                    lstAvailableBook = (from nB in dbContext.BookCorners.Include("College").Include("Department")
                                                .Where(x =>
                                                    x.TenantId == tenant.TenantId && x.StatusCode == StatusCodeConstants.ACTIVE
                                                    && x.College.StatusCode == StatusCodeConstants.ACTIVE
                                                    && x.Department.StatusCode == StatusCodeConstants.ACTIVE
                                                    )
                                        select new BookCorner_vm
                                        {
                                            BookId = nB.BookCornerId,
                                            BookName = nB.BookName,
                                            AuthorName = nB.AuthorName,
                                            BookLanguage = nB.BookLanguage.ToString(),
                                            Description = nB.Description,
                                            CollegeId = nB.CollegeId.Value,
                                            CollegeName = nB.College.CollegeName,
                                            DepartmentId = nB.DepartmentId.Value,
                                            DepartmentName = nB.Department.DepartmentName,
                                            ImageUpload1 = nB.ImagePath1,
                                            ImageUpload2 = nB.ImagePath2,
                                            ImageUpload3 = nB.ImagePath3,
                                            ImageUpload4 = nB.ImagePath4,
                                            PostedType = nB.PostedType,
                                            PostedDate = nB.CreatedOn.ToString(),
                                            //DaysAgo = DbFunctions.DiffDays(nB.CreatedOn, DateTime.Today).Value,
                                            IsFavouriteBook = (from a in dbContext.FavouriteBooks
                                                                   .Where(x => x.CreatedBy == nB.ApplicationUserId && x.StatusCode == StatusCodeConstants.ACTIVE
                                                                       && x.BookCornerId == nB.BookCornerId)
                                                               select new { b = true }).Any()
                                        }).OrderByDescending(x => x.PostedDate)
                                             .ToList();
                    if (lstAvailableBook.HasValue())
                    {
                        lstAvailableBook.ForEach(x => Separate(x));
                    }
                    return Serializer.ReturnContent(lstAvailableBook, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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

        public HttpResponseMessage Get([FromUri]int applicationUserId)
        {
            //_logger.Info("BookCorners HttpGet - Called");
            UniversityContext dbContext = null;
            List<BookCorner_vm> lstAvailableBook = null;
            Tenant tenant = null;
            int apiDataLogId = 0;
            CurrentUser currentUser = null;
            try
            {
                tenant = CurrentTenant;
                if (tenant.HasValue())
                {
                    dbContext = new UniversityContext();
                    lstAvailableBook = (from nB in dbContext.BookCorners.Include("College").Include("Department")
                                                .Where(x => x.ApplicationUserId == applicationUserId &&
                                                    x.TenantId == tenant.TenantId && x.StatusCode == StatusCodeConstants.ACTIVE
                                                    && x.College.StatusCode == StatusCodeConstants.ACTIVE
                                                    && x.Department.StatusCode == StatusCodeConstants.ACTIVE
                                                    )
                                        select new BookCorner_vm
                                        {
                                            BookId = nB.BookCornerId,
                                            BookName = nB.BookName,
                                            AuthorName = nB.AuthorName,
                                            BookLanguage = nB.BookLanguage.ToString(),
                                            Description = nB.Description,
                                            CollegeId = nB.CollegeId.Value,
                                            CollegeName = nB.College.CollegeName,
                                            DepartmentId = nB.DepartmentId.Value,
                                            DepartmentName = nB.Department.DepartmentName,
                                            ImageUpload1 = nB.ImagePath1,
                                            ImageUpload2 = nB.ImagePath2,
                                            ImageUpload3 = nB.ImagePath3,
                                            ImageUpload4 = nB.ImagePath4,
                                            PostedType = nB.PostedType,
                                            PostedDate = nB.CreatedOn.ToString(),
                                            //DaysAgo = DbFunctions.DiffDays(nB.CreatedOn, DateTime.Today).Value,
                                            IsFavouriteBook = (from a in dbContext.FavouriteBooks
                                                                   .Where(x => x.CreatedBy == applicationUserId && x.BookCornerId == nB.BookCornerId && x.StatusCode == StatusCodeConstants.ACTIVE)
                                                               select new { b = true }).Any()
                                        }).OrderByDescending(x => x.PostedDate)
                                             .ToList();
                    if (lstAvailableBook.HasValue())
                    {
                        lstAvailableBook.ForEach(x => Separate(x));
                    }
                    return Serializer.ReturnContent(lstAvailableBook, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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
