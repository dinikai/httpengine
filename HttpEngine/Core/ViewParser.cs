using System.Text;

namespace HttpEngine.Core
{
    public static class ViewParser
    {
        public static byte[] Parse(byte[] bytes, Dictionary<string, object> dictionary) => ParseRaw(bytes, dictionary, "@");
        public static string ParseSection(byte[] bytes, Dictionary<string, object> dictionary)
            => Encoding.UTF8.GetString(ParseRaw(bytes, dictionary, "$"));

        static byte[] ParseRaw(byte[] bytes, Dictionary<string, object> dictionary, string prefix)
        {
            string data = Encoding.UTF8.GetString(bytes);
            foreach (string key in dictionary.Keys)
            {
                data = data.Replace(prefix + key, dictionary[key].ToString());
            }

            return Encoding.UTF8.GetBytes(data);
        }

        public static string GetSection(ref byte[] bytes, string sectionName, Dictionary<string, object> dictionary)
        {
            string data = Encoding.UTF8.GetString(bytes);
            string section = data.Between($"!=={sectionName}", "==!");
            section = ParseSection(Encoding.UTF8.GetBytes(section), dictionary);

            string withoutSection = data.Between($"!=={sectionName}", "==!", true);
            bytes = Encoding.UTF8.GetBytes(withoutSection);
            return section;
        }
    }
}
