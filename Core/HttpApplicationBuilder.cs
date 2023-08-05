namespace HttpEngine.Core
{
    public class HttpApplicationBuilder
    {
        HttpApplicationBuilderOptions options;

        public HttpApplicationBuilder()
        {
            options = new();
        }

        public HttpApplicationBuilder(HttpApplicationBuilderOptions options) : this()
        {
            this.options = options;
        }

        public HttpApplication Build()
        {
            string host = options.Host ?? "http://localhost:8080/";
            string publicDirectory = options.PublicDirectory ?? $@"{Environment.CurrentDirectory}/Public";
            string staticDirectory = options.StaticDirectory ?? $@"{Environment.CurrentDirectory}/Static";
            Layout layout = options.Layout ?? new Layout(publicDirectory);
            CacheControl cacheControl = options.CacheControl ?? CacheControl.Public;

            if (!Directory.Exists(publicDirectory))
                Directory.CreateDirectory(publicDirectory);
            if (!Directory.Exists(staticDirectory))
                Directory.CreateDirectory(staticDirectory);

            if (options.Router == null)
            {
                var error404 = new Model()
                {
                    PublicDirectory = publicDirectory,
                    Layout = layout,
                };

                options.Router = new Router(
                    publicDirectory: publicDirectory,
                    staticDirectory: staticDirectory,
                    error404: error404
                );
            }

            var application = new HttpApplication(options.Router, host, layout, cacheControl);
            return application;
        }
    }
}
