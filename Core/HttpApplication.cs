using HttpEngine.Caching;
using System;
using System.Buffers.Text;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace HttpEngine.Core
{
    /// <summary>
    /// Класс, представляющий все приложение
    /// </summary>
    public class HttpApplication
    {
        /// <summary>
        /// Gets the router for handling incoming requests.
        /// </summary>
        public Router Router { get; }

        /// <summary>
        /// Gets or sets the cache control strategy for the application.
        /// </summary>
        public CacheControl CacheControl { get; set; }

        /// <summary>
        /// Gets or sets the layout for rendering views.
        /// </summary>
        public Layout? Layout { get; set; }

        /// <summary>
        /// Gets or sets the content encoding for responses.
        /// </summary>
        public Encoding ContentEncoding { get; set; }

        /// <summary>
        /// Gets the list of registered views.
        /// </summary>
        public List<View> Views { get; set; } = [];

        private List<CachedResource> cachedResources = [];
        private bool resourceCaching;
        private HttpListener listener;
        private List<Middleware> middlewares = [];

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpApplication"/> class.
        /// </summary>
        /// <param name="router">The router for handling requests.</param>
        /// <param name="hosts">The host addresses to listen on.</param>
        /// <param name="layout">The layout for rendering views.</param>
        /// <param name="cacheControl">The cache control strategy.</param>
        /// <param name="contentEncoding">The content encoding for responses.</param>
        public HttpApplication(Router router, string[] hosts, CacheControl cacheControl, Encoding contentEncoding, bool resourceCaching)
        {
            this.resourceCaching = resourceCaching;
            listener = new HttpListener();
            foreach (string host in hosts)
                listener.Prefixes.Add(host);

            Router = router;
            CacheControl = cacheControl;
            ContentEncoding = contentEncoding;

            if (resourceCaching)
            {
                CacheResources();
            }
        }

        /// <summary>
        /// Adds a model to handle requests matching specific routes.
        /// </summary>
        public Model UseModel(Model model)
        {
            model.Application = this;

            Router.Models.Insert(0, model);
            model.OnUse();

            Router.Error404 = model;

            return model;
        }

        /// <summary>
        /// Adds a model of type <typeparamref name="T"/> to handle requests matching specific routes.
        /// </summary>
        public Model UseModel<T>() where T : Model, new()
        {
            T model = new();
            model.Middlewares.AddRange(middlewares);
            return UseModel(model);
        }

        /// <summary>
        /// Removes a model from handling requests.
        /// </summary>
        public void RemoveModel(IModel model)
        {
            Router.Models.Remove(model);
        }

        /// <summary>
        /// Removes all models that match the specified predicate.
        /// </summary>
        public void RemoveAll(Predicate<IModel> predicate)
        {
            Router.Models.RemoveAll(x => predicate(x));
        }

        /// <summary>
        /// Maps a route to a handler function.
        /// </summary>
        public void Map(string route, Func<ModelRequest, ModelResult> func)
        {
            Router.Maps.Insert(0, new Map(null, route, func));
        }

        /// <summary>
        /// Maps a route to a handler function for HTTP GET method.
        /// </summary>
        public void MapGet(string route, Func<ModelRequest, ModelResult> func)
        {
            Router.Maps.Insert(0, new Map(HttpMethod.Get, route, func));
        }

        /// <summary>
        /// Maps a route to a handler function for HTTP POST method.
        /// </summary>
        public void MapPost(string route, Func<ModelRequest, ModelResult> func)
        {
            Router.Maps.Insert(0, new Map(HttpMethod.Post, route, func));
        }

        /// <summary>
        /// Adds a view of type <typeparamref name="T"/>.
        /// </summary>
        public void View<T>() where T : View, new()
        {
            View(new T());
        }

        /// <summary>
        /// Adds the specified view.
        /// </summary>
        public void View(View view)
        {
            view.Application ??= this;
            view.Layout ??= Layout;
            Views.Add(view);

            view.OnUse();
        }

        /// <summary>
        /// Gets the view of type <typeparamref name="T"/>.
        /// </summary>
        public View? GetView<T>() where T : View
        {
            return Views.FirstOrDefault(x => x is T);
        }

        public void AddMiddleware<T>() where T : Middleware, new()
        {
            foreach (var model in Router.Models)
            {
                model.Middleware<T>();
            }
        }

        /// <summary>
        /// Starts the application and listens for incoming HTTP requests.
        /// </summary>
        public void Run()
        {
            Console.Write($"Server started at ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(string.Join(", ", listener.Prefixes.ToArray()));
            Console.ForegroundColor = ConsoleColor.Gray;

            while (true)
            {
                listener.Start();
                HttpListenerContext context = listener.GetContext();

                new Thread(() => ProcessClient(context)).Start();
            }
        }

        private void ProcessClient(HttpListenerContext context)
        {
            // Маршрутизировать запрос и получить RouterResponse
            var routerResponse = Router.Route(context);

            // Формирование и отправка ответа
            context.Response.ContentLength64 = routerResponse.PageBuffer.Length;
            Stream output = context.Response.OutputStream;
            context.Response.StatusCode = routerResponse.StatusCode;
            context.Response.Headers = routerResponse.Headers;
            context.Response.ContentEncoding = ContentEncoding;

            string cacheControl;
            switch (CacheControl)
            {
                case CacheControl.NoStore:
                    cacheControl = "no-store";
                    break;
                case CacheControl.NoCache:
                    cacheControl = "no-cache";
                    break;
                case CacheControl.Private:
                    cacheControl = "private";
                    break;
                case CacheControl.Public:
                    cacheControl = "public";
                    break;
                default:
                    cacheControl = "public";
                    break;
            }
            context.Response.Headers.Add("Cache-Control", "no-store, no-cache, must-revalidate, max-age=0");
            //context.Response.Headers.Add("Cache-Control", cacheControl + ", max-age=86400");

            context.Response.Headers["Server"] = "HttpEngine/2024.0.3";
            if (routerResponse.ContentType != null)
                context.Response.ContentType = routerResponse.ContentType;
            context.Response.Headers.Add("ETag", $"\"{Convert.ToBase64String(SHA1.HashData(routerResponse.PageBuffer))}\"");

            foreach (var cookie in routerResponse.Cookies)
            {
                Cookie c = (Cookie)cookie;
                context.Response.Headers.Add("Set-Cookie", $"{c.Name}={c.Value}; Expires={c.Expires:r}");
            }

            try
            {
                output.Write(routerResponse.PageBuffer);
                output.Flush();
                output.Close();
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message);
                Console.ForegroundColor = ConsoleColor.Gray;
            }

            // Всякие выводы в консоль
            Console.WriteLine($" {context.Request.HttpMethod} {context.Request.Url}");
        }

        internal byte[]? ReadResource(string fileName)
        {
            if (resourceCaching)
            {
                var resource = cachedResources.FirstOrDefault(x =>  x.FileName == fileName);
                if (resource == null)
                    return Stubs.Stubs.ResourceNotFound(fileName);
                else
                    return resource.Data;
            } else
            {
                if (File.Exists(Path.Combine(Router.ResourcesDirectory, fileName)))
                    return File.ReadAllBytes(Path.Combine(Router.ResourcesDirectory, fileName));
                else
                    return Stubs.Stubs.ResourceNotFound(fileName);
            }
        }

        private void CacheResources()
        {
            Console.WriteLine("Caching resources...");
            foreach (string file in Directory.EnumerateFiles(Router.ResourcesDirectory, "*.*", SearchOption.AllDirectories))
            {
                var relativePath = Path.GetRelativePath(Router.ResourcesDirectory, file).Replace("\\", "/");
                Console.WriteLine($" Caching {relativePath}");
                var resource = new CachedResource(relativePath, File.ReadAllBytes(file));
                cachedResources.Add(resource);
            }
            Console.WriteLine();
        }
    }
}
