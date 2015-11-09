using System;
using University.Common.Models.Enums;
using University.Common.Models.Log;
using University.Common.Models.Security;
using University.Constants;
using University.Context;

namespace University.Api.Controllers.Log
{
    public class ErrorLog : SecuredController
    {
        public static void LogCustomError(CurrentUser currentUser, Exception ex, int apiDataLogId=1)
        {
            try
            {
                string message = (ex.InnerException != null) ? ((ex.InnerException.InnerException != null) ? ex.InnerException.InnerException.Message : ex.InnerException.ToString()) : ex.Message;
                string stackTrace = ex.StackTrace;
                if (apiDataLogId==0)
                {
                    apiDataLogId = 1;
                }
                if (!string.IsNullOrEmpty(message))
                {                    
                    ApiErrorLog errorLog = new ApiErrorLog 
                    { 
                        ErrorDetail = message, StackTrace=stackTrace, ApiDataLogId=apiDataLogId,
                        StatusCode = StatusCodeConstants.ACTIVE, Language = Language.English,
                        CreatedBy = currentUser.UserId, CreatedOn = DateTime.Now, TenantId = currentUser.TenantId, 
                    };
                    UniversityContext context = new UniversityContext();
                    context.ApiErrorLogs.Add(errorLog);
                    context.SaveChanges();
                }
            }
            catch (Exception ex2)
            {
            }
        }
    }
}