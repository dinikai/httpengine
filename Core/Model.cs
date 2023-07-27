using System.Text;

namespace HttpEngine.Core
{
    public class Model : IModel
    {
        public List<string> Routes { get; set; } = new();
        public string PublicDirectory { get; set; }
        public IModel Error404 { get; set; }
        public IModel Layout { get; set; }
        public bool UseLayout { get; set; } = true;

        public virtual ModelResponse OnRequest(ModelRequest request)
        {
            //throw new NotImplementedException();
            return new ModelResponse();
        }

        protected byte[] File(string path, ModelRequest request)
        {
            FileStream file = new FileStream(Path.Combine(PublicDirectory, path), FileMode.Open);
            byte[] buffer = new byte[file.Length];
            file.Read(buffer);
            file.Close();

            if (UseLayout)
            {
                ModelResponse layoutResponse = Layout.OnRequest(request);
                byte[] layoutBuffer = layoutResponse.ResponseData;
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
