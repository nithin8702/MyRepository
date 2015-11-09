using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;

namespace University.Api.Controllers.Serialize
{
    public class Serializer : SecuredController
    {
        public static HttpResponseMessage ReturnContent(object returnObj, IContentNegotiator content, MediaTypeFormatterCollection formatter, HttpRequestMessage request)
        {
            IContentNegotiator negotiator = content;
            ContentNegotiationResult result = null;
            result = negotiator.Negotiate(typeof(object), request, formatter);
            return new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new ObjectContent<object>(returnObj, result.Formatter, result.MediaType.MediaType)
            };
        }
    }
}