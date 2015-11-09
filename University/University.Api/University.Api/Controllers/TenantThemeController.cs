using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using University.Api.Controllers.Serialize;
using University.Common.Models;
using University.Common.Models.Enums;
using University.Constants;
using University.Context;
using University.Security.Models.ViewModel;
using University.Api.Extensions;
using University.Utilities;
using University.Common.Models.Security;

namespace University.Api.Controllers
{
    public class TenantThemeController : UnSecuredController
    {
        public HttpResponseMessage Put(ApiViewModel apiViewModel)
        {
            //_logger.Info("TenantTheme HttpPut - Called");
            UniversityContext dbContext = null;
            Tenant tenant = null;
            CurrentUser currentUser = null;
            try
            {
                tenant = CurrentTenant;
                if (tenant.HasValue())
                {
                    Token = apiViewModel.Token;
                    currentUser = ApiUser;
                    if (currentUser.HasValue())
                    {
                        //_logger.Info("cuser : " + currentUser.UserId);
                        Tenant serializedTenant = JsonConvert.DeserializeObject<Tenant>(apiViewModel.custom.ToString());
                        if (serializedTenant != null)
                        {
                            if (currentUser.AccountType != AccountType.Admin)
                            {
                                _logger.Warn(HttpConstants.NotAnAdminUser);
                                return Serializer.ReturnContent(HttpConstants.NotAnAdminUser, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                            }
                            dbContext = new UniversityContext();
                            var tenantToUpdate = dbContext.Tenants.SingleOrDefault(x => x.TenantId == tenant.TenantId);
                            if (tenantToUpdate != null)
                            {
                                tenantToUpdate.CSSField1 = serializedTenant.CSSField1;
                                tenantToUpdate.CSSField2 = serializedTenant.CSSField2;
                                tenantToUpdate.CSSField3 = serializedTenant.CSSField3;
                                tenantToUpdate.CSSField4 = serializedTenant.CSSField4;
                                tenantToUpdate.CSSField5 = serializedTenant.CSSField5;
                                tenantToUpdate.LastModifiedBy = currentUser.UserId;
                                tenantToUpdate.LastModifiedOn = DateTime.Now;
                                dbContext.SaveChanges();
                                //_logger.Warn(HttpConstants.Updated);
                                return Serializer.ReturnContent(HttpConstants.Updated, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                            }
                            else
                            {
                                _logger.Warn("Tenant does not exists");
                                return Serializer.ReturnContent("Tenant does not exists", this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
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
                    _logger.Warn("Tenant does not exists.");
                    return Serializer.ReturnContent("Tenant does not exists.", this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }
    }
}
