using System;

namespace HttpEngine.Core
{
    public interface IModel
    {
        public List<string> Routes { get; set; }
        public string PublicDirectory { get; set; }
        public IModel Error404 { get; set; }

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
        public byte[] ResponseData { get; set; } = Array.Empty<byte>();

        public ModelResponse() { }
        public ModelResponse(byte[] responseData) => ResponseData = responseData;
    }
}
