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
    public class BroadcastController : UnSecuredController
    {        
        public HttpResponseMessage Get([FromUri]CurrentUser currentUser)
        {
            _logger.Info("BroadCast HttpGet - Called");
            UniversityContext dbcontext = null;
            List<Broadcast_vm> lstBroadCast = null;
            int apiDataLogId = 0;
            try
            {
                if (currentUser.HasValue())
                {
                    dbcontext = new UniversityContext();
                    lstBroadCast = (from bC in dbContext.BroadCasts
                                   .Where(x =>
                                       x.StatusCode == StatusCodeConstants.ACTIVE
                                       && x.TenantId == currentUser.TenantId)
                                    select new Broadcast_vm
                                    {
                                        BroadCastId = bC.BroadCastId,
                                        ClassId = bC.ClassDetailId.Value,
                                        Sub = bC.Sub,
                                        Message = bC.Message,
                                        Path_Picture = bC.Path_Picture,
                                        Path_Doc = bC.Path_Doc,
                                        Path_Video = bC.Path_Video,
                                        Path_Voice = bC.Path_Voice
                                    }).ToList();
                }
                else
                    _logger.Warn("current user not satisfied");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                ErrorLog.LogCustomError(currentUser, ex, apiDataLogId);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
            return Serializer.ReturnContent(lstBroadCast, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);

        }

        /// <summary>
        /// Add - Broadcast
        /// </summary>
        /// <param name="apiViewModel"></param>
        /// <returns></returns>
        public HttpResponseMessage Post(ApiViewModel apiViewModel)
        {
            _logger.Info("BroadCast HttpPost - Called");

            CurrentUser currentUser = null;

            int apiDataLogId = 0;
            Tenant tenant = null;
            string data = string.Empty;
            try
            {
                tenant = CurrentTenant;
                if (apiViewModel.HasValue() && tenant != null)
                {
                    currentUser = ApiUser;
                    var user = dbContext.ApplicationUsers.SingleOrDefault(x => x.ApplicationUserId == currentUser.UserId);
                    if (user != null)
                    {
                        data = JsonConvert.SerializeObject(apiViewModel.custom);
                        apiDataLogId = DataLog
                            .LogData(currentUser, VerbConstants.Post, "BroadCast", data);
                        Broadcast_vm serializedBroadcast = JsonConvert
                            .DeserializeObject<Broadcast_vm>(apiViewModel.custom.ToString());
                        if (serializedBroadcast != null)
                        {
                            
                            dbContext = new UniversityContext();
                            BroadCast broadcast = new BroadCast
                            {
                                ApplicationUserId = currentUser.UserId,
                                ClassDetailId = serializedBroadcast.ClassId,
                                Sub = serializedBroadcast.Sub,
                                Message = serializedBroadcast.Message,
                                Path_Picture = serializedBroadcast.Path_Picture,
                                Path_Doc = serializedBroadcast.Path_Doc,
                                Path_Video = serializedBroadcast.Path_Video,
                                Path_Voice = serializedBroadcast.Path_Voice,
                                CreatedBy = currentUser.UserId,
                                CreatedOn = DateTime.Now,
                                TenantId = currentUser.TenantId,
                                StatusCode = StatusCodeConstants.ACTIVE,
                                Language = Language.English
                            };

                            dbContext.BroadCasts.Add(broadcast);
                            dbContext.SaveChanges();
                            return Serializer.ReturnContent(HttpStatusCode.Created
                                , this.Configuration.Services.GetContentNegotiator()
                                , this.Configuration.Formatters, this.Request);
                        }
                        else
                            _logger.Warn("serialized user is empty");
                    }
                    else
                        _logger.Warn("dbuser is empty");
                }
                else
                    _logger.Warn("apiViewModel is not valid");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                ErrorLog.LogCustomError(currentUser, ex, apiDataLogId);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
            return Serializer.ReturnContent(HttpStatusCode.NotImplemented
                            , this.Configuration.Services.GetContentNegotiator()
                            , this.Configuration.Formatters, this.Request);
        }
    }
}
