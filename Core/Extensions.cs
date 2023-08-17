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

        public static byte[] ParseView(this byte[] bytes, Dictionary<string, object> dictionary, bool removeSections = true)
        {
            string @string = Encoding.UTF8.GetString(ParseRaw(bytes, dictionary, "@"));
            int indexOfSection = @string.IndexOf("!==");
            int indexOfEnd = @string.LastIndexOf("==!");
            if (indexOfSection != -1 && indexOfEnd != -1 && removeSections)
                @string = @string.Remove(indexOfSection, indexOfEnd - indexOfSection + 3);

            return Encoding.UTF8.GetBytes(@string);
        }
        public static string ParseSection(this byte[] bytes, Dictionary<string, object> dictionary) => Encoding.UTF8.GetString(ParseRaw(bytes, dictionary, "$"));

        static byte[] ParseRaw(byte[] bytes, Dictionary<string, object> dictionary, string prefix)
        {
            string data = Encoding.UTF8.GetString(bytes);
            foreach (string key in dictionary.Keys)
            {
                data = data.Replace(prefix + key, dictionary[key].ToString());
            }

            return Encoding.UTF8.GetBytes(data);
        }

        public static string GetSection(this byte[] bytes, string sectionName, Dictionary<string, object> dictionary)
        {
            return ParseSection(Encoding.UTF8.GetBytes(bytes.GetSection(sectionName)), dictionary);
        }

        public static string GetSection(this byte[] bytes, string sectionName)
        {
            string data = Encoding.UTF8.GetString(bytes);
            string section = data.Between($"!=={sectionName}", "==!");

            return section;
        }
    }
}
