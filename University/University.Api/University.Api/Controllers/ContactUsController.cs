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
    public class ContactUsController : UnSecuredController
    {
        public HttpResponseMessage Post(ApiViewModel apiViewModel)
        {
            //_logger.Info("Contact Us HttpPost - Called");
            CurrentUser currentUser = null;
            UniversityContext dbContext = null;
            int apiDataLogId = 0;
            Tenant tenant = null;
            string data = string.Empty;
            try
            {
                tenant = CurrentTenant;
                ContactUs_vm serializedClassDetail = JsonConvert
                        .DeserializeObject<ContactUs_vm>(apiViewModel.custom.ToString());
                if (serializedClassDetail != null)
                {
                    dbContext = new UniversityContext();
                    ContactUs cD = new ContactUs
                    {
                        Name = serializedClassDetail.Name,
                        Email = serializedClassDetail.Email,
                        Address = serializedClassDetail.Address,
                        Contact = serializedClassDetail.Contact,
                        Message = serializedClassDetail.Message,
                        CreatedOn = DateTime.Now,
                        TenantId = tenant.TenantId,
                        StatusCode = StatusCodeConstants.ACTIVE,
                    };
                    dbContext.ContactUs.Add(cD);
                    dbContext.SaveChanges();

                    //var adminIds = dbContext.ApplicationUsers.Where(x => x.TenantId == tenant.TenantId && x.StatusCode == StatusCodeConstants.ACTIVE
                    //                && x.AccountType == AccountType.Admin).Select(x => x.ApplicationUserId).ToList();
                    //if (adminIds.HasValue())
                    //{
                    //    currentUser = new CurrentUser { };
                    //    foreach (var id in adminIds)
                    //    {
                    //        if (id > 0)
                    //        {
                    //            Notify.LogData(currentUser, dbContext, id, Module.Message,
                    //                        currentUser.FullName + " has Contacted.", cD.ContactUsId, null, "Message");
                    //        }
                    //    }
                    //}
                    //dbContext.SaveChanges();


                    return Serializer.ReturnContent(HttpConstants.Inserted
                        , this.Configuration.Services.GetContentNegotiator()
                        , this.Configuration.Formatters, this.Request);
                }
                else
                {
                    _logger.Warn(HttpConstants.InvalidInput);
                    return Serializer.ReturnContent(HttpConstants.InvalidInput, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                ErrorLog.LogCustomError(currentUser, ex, apiDataLogId);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        public HttpResponseMessage Get()
        {
            //_logger.Info("Contact Us HttpGet - Called");
            int apiDataLogId = 0;
            UniversityContext dbcontext = null;
            List<ContactUs_vm> lstContactUs = null;
            Tenant tenant = null;
            CurrentUser currentUser = null;
            try
            {
                tenant = CurrentTenant;
                if (tenant.HasValue())
                {
                    dbcontext = new UniversityContext();
                    lstContactUs = (from cT in dbcontext.ContactUs
                                        .Where(x => x.StatusCode == StatusCodeConstants.ACTIVE
                                            && x.TenantId == tenant.TenantId
                                            )
                                    select new ContactUs_vm
                                    {
                                        ContactUsId = cT.ContactUsId,
                                        Name = cT.Name,
                                        Email = cT.Email,
                                        Address = cT.Address,
                                        Contact = cT.Contact,
                                        Message = cT.Message,
                                    }).ToList();
                    return Serializer.ReturnContent(lstContactUs, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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

        public HttpResponseMessage Delete(ApiViewModel apiViewModel)
        {
            //_logger.Info("ContactUs Delete - Called");
            CurrentUser currentUser = null;
            UniversityContext dbContext = null;
            int apiDataLogId = 0;
            Tenant tenant = null;
            string data = string.Empty;
            ContactUs cD = null;
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
                            apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Delete, "ContactUs", data);
                            ContactUs_vm serializedContactUs = JsonConvert
                                .DeserializeObject<ContactUs_vm>(apiViewModel.custom.ToString());
                            if (serializedContactUs != null)
                            {
                                dbContext = new UniversityContext();
                                cD = dbContext.ContactUs.SingleOrDefault(x => x.ContactUsId == serializedContactUs.ContactUsId
                                    && x.StatusCode == StatusCodeConstants.ACTIVE && x.TenantId == tenant.TenantId);
                                if (cD != null)
                                {
                                    cD.StatusCode = StatusCodeConstants.INACTIVE;
                                    cD.LastModifiedBy = currentUser.UserId;
                                    cD.LastModifiedOn = DateTime.Now;
                                    dbContext.SaveChanges();

                                    return Serializer.ReturnContent(HttpConstants.Deleted
                                    , this.Configuration.Services.GetContentNegotiator()
                                    , this.Configuration.Formatters, this.Request);
                                }
                                else
                                {
                                    _logger.Warn("ContactUs does not exists");
                                    return Serializer.ReturnContent("ContactUs does not exists", this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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
