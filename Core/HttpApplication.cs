﻿using System;
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
        Layout layout;

        public HttpApplication(Router router, string host, Layout layout, CacheControl cacheControl)
        {
            listener = new HttpListener();
            listener.Prefixes.Add(host);

            Router = router;
            CacheControl = cacheControl;
            this.layout = layout;
        }

        public IModel UseModel(IModel model)
        {
            model.PublicDirectory ??= Router.PublicDirectory;
            model.Error404 ??= Router.Error404;
            model.Layout ??= layout;
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
            model.Layout ??= layout;
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

        public void MapGet(string route, Func<ModelRequest, ModelResponse> func)
        {
            Router.Maps.Insert(0, new Map(HttpMethod.Get, route, func));
        }

        public void MapPost(string route, Func<ModelRequest, ModelResponse> func)
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
            Console.WriteLine($"{listener.Prefixes.ToArray()[0]}");
            Console.ForegroundColor = ConsoleColor.Gray;

            long totalRequests = 0;
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
                output.Write(routerResponse.PageBuffer);
                output.Flush();
                
                listener.Stop();

                // Всякие выводы в консоль
                Console.WriteLine($"#{++totalRequests}: {context.Request.Url}");
            }
        }
    }
}
