using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpEngine.Core
{
    public class Map
    {
        public HttpMethod? Method { get; set; }
        public string Route { get; set; }
        public Func<ModelRequest, ModelResult> Func { get; set; }

        public Map(HttpMethod? method, string route, Func<ModelRequest, ModelResult> func)
        {
            Method = method;
            Route = route;
            Func = func;
        }
    }
}
