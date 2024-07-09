using System.Reflection;
using System.Text;

namespace HttpEngine.Core
{
    public static class HttpApplicationBuilder
    {
        public static HttpApplication Build(HttpApplicationBuilderOptions options)
        {
            string[] hosts = options.Hosts ?? ["http://*:8080/"];
            string filesDirectory = $"{Assembly.GetCallingAssembly().GetName().Name}_files";
            string resourcesDirectory = options.ResourcesDirectory ?? $@"{Environment.CurrentDirectory}/{filesDirectory}/resources";
            string publicDirectory = options.PublicDirectory ?? $@"{Environment.CurrentDirectory}/{filesDirectory}/public";
            CacheControl cacheControl = options.CacheControl ?? CacheControl.Public;
            string handler = options.Handler ?? "h";
            Encoding contentEncoding = options.ContentEncoding ?? Encoding.UTF8;
            bool resourceCaching = options.ResourceCaching;

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

            var application = new HttpApplication(options.Router, hosts, cacheControl, contentEncoding, resourceCaching);
            application.Layout = options.Layout ?? new Layout(application);
            return application;
        }
    }
}
