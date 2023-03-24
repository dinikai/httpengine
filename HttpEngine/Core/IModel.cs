using System;

namespace HttpEngine.Core
{
    public interface IModel
    {
        public ModelResponse OnRequest(ModelRequest request);
    }

    public class ModelRequest
    {
        public Dictionary<string, string> Arguments { get; set; }
        public string[] UrlRoutes { get; set; }
        public string Method { get; set; }

        public ModelRequest(Dictionary<string, string> arguments, string[] urlRoutes, string method)
        {
            Arguments = arguments;
            UrlRoutes = urlRoutes;
            Method = method;
        }
    }

    public class ModelResponse
    {
        public string ResponseFile { get; set; } = string.Empty;
        public Dictionary<string, object> ViewData { get; set; } = new();

        public ModelResponse() { }

        public ModelResponse(string responseFile) => ResponseFile = responseFile;

        public ModelResponse(Dictionary<string, object> viewData) => ViewData = viewData;
    }
}
