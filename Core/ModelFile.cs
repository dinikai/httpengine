using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpEngine.Core
{
    public class ModelFile
    {
        public byte[] Data { get; set; }

        public ModelFile(byte[] data)
        {
            Data = data;
        }

        public void ParseView(Dictionary<string, object> dictionary, bool removeSections = true)
        {
            string @string = Encoding.UTF8.GetString(ParseRaw(Data, dictionary, "@"));
            int indexOfSection = @string.IndexOf("!==");
            int indexOfEnd = @string.LastIndexOf("==!");
            if (indexOfSection != -1 && indexOfEnd != -1 && removeSections)
                @string = @string.Remove(indexOfSection, indexOfEnd - indexOfSection + 3);

            Data = Encoding.UTF8.GetBytes(@string);
        }
        private static string ParseSection(string section, Dictionary<string, object> dictionary) =>
            Encoding.UTF8.GetString(ParseRaw(Encoding.UTF8.GetBytes(section), dictionary, "$"));

        private static byte[] ParseRaw(byte[] bytes, Dictionary<string, object> dictionary, string prefix)
        {
            string data = Encoding.UTF8.GetString(bytes);
            foreach (string key in dictionary.Keys)
            {
                data = data.Replace(prefix + key + ";", dictionary[key].ToString());
            }

            return Encoding.UTF8.GetBytes(data);
        }

        public string GetSection(string sectionName, Dictionary<string, object> dictionary)
        {
            return ParseSection(GetSection(sectionName), dictionary);
        }

        private string GetSection(string sectionName)
        {
            string data = Encoding.UTF8.GetString(Data);
            string section = data.Between($"!=={sectionName};", "==!");

            return section;
        }
    }
}
