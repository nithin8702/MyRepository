using log4net;
using System.Web.Http.Controllers;
using System.Web.Http.Cors;
using System.Web.Http.Filters;

namespace University.Api.Filters
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UnSecuredApiController : AuthorizationFilterAttribute
    {
        #region Log4net

        public ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion
    }
}
