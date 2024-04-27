namespace HttpEngine.Core
{
    /// <summary>
    /// Represents the layout of a web application.
    /// </summary>
    public class Layout
    {
        /// <summary>
        /// Gets or sets the directory where resources are located.
        /// </summary>
        public string ResourcesDirectory { get; set; } = "";

        /// <summary>
        /// Gets or sets the HTTP application associated with the layout.
        /// </summary>
        public HttpApplication Application { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Layout"/> class with the specified resources directory and HTTP application.
        /// </summary>
        /// <param name="resourcesDirectory">The directory where resources are located.</param>
        /// <param name="application">The HTTP application associated with the layout.</param>
        public Layout(string resourcesDirectory, HttpApplication application)
        {
            ResourcesDirectory = resourcesDirectory;
            Application = application;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Layout"/> class.
        /// </summary>
        public Layout()
        {

        }

        /// <summary>
        /// Retrieves a view of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the view to retrieve.</typeparam>
        /// <returns>The view of the specified type.</returns>
        protected T? View<T>() where T : View
        {
            return (T)Application.GetView<T>()!;
        }

        /// <summary>
        /// Handles incoming model requests.
        /// </summary>
        /// <param name="request">The model request.</param>
        /// <returns>The model file associated with the request.</returns>
        public virtual ModelFile OnRequest(ModelRequest request)
        {
            return File("_layout.html");
        }

        /// <summary>
        /// Retrieves a file from the specified path.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <returns>The model file.</returns>
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
