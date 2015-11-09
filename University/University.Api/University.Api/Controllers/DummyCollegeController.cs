using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
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

namespace University.Api.Controllers
{
    public class DummyCollegeController : UnSecuredController
    {

        public HttpResponseMessage Post(ApiViewModel apiViewModel)
        {
            //_logger.Info("DummyCollege HttpPost - Called");
            CurrentUser currentUser = null;
            UniversityContext dbContext = null;
            int apiDataLogId = 0;
            Tenant tenant = null;
            string data = string.Empty;
            DummyCollege college = null;
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
                            apiDataLogId = DataLog.LogData(currentUser, VerbConstants.Post, "DummyCollege", data);

                            DummyCollege serializedCollege = JsonConvert
                                .DeserializeObject<DummyCollege>(apiViewModel.custom.ToString());
                            if (serializedCollege != null)
                            {
                                dbContext = new UniversityContext();
                                college = new DummyCollege
                                {
                                    ApplicationUserId = currentUser.UserId,
                                    CollegeName = serializedCollege.CollegeName,
                                    DepartmentName=serializedCollege.DepartmentName,
                                    Timing=serializedCollege.Timing,
                                    Location=serializedCollege.Location,
                                    ClassRoomNo=serializedCollege.ClassRoomNo,
                                    Link=serializedCollege.Link,
                                    BuildingNo=serializedCollege.BuildingNo,
                                    Notes=serializedCollege.Notes,
                                    CustomField01 = serializedCollege.CustomField01,
                                    CustomField02 = serializedCollege.CustomField02,
                                    CustomField03 = serializedCollege.CustomField03,
                                    CustomField04 = serializedCollege.CustomField04,
                                    CustomField05 = serializedCollege.CustomField05,
                                    CustomField06 = serializedCollege.CustomField06,
                                    CustomField07 = serializedCollege.CustomField07,
                                    CreatedBy = currentUser.UserId,
                                    CreatedOn = DateTime.Now,
                                    TenantId = tenant.TenantId,
                                    StatusCode = StatusCodeConstants.ACTIVE,
                                    Language = Language.English
                                };
                                dbContext.DummyColleges.Add(college);
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
