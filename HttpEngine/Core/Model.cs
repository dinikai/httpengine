using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpEngine.Core
{
    public class Model : IModel
    {
        public List<string> Routes { get; set; } = new();
        public string PublicDirectory { get; set; }
        public IModel Error404 { get; set; }

        public virtual ModelResponse OnRequest(ModelRequest request)
        {
            throw new NotImplementedException();
        }

        public byte[] File(string path)
        {
            FileStream file = new FileStream(Path.Combine(PublicDirectory, path), FileMode.Open);
            byte[] buffer = new byte[file.Length];
            file.Read(buffer, 0, buffer.Length);
            file.Close();

            return buffer;
        }
    }
}
