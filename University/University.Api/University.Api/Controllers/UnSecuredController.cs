using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Mvc;
using University.Common.Models;
using University.Common.Models.Security;
using University.Constants;
using University.Context;

namespace University.Api.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UnSecuredController : ApiController
    {
        #region Log4net

        public ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        private Tenant tenant;
        private CurrentUser currentUser;
        public string Token { get; set; }
        public UniversityContext dbContext = new UniversityContext();
        
        public Tenant CurrentTenant
        {
            get 
            {
                //_logger.Info("Tenant Property starts");
                if (tenant==null)
                {
                    var tnt = HttpContext.Current.Request.Headers.Get("Tenant");
                    if (tnt != null)
                    {
                        tenant = JsonConvert.DeserializeObject<Tenant>(tnt.ToString());
                        //_logger.Info("Tenant Id : " + tenant.TenantId);
                    }
                    else
                        _logger.Warn("tenant property is empty");
                }
                //_logger.Warn("Tenant Property ends");
                return tenant; 
            }
        }

        public CurrentUser ApiUser
        {
            get
            {
                //_logger.Info("User Property starts");
                if (CurrentTenant != null)
                {
                    try
                    {
                        //_logger.Info("token : " + Token);
                        var dbToken = dbContext.SecurityTokens.Include("ApplicationUser").SingleOrDefault(x => x.Token == Token
                        && x.StatusCode == StatusCodeConstants.ACTIVE);
                        if (dbToken != null)
                        {
                            //_logger.Info("dbToken has value : " + dbToken.SecurityTokenId);
                            //_logger.Info(dbToken.ApplicationUserId);
                            //_logger.Info(dbToken.ApplicationUser.AccountType);
                            if (dbToken.Expiration < DateTime.Now)
                            {
                                dbToken.Expiration = DateTime.Now.AddMinutes(tenant.SessionTime);
                                dbContext.SaveChanges();
                            }
                            currentUser = new CurrentUser { UserId = dbToken.ApplicationUserId, TenantId = dbToken.TenantId,
                                FullName = dbToken.ApplicationUser.FirstName + " " + dbToken.ApplicationUser.LastName,
                                UserName=dbToken.ApplicationUser.UserName,AccountType=dbToken.ApplicationUser.AccountType };
                        }
                        else
                            _logger.Warn("Not valid Token");
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("Current User Error : " + ex.Message);
                    }
                }
                else
                    _logger.Warn("Empty Tenant");
                //_logger.Info("User Property ends");
                return currentUser;
            }
        }

        





        //public Tenant MapTenant()
        //{
        //    _logger.Info("GetTenant - Starts");
        //    _logger.Info("Request : " + Request.Headers.Host);
        //    string hostName = string.Empty;
        //    UniversityContext dbContext = null;
        //    Tenant tenant = null;
        //    try
        //    {
        //        hostName = Request.Headers.Host;
        //        if (!string.IsNullOrEmpty(hostName))
        //        {
        //            dbContext = new UniversityContext();
        //            tenant = dbContext.Tenants.SingleOrDefault(x => x.HostName == hostName && x.StatusCode == StatusCodeConstants.ACTIVE);
        //        }
        //        else
        //            _logger.Warn("hostName not available");
        //        if (tenant == null)
        //            _logger.Warn("Tenant not available for host : " + hostName);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Error("Tenant Exception :" + ex.Message);
        //    }
        //    finally
        //    {
        //        _logger.Info("GetTenant - Ends");
        //    }
        //    return tenant;
        //    //var test = HttpContext.Current.Request.Headers.Get("Tenant");
        //    //var t = JsonConvert.DeserializeObject<Tenant>(test.ToString());

        //}
    }
}
