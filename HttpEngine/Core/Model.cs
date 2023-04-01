using System.Text;

namespace HttpEngine.Core
{
    public class Model : IModel
    {
        public List<string> Routes { get; set; } = new();
        public string PublicDirectory { get; set; }
        public IModel Error404 { get; set; }
        public string Layout { get; set; }

        public virtual ModelResponse OnRequest(ModelRequest request)
        {
            throw new NotImplementedException();
        }

        public byte[] File(string path, bool useLayout = true)
        {
            FileStream file = new FileStream(Path.Combine(PublicDirectory, path), FileMode.Open);
            byte[] buffer = new byte[file.Length];
            file.Read(buffer);
            file.Close();

            FileStream layoutFile = new FileStream(Path.Combine(PublicDirectory, Layout), FileMode.Open);
            byte[] layoutBuffer = new byte[layoutFile.Length];
            layoutFile.Read(layoutBuffer);
            layoutFile.Close();

            if (useLayout)
            {
                byte[] layout = ViewParser.Parse(ref layoutBuffer, new()
                {
                    ["body"] = Encoding.UTF8.GetString(buffer),
                }, false);
                return layout;
            } else
            {
                return buffer;
            }
            
        }
    }
}
