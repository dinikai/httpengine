using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;

namespace HttpEngine.Core
{
    public interface IModel
    {
        List<string> Routes { get; set; }
        string PublicDirectory { get; set; }
        IModel Error404 { get; set; }
        Layout Layout { get; set; }
        HttpApplication Application { get; set; }

        ModelResult OnRequest(ModelRequest request);
        ModelResult? CallModel<T>(ModelRequest request) where T : IModel;
        void OnUse();
    }

    public class ModelRequest
    {
        public RequestArguments Arguments { get; set; }
        public string[] UrlRoutes { get; set; }
        public string Url { get; set; }
        public string RawUrl { get; set; }
        public HttpMethod Method { get; set; }
        public string? Handler { get; set; }
        public CookieCollection RequestCookies { get; set; }
        public CookieCollection ResponseCookies { get; set; }
        public NameValueCollection Headers { get; set; }
        public string Route { get; set; }

        public ModelRequest(RequestArguments arguments, string[] urlRoutes, string url, string rawUrl, HttpMethod method,
            CookieCollection requestCookies, CookieCollection responseCookies, NameValueCollection headers, string route)
        {
            Arguments = arguments;
            UrlRoutes = urlRoutes;
            Url = url;
            RawUrl = rawUrl;
            Method = method;
            RequestCookies = requestCookies;
            ResponseCookies = responseCookies;
            Headers = headers;
            Route = route;
        }
    }

    public class ModelResult
    {
        public ModelFile File { get; set; }
        public WebHeaderCollection Headers { get; set; }
        public int StatusCode { get; set; }

        public ModelResult()
        {
            File = new(Array.Empty<byte>());
            Headers = new();
            StatusCode = -1;
        }

        public ModelResult(ModelFile file) : this() => File = file;

        public ModelResult(byte[] data) : this() => File = new(data);

        public ModelResult(string text) : this() => File = new(Encoding.UTF8.GetBytes(text));
    }
}
