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
        HttpListener Listener;

        /// <summary>
        /// Объект роутера
        /// </summary>
        Router Router;

        public HttpApplication(Router router, string ip)
        {
            Listener = new HttpListener();
            Listener.Prefixes.Add(ip);

            Router = router;
        }

        public IModel UseModel(IModel model)
        {
            model.PublicDirectory = Router.PublicDirectory;
            model.Error404 = Router.Error404Page;
            Router.Models.Add(model);

            return model;
        }

        public IModel UseModel<T>() where T : IModel, new()
        {
            T model = new()
            {
                PublicDirectory = Router.PublicDirectory,
                Error404 = Router.Error404Page,
            };
            Router.Models.Add(model);

            return model;
        }

        /// <summary>
        /// Запускает приложение
        /// </summary>
        public void Run()
        {
            Console.Write($"Server started at ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{Listener.Prefixes.ToArray()[0]}");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Listening for connections...");
            while (true)
            {
                Listener.Start();
                HttpListenerContext context = Listener.GetContext();

                // Обработать запрос роутером и получить RouterResponse
                var routerResponse = Router.Route(context);

                // Формирование и отправка ответа
                context.Response.ContentLength64 = routerResponse.PageBuffer.Length;
                Stream output = context.Response.OutputStream;
                context.Response.StatusCode = routerResponse.StatusCode;
                output.Write(routerResponse.PageBuffer);
                output.Flush();

                Listener.Stop();

                // Всякие выводы в консоль
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Got request: ");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(context.Request.Url!.ToString());
            }
        }
    }
}
