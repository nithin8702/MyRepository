using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using University.Api.Controllers.Serialize;
using University.Common.Models;
using System.Web.Http;
using University.Security.Models.ViewModel;
using Newtonsoft.Json;
using University.Constants;
using University.Utilities;
using University.Api.Extensions;
using University.Context;
using University.Common.Models.Security;
using University.Common.Models.Enums;
using University.Security.Models;
using University.Common.Models.ViewModel;

namespace University.Api.Controllers
{
    public class TenantController : UnSecuredController
    {
        public HttpResponseMessage Get()
        {
            //_logger.Info("Tenant HttpGet - Called");
            List<Tenant_vm> lstTenant = null;
            try
            {

                UniversityContext dbContext = new UniversityContext();
                //_logger.Info("server : " + dbContext.Database.Connection.ConnectionString);
                lstTenant = (from tnt in dbContext.Tenants.Where(x => x.StatusCode == StatusCodeConstants.ACTIVE)
                             join
                             tkn in dbContext.TenantTokens.Where(x => x.StatusCode == StatusCodeConstants.ACTIVE) on tnt.TenantId equals tkn.TenantId
                             select new Tenant_vm
                             {
                                 TenantId = tnt.TenantId,
                                 TenantName = tnt.TenantName,
                                 DisplayName = tnt.DisplayName,
                                 CompanyUrl = tnt.CompanyUrl,
                                 IsRoot = tnt.IsRoot,
                                 LogoPath = tnt.LogoPath,
                                 Token = tkn.Token,
                                 SessionTime = tnt.SessionTime,
                                 NotificationTime = tnt.NotificationTime,
                                 ParentTenantId = tnt.ParentTenantId,
                             }).ToList();
                return Serializer.ReturnContent(lstTenant, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);

            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
            return Serializer.ReturnContent(HttpConstants.NotImplemented, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
        }

        public HttpResponseMessage Post(ApiViewModel apiViewModel)
        {
            //_logger.Info("Tenant HttpPost - Called");
            UniversityContext dbContext = null;
            //CurrentUser currentUser = null;
            try
            {
                if (apiViewModel.HasValue())
                {
                    Token = apiViewModel.Token;
                    //currentUser = ApiUser;
                    //_logger.Info("Token : " + Token);
                    dbContext = new UniversityContext();
                    var dbUser = dbContext.SecurityTokens.Include("ApplicationUser").SingleOrDefault(x => x.Token == Token);
                    if (dbUser != null && dbUser.ApplicationUser != null)
                    {
                        //_logger.Info("dbuser has value " + dbUser.ApplicationUserId);
                        if (dbUser.ApplicationUser.AccountType != AccountType.BranchAdmin)
                        {
                            //_logger.Warn("Not a Branch Admin.");
                            return Serializer.ReturnContent("Not a Branch Admin.", this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                        }
                        Tenant serializedTenant = JsonConvert
                        .DeserializeObject<Tenant>(apiViewModel.custom.ToString());
                        if (serializedTenant != null)
                        {
                            if (dbContext.Tenants.Count(x => x.TenantName == serializedTenant.TenantName) > 0)
                            {
                                //_logger.Warn("Tenant name already exists.");
                                return Serializer.ReturnContent("Tenant name already exists.", this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                            }
                            serializedTenant.ParentTenantId = dbUser.TenantId;
                            serializedTenant.StatusCode = StatusCodeConstants.ACTIVE;
                            serializedTenant.HostName = Request.Headers.Host;
                            serializedTenant.SessionTime = 20;
                            serializedTenant.NotificationTime = 1;
                            //serializedTenant.TenantId = dbUser.TenantId;
                            serializedTenant.CreatedBy = dbUser.ApplicationUserId;
                            serializedTenant.CreatedOn = DateTime.Now;
                            serializedTenant.Language = Language.English;

                            dbContext.Tenants.Add(serializedTenant);

                            TenantToken tenantToken = new TenantToken
                            {
                                Token = Guid.NewGuid().ToString("D") + Guid.NewGuid().ToString("D"),
                                TenantId = serializedTenant.TenantId,
                                Language = Language.English,
                                StatusCode = StatusCodeConstants.ACTIVE
                            };
                            dbContext.TenantTokens.Add(tenantToken);



                            dbContext.SaveChanges();
                            //_logger.Warn(HttpConstants.Inserted);
                            return Serializer.ReturnContent(HttpConstants.Inserted, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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
                    _logger.Warn(HttpConstants.InvalidApiViewModel);
                    return Serializer.ReturnContent(HttpConstants.InvalidApiViewModel, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
            //return Serializer.ReturnContent(HttpConstants.NotImplemented, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
        }

        public HttpResponseMessage Put(ApiViewModel apiViewModel)
        {
            //_logger.Info("Tenant HttpPut - Called");
            UniversityContext dbContext = null;
            //CurrentUser currentUser = null;
            try
            {
                if (apiViewModel.HasValue())
                {
                    Token = apiViewModel.Token;
                    //currentUser = ApiUser;
                    //_logger.Info("Token : " + Token);
                    dbContext = new UniversityContext();
                    var dbUser = dbContext.SecurityTokens.Include("ApplicationUser").SingleOrDefault(x => x.Token == Token);
                    if (dbUser != null && dbUser.ApplicationUser != null)
                    {
                        //_logger.Info("dbuser has value " + dbUser.ApplicationUserId);
                        if (dbUser.ApplicationUser.AccountType != AccountType.BranchAdmin)
                        {
                            //_logger.Warn("Not a Branch Admin.");
                            return Serializer.ReturnContent("Not a Branch Admin.", this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                        }
                        Tenant serializedTenant = JsonConvert
                        .DeserializeObject<Tenant>(apiViewModel.custom.ToString());
                        if (serializedTenant != null)
                        {
                            var tenant = dbContext.Tenants.SingleOrDefault(x => x.TenantId == serializedTenant.TenantId);
                            if (tenant != null)
                            {
                                tenant.DisplayName = serializedTenant.DisplayName;
                                tenant.LastModifiedBy = dbUser.ApplicationUserId;
                                tenant.LastModifiedOn = DateTime.Now;
                                dbContext.SaveChanges();

                                //_logger.Warn(HttpConstants.Updated);
                                return Serializer.ReturnContent(HttpConstants.Updated, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                            }
                            else
                            {
                                _logger.Warn("Tenant does not exists.");
                                return Serializer.ReturnContent("Tenant does not exists.", this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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
                    _logger.Warn(HttpConstants.InvalidApiViewModel);
                    return Serializer.ReturnContent(HttpConstants.InvalidApiViewModel, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
            //return Serializer.ReturnContent(HttpConstants.NotImplemented, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
        }

        public HttpResponseMessage Delete(ApiViewModel apiViewModel)
        {
            //_logger.Info("Tenant HttpDelete - Called");
            UniversityContext dbContext = null;
            //CurrentUser currentUser = null;
            try
            {
                if (apiViewModel.HasValue())
                {
                    Token = apiViewModel.Token;
                    //currentUser = ApiUser;
                    //_logger.Info("Token : " + Token);
                    dbContext = new UniversityContext();
                    var dbUser = dbContext.SecurityTokens.Include("ApplicationUser").SingleOrDefault(x => x.Token == Token);
                    if (dbUser != null && dbUser.ApplicationUser != null)
                    {
                        //_logger.Info("dbuser has value " + dbUser.ApplicationUserId);
                        if (dbUser.ApplicationUser.AccountType != AccountType.BranchAdmin)
                        {
                            //_logger.Warn("Not a Branch Admin.");
                            return Serializer.ReturnContent("Not a Branch Admin.", this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                        }
                        Tenant serializedTenant = JsonConvert
                        .DeserializeObject<Tenant>(apiViewModel.custom.ToString());
                        if (serializedTenant != null)
                        {
                            var tenant = dbContext.Tenants.SingleOrDefault(x => x.TenantId == serializedTenant.TenantId);
                            if (tenant != null)
                            {
                                tenant.StatusCode = StatusCodeConstants.INACTIVE;
                                tenant.LastModifiedBy = dbUser.ApplicationUserId;
                                tenant.LastModifiedOn = DateTime.Now;
                                dbContext.SaveChanges();

                                //_logger.Warn(HttpConstants.Deleted);
                                return Serializer.ReturnContent(HttpConstants.Deleted, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                            }
                            else
                            {
                                _logger.Warn("Tenant does not exists.");
                                return Serializer.ReturnContent("Tenant does not exists.", this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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
                    _logger.Warn(HttpConstants.InvalidApiViewModel);
                    return Serializer.ReturnContent(HttpConstants.InvalidApiViewModel, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
            //return Serializer.ReturnContent(HttpConstants.NotImplemented, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
        }

    }
}
