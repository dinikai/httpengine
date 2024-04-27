using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpEngine.Core
{
    /// <summary>
    /// Represents a model file in the HTTP engine.
    /// </summary>
    public class ModelFile
    {
        /// <summary>
        /// Gets or sets the data of the model file.
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelFile"/> class with the specified data.
        /// </summary>
        /// <param name="data">The data of the model file.</param>
        public ModelFile(byte[] data)
        {
            Data = data;
        }

        /// <summary>
        /// Parses the view using the provided dictionary, optionally removing sections.
        /// </summary>
        /// <param name="dictionary">The dictionary containing key-value pairs for replacement.</param>
        /// <param name="removeSections">Specifies whether to remove sections from the view.</param>
        public void ParseView(Dictionary<string, object> dictionary, bool removeSections = true)
        {
            string @string = Encoding.UTF8.GetString(ParseRaw(Data, dictionary, "@"));
            int indexOfSection = @string.IndexOf("!==");
            int indexOfEnd = @string.LastIndexOf("==!");
            if (indexOfSection != -1 && indexOfEnd != -1 && removeSections)
                @string = @string.Remove(indexOfSection, indexOfEnd - indexOfSection + 3);

            Data = Encoding.UTF8.GetBytes(@string);
        }

        /// <summary>
        /// Gets the content of a section from the model file using the provided dictionary.
        /// </summary>
        /// <param name="sectionName">The name of the section.</param>
        /// <param name="dictionary">The dictionary containing key-value pairs for replacement.</param>
        /// <returns>The content of the section.</returns>
        public string GetSection(string sectionName, Dictionary<string, object> dictionary)
        {
            return ParseSection(GetSection(sectionName), dictionary);
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

        private string GetSection(string sectionName)
        {
            string data = Encoding.UTF8.GetString(Data);
            string section = data.Between($"!=={sectionName};", "==!");

            return section;
        }
    }
}
