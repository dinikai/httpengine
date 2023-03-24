using System.Collections.Specialized;
using System.Web;

namespace HttpEngine.Core
{
    public static class Extensions
    {
        public static Dictionary<string, string> ToDictionary(this NameValueCollection nvc)
        {
            return nvc.AllKeys.ToDictionary(k => k, k => nvc[k]);
        }

        public static Dictionary<string, string> GetPostParams(string rawData)
        {
            Dictionary<string, string> postParams = new Dictionary<string, string>();
            string[] rawParams = rawData.Split('&');
            foreach (string param in rawParams)
            {
                string[] kvPair = param.Split('=');
                string key = kvPair[0];
                string value = HttpUtility.UrlDecode(kvPair[1]);
                postParams.Add(key, value);
            }

            return postParams;
        }
    }
}
