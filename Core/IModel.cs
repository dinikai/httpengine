using System;
using System.Net;

namespace HttpEngine.Core
{
    public interface IModel
    {
        public List<string> Routes { get; set; }
        public string PublicDirectory { get; set; }
        public IModel Error404 { get; set; }
        public Layout Layout { get; set; }

        public ModelResponse OnRequest(ModelRequest request);
        public void OnUse();
    }

    public class ModelRequest
    {
        public RequestArguments Arguments { get; set; }
        public string[] UrlRoutes { get; set; }
        public HttpMethod Method { get; set; }
        public string? Handler { get; set; }
        public CookieCollection RequestCookies { get; set; }
        public CookieCollection ResponseCookies { get; set; }

        public ModelRequest(RequestArguments arguments, string[] urlRoutes, HttpMethod method, CookieCollection requestCookies, CookieCollection responseCookies)
        {
            Arguments = arguments;
            UrlRoutes = urlRoutes;
            Method = method;
            RequestCookies = requestCookies;
            ResponseCookies = responseCookies;
        }
    }

    public class ModelResponse
    {
        public byte[] ResponseData { get; set; } = Array.Empty<byte>();

        public ModelResponse() { }
        public ModelResponse(byte[] responseData) => ResponseData = responseData;
    }
}
