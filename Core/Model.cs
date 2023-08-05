using System.Net;
using System.Text;

namespace HttpEngine.Core
{
    public class Model : IModel
    {
        public List<string> Routes { get; set; } = new();
        public string PublicDirectory { get; set; }
        public IModel Error404 { get; set; }
        public Layout Layout { get; set; }
        public bool UseLayout { get; set; } = true;
        public HttpApplication Application { get; set; }

        public virtual ModelResponse OnRequest(ModelRequest request)
        {
            //throw new NotImplementedException();
            return new ModelResponse();
        }

        public virtual void OnUse()
        {

        }

        protected byte[] File(string path, ModelRequest request)
        {
            FileStream file = new FileStream(Path.Combine(PublicDirectory, path), FileMode.Open);
            byte[] buffer = new byte[file.Length];
            file.Read(buffer);
            file.Close();

            if (UseLayout)
            {
                byte[] layoutBuffer = Layout.OnRequest(request);
                byte[] layout = layoutBuffer.ParseView(new()
                {
                    ["body"] = Encoding.UTF8.GetString(buffer),
                }, false);
                return layout;
            } else
            {
                return buffer;
            }
        }

        public ModelResponse? CallModel<T>(ModelRequest request) where T : IModel
        {
            IModel? model = Application.Router.Models.FirstOrDefault(x => x is T);
            if (model == null)
                return null;

            return model.OnRequest(request);
        }

        public static ModelResponse Redirect(string url)
        {
            WebHeaderCollection headers = new()
            {
                { "Location", url }
            };
            return new ModelResponse()
            {
                Headers = headers,
                StatusCode = 302
            };
        }

        protected void RemoveModel()
        {
            Application.RemoveModel(this);
        }
    }
}
