using System.Text;

namespace HttpEngine.Core
{
    /// <summary>
    /// Represents a view in the HTTP engine.
    /// </summary>
    public abstract class View
    {
        /// <summary>
        /// Gets or sets the layout associated with the view.
        /// </summary>
        public Layout Layout { get; set; }

        /// <summary>
        /// Gets or sets the directory where resources are located.
        /// </summary>
        public string ResourcesDirectory { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use the layout for the view.
        /// </summary>
        public bool UseLayout { get; set; } = true;

        /// <summary>
        /// Gets the view content based on the model request.
        /// </summary>
        /// <param name="request">The model request.</param>
        /// <returns>The model file representing the view content.</returns>
        public abstract ModelFile GetView(ModelRequest request);

        /// <summary>
        /// Retrieves a model file from the specified file name and request.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="request">The model request.</param>
        /// <returns>The model file.</returns>
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
