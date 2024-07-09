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
        public Layout? Layout { get; set; }

        /// <summary>
        /// Gets or sets the HTTP application associated with the model.
        /// </summary>
        public HttpApplication Application { get; set; }

        /// <summary>
        /// Gets the view content based on the model request.
        /// </summary>
        /// <param name="request">The model request.</param>
        /// <returns>The model file representing the view content.</returns>
        public abstract ModelFile Render(ModelRequest request);

        /// <summary>
        /// Called when the view is put into use.
        /// </summary>
        public virtual void OnUse()
        {

        }

        /// <summary>
        /// Retrieves a model file from the specified file.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="request">The model request.</param>
        /// <returns>The model file.</returns>
        protected ModelFile File(string fileName, ModelRequest request)
        {
            byte[] data = Application.ReadResource(fileName)!;

            string @string = Encoding.UTF8.GetString(data);
            data = Encoding.UTF8.GetBytes(@string.Replace("\r", ""));

            if (Layout == null)
            {
                return new ModelFile(data);
            }
            else
            {
                ModelFile layout = Layout.OnRequest(request);
                layout.ParseView(new()
                {
                    ["body"] = Encoding.UTF8.GetString(data),
                }, false);
                return layout;
            }
        }
    }
}
