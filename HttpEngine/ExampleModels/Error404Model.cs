using HttpEngine.Core;
using System;

namespace HttpEngine.Models
{
    internal class Error404Model : IModel
    {
        public ModelResponse OnRequest(ModelRequest request) => new ModelResponse("Pages/Error404.html");
    }
}
