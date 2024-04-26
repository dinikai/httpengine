using System.Collections.Specialized;
using System.Text;
using System.Web;
using static System.Collections.Specialized.BitVector32;

namespace HttpEngine.Core
{
    public static class Extensions
    {
        public static Dictionary<string, string> ToDictionary(this NameValueCollection nvc)
        {
            try
            {
                return nvc.AllKeys.ToDictionary(k => k, k => nvc[k])!;
            } catch
            {
                return new();
            }
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

        public static string Between(this string @this, string a, string b)
        {
            int posA = @this.IndexOf(a);
            int posB = @this.IndexOf(b, posA + 1);
            int adjustedPosA = posA + a.Length;

            return @this[adjustedPosA..posB];
        }

        public static string BetweenAndRemove(this string @this, string a, string b, ref string newString)
        {
            int posA = @this.IndexOf(a);
            int posB = @this.IndexOf(b, posA + 1);

            string between = @this.Between(a, b);
            newString = @this.Remove(posA, posB - posA + b.Length);
            return between;
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
