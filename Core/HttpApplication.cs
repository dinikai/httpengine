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
        Router router;
        Layout layout;

        public HttpApplication(Router router, string host, Layout layout)
        {
            listener = new HttpListener();
            listener.Prefixes.Add(host);

            this.router = router;
            this.layout = layout;
        }

        public IModel UseModel(IModel model)
        {
            model.PublicDirectory ??= router.PublicDirectory;
            model.Error404 ??= router.Error404Page;
            model.Layout ??= layout;
            model.Application = this;

            model.OnUse();
            router.Models.Add(model);

            return model;
        }

        public IModel UseModel<T>() where T : IModel, new()
        {
            T model = new();
            model.PublicDirectory ??= router.PublicDirectory;
            model.Error404 ??= router.Error404Page;
            model.Layout ??= layout;
            model.Application = this;

            model.OnUse();
            router.Models.Add(model);

            return model;
        }

        public void RemoveModel(IModel model)
        {
            router.Models.Remove(model);
        }

        public void RemoveAll(Predicate<IModel> predicate)
        {
            router.Models.RemoveAll(x => predicate(x));
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
            Console.WriteLine("Listening for connections...");
            while (true)
            {
                listener.Start();
                HttpListenerContext context = listener.GetContext();

                // Маршрутизировать запрос и получить RouterResponse
                var routerResponse = router.Route(context);

                // Формирование и отправка ответа
                context.Response.ContentLength64 = routerResponse.PageBuffer.Length;
                Stream output = context.Response.OutputStream;
                context.Response.StatusCode = routerResponse.StatusCode;
                output.Write(routerResponse.PageBuffer);
                output.Flush();
                
                listener.Stop();

                // Всякие выводы в консоль
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Got request: ");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(context.Request.Url!.ToString());
            }
        }
    }
}
