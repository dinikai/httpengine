using HttpEngine.Core;
using System;

namespace HttpEngine.Models
{
    internal class IndexModel : IModel
    {
        public ModelResponse OnRequest(ModelRequest request)
        {
            var response = new ModelResponse();
            string argsString = "";
            foreach (var argument in request.Arguments)
            {
                argsString += $"<li>{argument.Key} = \"{argument.Value}\"</li>";
            }

            Random random = new Random();
            response.ViewData = new()
            {
                ["title"] = request.Method,
                ["random"] = random.Next(50000),
                ["args"] = argsString
            };

            response.ResponseFile = "Pages/Index.html";
            return response;
        }
    }
}
