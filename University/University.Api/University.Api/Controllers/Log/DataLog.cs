using System;
using University.Api.Extensions;
using University.Common.Models.Enums;
using University.Common.Models.Log;
using University.Common.Models.Security;
using University.Constants;
using University.Context;

namespace University.Api.Controllers.Log
{
    public class DataLog : SecuredController
    {
        public static int LogData(CurrentUser currentUser, string verbName, string controllerName, string data)
        {
            int apiDataLogId = 0;
            try
            {
                if (currentUser.HasValue() && !string.IsNullOrEmpty(data))
                {
                    ApiDataLog dataLog = new ApiDataLog
                    {
                        DataLog = data,
                        VerbName = verbName,
                        ControllerName = controllerName,
                        StatusCode = StatusCodeConstants.ACTIVE,
                        CreatedBy = currentUser.UserId,
                        CreatedOn = DateTime.Now,
                        TenantId = currentUser.TenantId,
                        Language = Language.English
                    };
                    UniversityContext context = new UniversityContext();
                    context.ApiDataLogs.Add(dataLog);
                    context.SaveChanges();
                    apiDataLogId = dataLog.ApiDataLogId;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return apiDataLogId;
        }
    }
}