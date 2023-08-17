namespace HttpEngine.Core
{
    public class Layout
    {
        public string PublicDirectory { get; set; } = "";

        public Layout(string publicDirectory)
        {
            PublicDirectory = publicDirectory;
        }

        public Layout()
        {
            
        }

        public virtual ModelFile OnRequest(ModelRequest request)
        {
            return File("_Layout.html");
        }

        protected ModelFile File(string path)
        {
            FileStream file = new FileStream(Path.Combine(PublicDirectory, path), FileMode.Open);
            byte[] buffer = new byte[file.Length];
            file.Read(buffer);
            file.Close();

            return new ModelFile(buffer);
        }
    }
}
