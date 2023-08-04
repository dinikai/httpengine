using System;
using System.Net;

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
        Layout layout;

        public HttpApplication(Router router, string host, Layout layout)
        {
            listener = new HttpListener();
            listener.Prefixes.Add(host);

            this.Router = router;
            this.layout = layout;
        }

        public IModel UseModel(IModel model)
        {
            model.PublicDirectory ??= Router.PublicDirectory;
            model.Error404 ??= Router.Error404;
            model.Layout ??= layout;
            model.Application = this;

            model.OnUse();
            Router.Models.Insert(0, model);

            return model;
        }

        public IModel UseModel<T>() where T : IModel, new()
        {
            T model = new();
            model.PublicDirectory ??= Router.PublicDirectory;
            model.Error404 ??= Router.Error404;
            model.Layout ??= layout;
            model.Application = this;

            model.OnUse();
            Router.Models.Insert(0, model);

            return model;
        }

        public void RemoveModel(IModel model)
        {
            Router.Models.Remove(model);
        }

        public void RemoveAll(Predicate<IModel> predicate)
        {
            Router.Models.RemoveAll(x => predicate(x));
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
                output.Write(routerResponse.PageBuffer);
                output.Flush();
                
                listener.Stop();

                // Всякие выводы в консоль
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write($"#{++totalRequests}: {context.Request.Url!.ToString()}");
            }
        }
    }
}
