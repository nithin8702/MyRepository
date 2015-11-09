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
    public class ContactBookOwnerController : UnSecuredController
    {

        public HttpResponseMessage Get([FromUri]int applicationUserId)
        {
            //_logger.Info("ContactBookOwner HttpGet - Called");
            UniversityContext dbcontext = null;
            List<ContactBookOwner_vm> lstContactBookOwner = null;
            Tenant tenant = null;
            int apiDataLogId = 0;
            CurrentUser currentUser = null;
            try
            {
                tenant = CurrentTenant;
                if (tenant.HasValue())
                {
                    dbcontext = new UniversityContext();
                    string tmp = applicationUserId.ToString();
                    lstContactBookOwner = (from cB in dbContext.ContactBookOwners
                                   .Where(x =>
                                       (x.ToUserId == applicationUserId || x.FromUserId == applicationUserId)
                                       //&& x.StatusCode == StatusCodeConstants.ACTIVE
                                       && (x.CustomField02 != tmp && x.CustomField03 != tmp)
                                       && x.TenantId == tenant.TenantId)
                                           join user in dbContext.ApplicationUsers on cB.FromUserId equals user.ApplicationUserId
                                           select new ContactBookOwner_vm
                                           {
                                               ApplicationUserId = applicationUserId,
                                               ContactBookOwnerId = cB.ContactBookOwnerId,
                                               ToUserId = user.ApplicationUserId,
                                               UserName = user.FirstName + " " + user.LastName,
                                               FirstName = user.FirstName,
                                               LastName = user.LastName,
                                               EmailAddress = user.EmailAddress,
                                               Contact = user.Contact,
                                               Subject = cB.Subject,
                                               Body = cB.Body,
                                               CustomField01 = cB.CustomField01,
                                               BookId = cB.CustomField04,
                                               BookName = cB.CustomField05,
                                               Status = cB.CustomField06,
                                               PostedDate = cB.CreatedOn.ToString()
                                           }).OrderByDescending(x => x.PostedDate).ToList();
                    if (lstContactBookOwner.HasValue())
                    {
                        lstContactBookOwner.ForEach(x => Separate(x));
                    }
                    return Serializer.ReturnContent(lstContactBookOwner, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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

        private object Separate(ContactBookOwner_vm x)
        {
            if (x != null)
            {
                x.DaysAgo = TimeSpanFormat.FormatDaysAgo(x.PostedDate);
            }
            return x;
        }

        public HttpResponseMessage Post(ApiViewModel apiViewModel)
        {
            //_logger.Info("ContactBookOwner HttpPost - Called");
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
                            apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Post, "ContactBookOwner", data);
                            ContactBookOwner_vm serializedContactBookOwner = JsonConvert
                                .DeserializeObject<ContactBookOwner_vm>(apiViewModel.custom.ToString());
                            if (serializedContactBookOwner != null)
                            {
                                dbContext = new UniversityContext();
                                //if (dbContext.RestrictedUsers.Count(x => x.ApplicationUserId == currentUser.UserId
                                //    && (x.Module == Module.Message || x.Module == Module.All)) > 0)
                                //{
                                //    _logger.Warn("User Restricted.");
                                //    return Serializer.ReturnContent("User Restricted.", this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                                //}
                                ContactBookOwner cB = new ContactBookOwner
                                {
                                    FromUserId = currentUser.UserId,
                                    ToUserId = serializedContactBookOwner.ToUserId,
                                    Subject = serializedContactBookOwner.Subject,
                                    Body = serializedContactBookOwner.Body,
                                    CustomField01 = serializedContactBookOwner.CustomField01,
                                    CustomField04 = serializedContactBookOwner.BookId,
                                    CustomField05 = serializedContactBookOwner.BookName,
                                    CreatedBy = currentUser.UserId,
                                    CreatedOn = DateTime.Now,
                                    TenantId = currentUser.TenantId,
                                    StatusCode = StatusCodeConstants.ACTIVE,
                                    Language = Language.English
                                };
                                dbContext.ContactBookOwners.Add(cB);
                                dbContext.SaveChanges();
                                Notify.LogData(currentUser, dbContext, serializedContactBookOwner.ToUserId, Module.Message,
                                    currentUser.FullName + " has posted new message.", cB.ContactBookOwnerId, null, "ContactBookOwner");
                                dbContext.SaveChanges();
                                return Serializer.ReturnContent("Message Sent."
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
            //_logger.Info("ContactBookOwner Delete - Called");
            CurrentUser currentUser = null;
            UniversityContext dbContext = null;
            int apiDataLogId = 0;
            Tenant tenant = null;
            string data = string.Empty;
            ContactBookOwner cD = null;
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
                            apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Delete, "CollegeMap", data);
                            ContactBookOwner_vm serializedContactBookOwner = JsonConvert
                                .DeserializeObject<ContactBookOwner_vm>(apiViewModel.custom.ToString());
                            if (serializedContactBookOwner != null)
                            {
                                dbContext = new UniversityContext();
                                cD = dbContext.ContactBookOwners.SingleOrDefault(x => x.ContactBookOwnerId == serializedContactBookOwner.ContactBookOwnerId
                                    && x.TenantId == tenant.TenantId);
                                if (cD != null)
                                {
                                    //cD.StatusCode = StatusCodeConstants.INACTIVE;
                                    if (string.IsNullOrEmpty(cD.CustomField02))
                                    {
                                        cD.CustomField02 = currentUser.UserId.ToString();
                                    }
                                    else
                                    {
                                        cD.CustomField03 = currentUser.UserId.ToString();
                                    }
                                    cD.LastModifiedBy = currentUser.UserId;
                                    cD.LastModifiedOn = DateTime.Now;
                                    dbContext.SaveChanges();

                                    return Serializer.ReturnContent("Message Deleted."
                                    , this.Configuration.Services.GetContentNegotiator()
                                    , this.Configuration.Formatters, this.Request);
                                }
                                else
                                {
                                    _logger.Warn("ContactBookOwner does not exists");
                                    return Serializer.ReturnContent("ContactBookOwner does not exists", this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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
