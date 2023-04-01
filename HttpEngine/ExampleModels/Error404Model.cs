using HttpEngine.Core;
using System;

namespace HttpEngine.Models
{
    internal class Error404Model : Model
    {
        public override ModelResponse OnRequest(ModelRequest request) => new ModelResponse(File("Pages/Error404.html", false));
    }
}
