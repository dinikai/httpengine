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

        public static string Between(this string @this, string a, string b, bool remove = false)
        {
            int posA = @this.IndexOf(a);
            int posB = @this.IndexOf(b, posA + 1);
            if (posA == -1)
            {
                return "Between error: A = -1";
            }
            if (posB == -1)
            {
                return "Between error: B = -1";
            }
            int adjustedPosA = posA + a.Length;
            if (adjustedPosA >= posB)
            {
                return "Between error: PosA > PosB";
            }

            if (remove)
                return @this.Remove(posA, posB - posA + b.Length);
            else
                return @this.Substring(adjustedPosA, posB - adjustedPosA);
        }

        public static string ReplaceFirst(this string @this, string oldValue, string newValue)
        {
            int startindex = @this.IndexOf(oldValue);

            if (startindex == -1)
            {
                return @this;
            }

            return @this.Remove(startindex, oldValue.Length).Insert(startindex, newValue);
        }
    }
}
