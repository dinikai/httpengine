﻿using HttpEngine.Models;

namespace HttpEngine.Core
{
    internal class HttpApplicationBuilder
    {
        private Router? Router;

        public HttpApplicationBuilder()
        {
        }
        public HttpApplicationBuilder(HttpApplicationBuilderOptions options)
        {
            Router = options.Router;
        }

        public HttpApplication Build()
        {
            if (Router == null)
            {
                string publicDirectory = $@"{Environment.CurrentDirectory}/Public";
                var error404 = new Error404Model()
                {
                    PublicDirectory = publicDirectory
                };

                Router = new Router(
                    publicDirectory: publicDirectory,
                    error404Page: error404
                );
            }

            var application = new HttpApplication(Router);
            return application;
        }
    }
}
