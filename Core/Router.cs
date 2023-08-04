using System.IO;
using System.Linq.Expressions;
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
        public string StaticDirectory { get; set; }

        /// <summary>
        /// Страница-заглушка при 404
        /// </summary>
        public IModel Error404 { get; set; }

        /// <summary>
        /// Список моделей
        /// </summary>
        public List<IModel> Models { get; set; }

        public Router(string publicDirectory, string staticDirectory, IModel error404)
        {
            PublicDirectory = publicDirectory;
            StaticDirectory = staticDirectory;
            Error404 = error404;
            Models = new List<IModel>();
        }

        public RouterResponse Route(HttpListenerContext context)
        {
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

            string rawUrlWithoutArgs = "/" + context.Request.RawUrl!.Split("?")[0].Trim('/'); // URL без GET-аргументов
            List<string> urlRoutes = rawUrlWithoutArgs.Split("/").ToList();
            urlRoutes.RemoveAll(x => x == "");
            List<(string Name, string Value)> urlArguments = new();

            IModel? model = null;
            foreach (IModel modelEach in Models)
            {
                if (!modelEach.Routes.Any())
                    model = modelEach;

                foreach (string routeEach in modelEach.Routes)
                {
                    bool nextRoute = false;
                    List<(string, string)> eachUrlArguments = new();
                    List<string> modelRoutes = routeEach.Split("/").ToList();
                    modelRoutes.RemoveAll(x => x == "");

                    if (urlRoutes.Count != modelRoutes.Count)
                        continue;

                    for (int i = 0; i < urlRoutes.Count; i++)
                    {
                        if (modelRoutes[i].StartsWith("{") && modelRoutes[i].EndsWith("}"))
                        {
                            eachUrlArguments.Add((modelRoutes[i].Trim('{', '}'), urlRoutes[i]));
                        }
                        else
                        {
                            if (modelRoutes[i] != urlRoutes[i])
                            {
                                nextRoute = true;
                                break;
                            }
                        }
                    }
                    if (nextRoute)
                        continue;
                    urlArguments = eachUrlArguments;
                    model = modelEach;
                }
            }
            for (int i = 0; i < urlArguments.Count; i++)
            {
                if (!arguments.Arguments.ContainsKey(urlArguments[i].Name))
                    arguments.Arguments.Add(urlArguments[i].Name, urlArguments[i].Value);
            }

            byte[] viewData;
            ModelResponse modelResponse;

            int statusCode = 200;
            WebHeaderCollection headers = new();
            bool publicFile;
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
                headers = modelResponse.Headers;
                if (modelResponse.StatusCode != -1)
                    statusCode = modelResponse.StatusCode;
            }
            else
            {
                // Иначе пытаемся найти сырой файл
                string path = @$"{StaticDirectory}{rawUrlWithoutArgs}";
                publicFile = true;

                if (File.Exists(path))
                {
                    FileStream file = new FileStream(path, FileMode.Open);
                    viewData = new byte[file.Length];
                    file.Read(viewData, 0, viewData.Length);
                    file.Close();
                }
                else
                {
                    // Выбрасываем страницу с ошибкой 404 и ставим соответствующий код статуса
                    var modelRequest = new ModelRequest(arguments, urlRoutes.ToArray(), method, context.Request.Cookies, context.Response.Cookies);
                    modelResponse = Error404.OnRequest(modelRequest);
                    viewData = modelResponse.ResponseData;
                    statusCode = 404;
                    headers = modelResponse.Headers;
                }
            }

            // Компонуем и возвращаем ответ
            return new RouterResponse()
            {
                UrlRoutes = urlRoutes.ToArray(),
                PageBuffer = viewData,
                Arguments = arguments,
                PublicFile = publicFile,
                StatusCode = statusCode,
                Headers = headers
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
        public WebHeaderCollection Headers { get; set; }
    }
}
