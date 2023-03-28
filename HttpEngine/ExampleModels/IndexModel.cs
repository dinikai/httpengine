using HttpEngine.Core;
using System;

namespace HttpEngine.Models
{
    internal class IndexModel : Model
    {
        public IndexModel()
        {
            Routes = new() { "/", "/Index" };
        }

        public override ModelResponse OnRequest(ModelRequest request)
        {
            var response = new ModelResponse();
            string argsString = "";
            foreach (var argument in request.Arguments)
            {
                argsString += $"<li>{argument.Key} = \"{argument.Value}\"</li>";
            }

            Random random = new Random();

            var viewReplace = new Dictionary<string, object>()
            {
                ["title"] = "HttpEngine",
                ["random"] = random.Next(1000),
                ["args"] = argsString,
            };

            response.ResponseData = ViewParser.Parse(File("Pages/Index.html"), viewReplace);
            return response;
        }
    }
}
