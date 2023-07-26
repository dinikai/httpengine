namespace HttpEngine.Core
{
    public class HttpApplicationBuilder
    {
        Router? router;
        string? ip;
        string? layout;

        public HttpApplicationBuilder()
        {
        }
        public HttpApplicationBuilder(HttpApplicationBuilderOptions options)
        {
            router = options.Router;
            ip = options.Ip;
            layout = options.Layout;
        }

        public HttpApplication Build()
        {
            string ip = this.ip ?? "http://localhost:8888/";
            string layout = this.layout ?? "_Layout.html";

            if (router == null)
            {
                string publicDirectory = $@"{Environment.CurrentDirectory}/Public";
                var error404 = new Model()
                {
                    PublicDirectory = publicDirectory,
                    Layout = layout,
                };

                router = new Router(
                    publicDirectory: publicDirectory,
                    error404Page: error404
                );
            }

            var application = new HttpApplication(router, ip, layout);
            return application;
        }
    }
}
