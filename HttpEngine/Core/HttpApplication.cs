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
        public HttpListener Listener { get; set; }
        /// <summary>
        /// Объект роутера
        /// </summary>
        public Router Router { get; set; }

        public HttpApplication(Router router)
        {
            Listener = new HttpListener();

            Router = router;
        }

        public void UseModel(IModel model)
        {
            model.PublicDirectory = Router.PublicDirectory;
            Router.Models.Add(model);
        }

        public void UseModel<T>() where T : IModel, new()
        {
            T model = new()
            {
                PublicDirectory = Router.PublicDirectory
            };
            Router.Models.Add(model);
        }

        /// <summary>
        /// Запускает приложение
        /// </summary>
        public void Run()
        {
            Console.WriteLine($"Server started at {Listener.Prefixes.ToArray()[0]}\nListening for connections...");
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
                Console.WriteLine("Got request: " + context.Request.Url!.ToString());
                Console.WriteLine("\twith code " + routerResponse.StatusCode.ToString());
                Console.WriteLine("\tRaw URL: " + context.Request.RawUrl);
                Console.Write("\tQuery parameters: ");
                List<string> args = new List<string>();
                foreach (var parameter in routerResponse.Arguments)
                    args.Add($"{parameter.Key}={parameter.Value}");
                Console.WriteLine(string.Join(", ", args));
                Console.WriteLine();
            }
        }
    }

    public class ApplicationEventArgs : EventArgs
    {
        public ApplicationEventArgs()
        {

        }
    }
}
