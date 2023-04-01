using HttpEngine.Core;
using System;
using System.Text;

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
            byte[] responseFile = File("Pages/Index.html");
            var response = new ModelResponse();

            var argsSb = new StringBuilder();
            foreach (var argument in request.Arguments)
            {
                string section = ViewParser.GetSection(ref responseFile, "argument", new()
                {
                    ["argName"] = argument.Key,
                    ["argValue"] = argument.Value,
                });
                argsSb.Append(section);
            }

            Random random = new Random();
            var viewReplace = new Dictionary<string, object>()
            {
                ["title"] = "HttpEngine",
                ["random"] = random.Next(1000),
                ["args"] = argsSb.ToString(),
            };

            response.ResponseData = ViewParser.Parse(responseFile, viewReplace);
            return response;
        }
    }
}
