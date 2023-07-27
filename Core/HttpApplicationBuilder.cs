﻿namespace HttpEngine.Core
{
    public class HttpApplicationBuilder
    {
        Router? router;
        string? host;
        IModel? layout;

        public HttpApplicationBuilder()
        {
        }
        public HttpApplicationBuilder(HttpApplicationBuilderOptions options)
        {
            router = options.Router;
            host = options.Host;
            layout = options.Layout;
        }

        public HttpApplication Build()
        {
            string host = this.host ?? "http://localhost:8888/";
            string publicDirectory = $@"{Environment.CurrentDirectory}/Public";
            IModel layout = this.layout ?? new LayoutModel()
            {
                PublicDirectory = publicDirectory,
                UseLayout = false,
            };

            if (router == null)
            {
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

            var application = new HttpApplication(router, host, layout);
            return application;
        }
    }
}
