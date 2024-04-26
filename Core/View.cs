using System.Text;

namespace HttpEngine.Core
{
    public abstract class View
    {
        public Layout Layout { get; set; }
        public string ResourcesDirectory { get; set; }
        public bool UseLayout { get; set; } = true;

        public abstract ModelFile GetView(ModelRequest request);

        protected ModelFile File(string fileName, ModelRequest request)
        {
            FileStream file = new FileStream(Path.Combine(ResourcesDirectory, fileName), FileMode.Open);
            byte[] buffer = new byte[file.Length];
            file.Read(buffer);
            file.Close();

            string @string = Encoding.UTF8.GetString(buffer);
            buffer = Encoding.UTF8.GetBytes(@string.Replace("\r", ""));

            if (UseLayout)
            {
                ModelFile layout = Layout.OnRequest(request);
                layout.ParseView(new()
                {
                    ["body"] = Encoding.UTF8.GetString(buffer),
                }, false);
                return layout;
            }
            else
            {
                return new ModelFile(buffer);
            }
        }
    }
}
