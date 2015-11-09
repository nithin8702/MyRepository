using log4net;
using System.Web.Http;

namespace University.Api.Controllers
{
    public class SecuredController : ApiController
    {
        public static ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
