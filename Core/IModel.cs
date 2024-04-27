using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;

namespace HttpEngine.Core
{
    public interface IModel
    {
        List<string> Routes { get; set; }
        IModel Error404 { get; set; }
        HttpApplication Application { get; set; }

        ModelResult OnRequest(ModelRequest request);
        ModelResult? CallModel<T>(ModelRequest request) where T : IModel;
        void OnUse();
    }

    /// <summary>
    /// Represents a model request in the HTTP engine.
    /// </summary>
    public class ModelRequest
    {
        /// <summary>
        /// Gets the request arguments.
        /// </summary>
        public RequestArguments Arguments { get; }

        /// <summary>
        /// Gets the URL routes.
        /// </summary>
        public string[] UrlRoutes { get; }

        /// <summary>
        /// Gets the URL without GET parameters.
        /// </summary>
        public string Url { get; }

        /// <summary>
        /// Gets the raw URL.
        /// </summary>
        public string RawUrl { get; }

        /// <summary>
        /// Gets the HTTP method.
        /// </summary>
        public HttpMethod Method { get; }

        /// <summary>
        /// Gets the request handler.
        /// </summary>
        public string? Handler { get; }

        /// <summary>
        /// Gets the request cookies.
        /// </summary>
        public CookieCollection RequestCookies { get; }

        /// <summary>
        /// Gets the response cookies.
        /// </summary>
        public CookieCollection ResponseCookies { get; }

        /// <summary>
        /// Gets the request headers.
        /// </summary>
        public NameValueCollection Headers { get; }

        /// <summary>
        /// Gets the route that request mathed.
        /// </summary>
        public string Route { get; }

        /// <summary>
        /// Gets the client IP address.
        /// </summary>
        public IPAddress ClientAddress { get; }

        public ModelRequest(RequestArguments arguments, string[] urlRoutes, string url, string rawUrl, HttpMethod method, string? handler,
            CookieCollection requestCookies, CookieCollection responseCookies, NameValueCollection headers, string route, IPAddress clientAddress)
        {
            Arguments = arguments;
            UrlRoutes = urlRoutes;
            Url = url;
            RawUrl = rawUrl;
            Method = method;
            Handler = handler;
            RequestCookies = requestCookies;
            ResponseCookies = responseCookies;
            Headers = headers;
            Route = route;
            ClientAddress = clientAddress;
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
