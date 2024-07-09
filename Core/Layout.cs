namespace HttpEngine.Core
{
    /// <summary>
    /// Represents the layout of a web application.
    /// </summary>
    public class Layout
    {
        /// <summary>
        /// Gets or sets the HTTP application associated with the layout.
        /// </summary>
        public HttpApplication Application { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Layout"/> class with the specified application.
        /// </summary>
        /// <param name="application">The HTTP application associated with the layout.</param>
        public Layout(HttpApplication application)
        {
            Application = application;
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
        /// Retrieves a model file from the specified file.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="request">The model request.</param>
        /// <returns>The model file.</returns>
        protected ModelFile File(string fileName)
        {
            byte[] data = Application.ReadResource(fileName)!;

            return new ModelFile(data);
        }
    }
}
