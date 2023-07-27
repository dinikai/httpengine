using System.IO;
using System.Net;
using System.Text;

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
        /// Список моделей
        /// </summary>
        public List<IModel> Models { get; set; }

        public Router(string publicDirectory, IModel error404Page)
        {
            PublicDirectory = publicDirectory;
            Error404Page = error404Page;
            Models = new List<IModel>();
        }

        public RouterResponse Route(HttpListenerContext context)
        {
            string rawUrlWithoutArgs = "/" + context.Request.RawUrl!.Split("?")[0].Trim('/'); // URL без GET-аргументов
            List<string> urlRoutes = rawUrlWithoutArgs.Split("/").ToList();
            urlRoutes.RemoveAll(x => x == "");

            bool publicFile;
            IModel? model = null;
            foreach (IModel modelEach in Models)
            {
                foreach (string routeEach in modelEach.Routes)
                {
                    if (routeEach == rawUrlWithoutArgs) model = modelEach;
                }
            }
            byte[] viewData;
            ModelResponse modelResponse;

            HttpMethod method;
            switch (context.Request.HttpMethod)
            {
                case "GET":
                    method = HttpMethod.Get;
                    break;
                case "POST":
                    method = HttpMethod.Post;
                    break;
                default:
                    method = HttpMethod.Get;
                    break;
            }

            RequestArguments arguments;
            if (method == HttpMethod.Get) arguments = new RequestArguments(context.Request.QueryString.ToDictionary());
            else
            {
                using var reader = new StreamReader(context.Request.InputStream);
                arguments = new MultipartRequestArguments(Extensions.GetPostParams(reader.ReadToEnd()), new());
            }

            int statusCode = 200;
            // Если модель существует,
            if (model != null)
            {
                // то вызываем модель и слепливаем путь к файлу, который потом отправим
                var modelRequest = new ModelRequest(arguments, urlRoutes.ToArray(), method, context.Request.Cookies, context.Response.Cookies);
                if (arguments.Arguments.ContainsKey("handler")) modelRequest.Handler = arguments.Arguments["handler"];
                else modelRequest.Handler = "";

                modelResponse = model.OnRequest(modelRequest);
                viewData = modelResponse.ResponseData;
                publicFile = false;
            } else
            {
                // Иначе пытаемся найти сырой файл
                string path = @$"{PublicDirectory}{rawUrlWithoutArgs}";
                publicFile = true;

                if (File.Exists(path))
                {
                    FileStream file = new FileStream(path, FileMode.Open);
                    viewData = new byte[file.Length];
                    file.Read(viewData, 0, viewData.Length);
                    file.Close();
                } else
                {
                    // Выбрасываем страницу с ошибкой 404 и ставим соответствующий код статуса
                    var modelRequest = new ModelRequest(arguments, urlRoutes.ToArray(), method, context.Request.Cookies, context.Response.Cookies);
                    modelResponse = Error404Page.OnRequest(modelRequest);
                    viewData = modelResponse.ResponseData;
                    statusCode = 404;
                }
            }

            // Компонуем и возвращаем ответ
            return new RouterResponse()
            {
                UrlRoutes = urlRoutes.ToArray(),
                PageBuffer = viewData,
                Arguments = arguments,
                PublicFile = publicFile,
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
        public RequestArguments Arguments { get; set; }
        public bool PublicFile { get; set; }
        public int StatusCode { get; set; }
    }
}
