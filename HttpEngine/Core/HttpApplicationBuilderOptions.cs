using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpEngine.Core
{
    internal class HttpApplicationBuilderOptions
    {
        public Router Router { get; set; }

        public HttpApplicationBuilderOptions(Router router)
        {
            Router = router;
        }
    }
}
