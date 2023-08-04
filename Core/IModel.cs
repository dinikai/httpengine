using System;
using System.Net;

namespace HttpEngine.Core
{
    public interface IModel
    {
        List<string> Routes { get; set; }
        string PublicDirectory { get; set; }
        IModel Error404 { get; set; }
        Layout Layout { get; set; }
        HttpApplication Application { get; set; }

        ModelResponse OnRequest(ModelRequest request);
        ModelResponse? CallModel<T>(ModelRequest request) where T : IModel;
        ModelResponse Redirect(string url);
        void OnUse();
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
        public WebHeaderCollection Headers { get; set; }
        public int StatusCode { get; set; }

        public ModelResponse()
        {
            Headers = new();
            StatusCode = -1;
        }
        public ModelResponse(byte[] responseData) : this() => ResponseData = responseData;
    }
}
