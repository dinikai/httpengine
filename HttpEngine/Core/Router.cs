using System.Net;

namespace HttpEngine.Core
{
    /// <summary>
    /// Если модель есть в словаре, то вызывает ее, если нет, пытается отправить сырой файл, или выдает 404
    /// </summary>
    public class Router
    {
        /// <summary>
        /// Директория с публичными файлами, доступными по интернету
        /// </summary>
        public string PublicDirectory { get; set; }
        /// <summary>
        /// Страница-заглушка при 404
        /// </summary>
        public IModel Error404Page { get; set; }
        /// <summary>
        /// Все пути к моделям через URL
        /// </summary>
        public Dictionary<string, IModel> Routes { get; set; }

        public Router(string publicDirectory, Dictionary<string, IModel> routes, IModel error404Page)
        {
            PublicDirectory = publicDirectory;
            Routes = routes;
            Error404Page = error404Page;
        }

        public RouterResponse Route(HttpListenerContext context)
        {
            string rawUrlWithoutArgs = context.Request.RawUrl!.Split("?")[0]; // URL без GET-аргументов
            List<string> urlRoutes = rawUrlWithoutArgs.Split("/").ToList();
            urlRoutes.RemoveAt(0);

            Dictionary<string, string> args; // Извлекаем GET-аргументы
            if (context.Request.HttpMethod == "GET") args = context.Request.QueryString.ToDictionary();
            else
            {
                using (var reader = new StreamReader(context.Request.InputStream))
                {
                    args = Extensions.GetPostParams(reader.ReadToEnd());
                }
            }

            byte[] buffer;
            bool publicFile, containsRoute = Routes.ContainsKey("/" + urlRoutes[0]), fileExists = true;
            string pagePath;
            ModelResponse modelResponse = new();
            // Если такой путь к модели существует,
            if (containsRoute)
            {
                // то вызываем модель и слепливаем путь к файлу, который потом отправим
                var modelRequest = new ModelRequest(args, urlRoutes.ToArray(), context.Request.HttpMethod);
                modelResponse = Routes["/" + urlRoutes[0]].OnRequest(modelRequest);
                pagePath = @$"{PublicDirectory}/{modelResponse.ResponseFile}";
                publicFile = false;
            } else
            {
                // Иначе пытаемся найти сырой файл
                pagePath = @$"{PublicDirectory}{rawUrlWithoutArgs}";
                publicFile = true;
            }

            int statusCode = 200;
            if (!File.Exists(pagePath))
            {
                // Выбрасываем страницу с ошибкой 404 и ставим соответствующий код статуса
                var modelRequest = new ModelRequest(args, urlRoutes.ToArray(), context.Request.HttpMethod);
                modelResponse = Error404Page.OnRequest(modelRequest);
                pagePath = @$"{PublicDirectory}/{modelResponse.ResponseFile}";
                statusCode = 404;
                fileExists = false;
            }
            // Читаем страницу, сырой файл или страницу с ошибкой
            FileStream file = new FileStream(pagePath, FileMode.Open);
            buffer = new byte[file.Length];
            file.Read(buffer, 0, buffer.Length);
            file.Close();

            // Обрабатываем исходную HTML-страницу, заменяя метки (@tag) на значения, вышедшие из модели
            if (containsRoute || !fileExists)
                buffer = ViewParser.Parse(buffer, modelResponse.ViewData);

            // Компонуем и возвращаем ответ
            return new RouterResponse()
            {
                UrlRoutes = urlRoutes.ToArray(),
                PageBuffer = buffer,
                Arguments = args,
                PublicFile = publicFile,
                Path = pagePath,
                StatusCode = statusCode
            };
        }
    }

    /// <summary>
    /// Структура, содержащая ответ от роутера (страница, которую надо показать, GET-аргументы, код статуса ответа и т.д.)
    /// </summary>
    public struct RouterResponse
    {
        public string[] UrlRoutes { get; set; }
        public byte[] PageBuffer { get; set; }
        public Dictionary<string, string> Arguments { get; set; }
        public bool PublicFile { get; set; }
        public string Path { get; set; }
        public int StatusCode { get; set; }
    }
}
