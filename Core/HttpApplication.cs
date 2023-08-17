using System;
using System.Buffers.Text;
using System.Net;
using System.Security.Cryptography;

namespace HttpEngine.Core
{
    /// <summary>
    /// Класс, представляющий все приложение
    /// </summary>
    public class HttpApplication
    {
        /// <summary>
        /// Главный HttpListener
        /// </summary>
        HttpListener listener;

        /// <summary>
        /// Объект роутера
        /// </summary>
        public Router Router { get; set; }
        public CacheControl CacheControl { get; set; }
        public Layout Layout { get; set; }
        
        public HttpApplication(Router router, string[] hosts, Layout layout, CacheControl cacheControl)
        {
            listener = new HttpListener();
            foreach (string host in hosts)
                listener.Prefixes.Add(host);

            Router = router;
            CacheControl = cacheControl;
            Layout = layout;
        }

        public IModel UseModel(IModel model)
        {
            model.PublicDirectory ??= Router.PublicDirectory;
            model.Error404 ??= Router.Error404;
            model.Layout ??= Layout;
            model.Application = this;

            Router.Models.Insert(0, model);
            model.OnUse();

            return model;
        }

        public IModel UseModel<T>() where T : IModel, new()
        {
            T model = new();
            return UseModel(model);
        }

        public IModel Use404(IModel model)
        {
            model.PublicDirectory ??= Router.PublicDirectory;
            model.Layout ??= Layout;
            model.Application = this;

            Router.Error404 = model;
            model.OnUse();

            return model;
        }

        public IModel Use404<T>() where T : IModel, new()
        {
            T model = new();
            return Use404(model);
        }

        public void RemoveModel(IModel model)
        {
            Router.Models.Remove(model);
        }

        public void RemoveAll(Predicate<IModel> predicate)
        {
            Router.Models.RemoveAll(x => predicate(x));
        }

        public void Map(string route, Func<ModelRequest, ModelResult> func)
        {
            Router.Maps.Insert(0, new Map(null, route, func));
        }

        public void MapGet(string route, Func<ModelRequest, ModelResult> func)
        {
            Router.Maps.Insert(0, new Map(HttpMethod.Get, route, func));
        }

        public void MapPost(string route, Func<ModelRequest, ModelResult> func)
        {
            Router.Maps.Insert(0, new Map(HttpMethod.Post, route, func));
        }

        /// <summary>
        /// Запускает приложение
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

                // Маршрутизировать запрос и получить RouterResponse
                var routerResponse = Router.Route(context);

                // Формирование и отправка ответа
                context.Response.ContentLength64 = routerResponse.PageBuffer.Length;
                Stream output = context.Response.OutputStream;
                context.Response.StatusCode = routerResponse.StatusCode;
                context.Response.Headers = routerResponse.Headers;

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
                context.Response.Headers.Add("Cache-Control", cacheControl);

                context.Response.Headers["Server"] = "HttpEngine/1.0";
                if (routerResponse.ContentType != null)
                    context.Response.ContentType = routerResponse.ContentType;
                context.Response.Headers.Add("ETag", $"\"{Convert.ToBase64String(SHA1.HashData(routerResponse.PageBuffer))}\"");

                try
                {
                    output.Write(routerResponse.PageBuffer);
                    output.Flush();
                    output.Close();
                } catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e.Message);
                    Console.ForegroundColor = ConsoleColor.Gray;
                } finally
                {
                    listener.Stop();
                }

                // Всякие выводы в консоль
                Console.WriteLine($"{context.Request.HttpMethod} {context.Request.Url}");
            }
        }
    }
}
