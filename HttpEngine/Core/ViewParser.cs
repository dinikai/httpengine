using System.Text;

namespace HttpEngine.Core
{
    public static class ViewParser
    {
        public static byte[] Parse(byte[] bytes, Dictionary<string, object> dictionary)
        {
            string data = Encoding.UTF8.GetString(bytes);
            foreach (string key in dictionary.Keys)
            {
                data = data.Replace("@" + key, dictionary[key].ToString());
            }

            return Encoding.UTF8.GetBytes(data);
        }
    }
}
