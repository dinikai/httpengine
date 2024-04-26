namespace HttpEngine.Core
{
    public class Layout
    {
        public string ResourcesDirectory { get; set; } = "";
        public HttpApplication Application { get; set; }

        public Layout(string resourcesDirectory, HttpApplication application)
        {
            ResourcesDirectory = resourcesDirectory;
            Application = application;
        }

        public Layout()
        {

        }

        protected T? View<T>() where T : View
        {
            return (T)Application.GetView<T>()!;
        }

        public virtual ModelFile OnRequest(ModelRequest request)
        {
            return File("_layout.html");
        }

        protected ModelFile File(string path)
        {
            FileStream file = new FileStream(Path.Combine(ResourcesDirectory, path), FileMode.Open);
            byte[] buffer = new byte[file.Length];
            file.Read(buffer);
            file.Close();

            return new ModelFile(buffer);
        }
    }
}
