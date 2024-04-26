using System.Text;

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
            string[] hosts = options.Hosts ?? ["http://localhost:8080/"];
            string resourcesDirectory = options.ResourcesDirectory ?? $@"{Environment.CurrentDirectory}/resources";
            string publicDirectory = options.PublicDirectory ?? $@"{Environment.CurrentDirectory}/public";
            Layout layout = options.Layout ?? new Layout();
            layout.ResourcesDirectory = resourcesDirectory;
            CacheControl cacheControl = options.CacheControl ?? CacheControl.Public;
            string handler = options.Handler ?? "h";
            Encoding contentEncoding = options.ContentEncoding ?? Encoding.UTF8;

            if (!Directory.Exists(resourcesDirectory))
                Directory.CreateDirectory(resourcesDirectory);
            if (!Directory.Exists(publicDirectory))
                Directory.CreateDirectory(publicDirectory);

            if (options.Router == null)
            {
                var error404 = new Model();

                options.Router = new Router(
                    resourcesDirectory: resourcesDirectory,
                    publicDirectory: publicDirectory,
                    error404: error404,
                    handler: handler
                );
            }

            var application = new HttpApplication(options.Router, hosts, layout, cacheControl, contentEncoding);
            return application;
        }
    }
}
