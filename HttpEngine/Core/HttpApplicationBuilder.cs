using HttpEngine.Models;

namespace HttpEngine.Core
{
    internal class HttpApplicationBuilder
    {
        Router? router;
        string? ip;

        public HttpApplicationBuilder()
        {
        }
        public HttpApplicationBuilder(HttpApplicationBuilderOptions options)
        {
            router = options.Router;
            ip = options.Ip;
        }

        public HttpApplication Build()
        {
            if (router == null)
            {
                string publicDirectory = $@"{Environment.CurrentDirectory}/Public";
                var error404 = new Error404Model()
                {
                    PublicDirectory = publicDirectory
                };

                router = new Router(
                    publicDirectory: publicDirectory,
                    error404Page: error404
                );
            }
            if (ip == null)
            {
                ip = "http://localhost:8888/";
            }

            var application = new HttpApplication(router, ip);
            return application;
        }
    }
}
