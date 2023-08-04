namespace HttpEngine.Core
{
    public class HttpApplicationBuilder
    {
        Router? router;
        string? host;
        Layout? layout;
        string? publicDirectory, staticDirectory;

        public HttpApplicationBuilder()
        {
        }
        public HttpApplicationBuilder(HttpApplicationBuilderOptions options)
        {
            router = options.Router;
            host = options.Host;
            layout = options.Layout;
            publicDirectory = options.PublicDirectory;
            staticDirectory = options.StaticDirectory;
        }

        public HttpApplication Build()
        {
            string host = this.host ?? "http://localhost:8888/";
            string publicDirectory = this.publicDirectory ?? $@"{Environment.CurrentDirectory}/Public";
            string staticDirectory = this.staticDirectory ?? $@"{Environment.CurrentDirectory}/Static";
            Layout layout = this.layout ?? new Layout(publicDirectory);

            if (!Directory.Exists(publicDirectory))
                Directory.CreateDirectory(publicDirectory);
            if (!Directory.Exists(staticDirectory))
                Directory.CreateDirectory(staticDirectory);

            if (router == null)
            {
                var error404 = new Model()
                {
                    PublicDirectory = publicDirectory,
                    Layout = layout,
                };

                router = new Router(
                    publicDirectory: publicDirectory,
                    staticDirectory: staticDirectory,
                    error404: error404
                );
            }

            var application = new HttpApplication(router, host, layout);
            return application;
        }
    }
}
