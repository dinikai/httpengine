using System.Net;

namespace HttpEngine.Core
{
    /// <summary>
    /// Represents a model in the HTTP engine.
    /// </summary>
    public class Model : IModel
    {
        /// <summary>
        /// Gets or sets the routes associated with the model.
        /// </summary>
        public List<string> Routes { get; set; } = new();

        /// <summary>
        /// Gets or sets the error 404 model.
        /// </summary>
        public IModel Error404 { get; set; }

        /// <summary>
        /// Gets or sets the HTTP application associated with the model.
        /// </summary>
        public HttpApplication Application { get; set; }

        /// <summary>
        /// Handles incoming model requests.
        /// </summary>
        /// <param name="request">The model request.</param>
        /// <returns>The model result.</returns>
        public virtual ModelResult OnRequest(ModelRequest request)
        {
            return new ModelResult();
        }

        /// <summary>
        /// Called when the model is put into use.
        /// </summary>
        public virtual void OnUse()
        {

        }

        /// <summary>
        /// Retrieves a view of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the view to retrieve.</typeparam>
        /// <returns>The view of the specified type.</returns>
        protected T View<T>() where T : View
        {
            return (T)Application.GetView<T>()!;
        }

        /// <summary>
        /// Retrieves a raw file from the specified file name.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <returns>The model file.</returns>
        public static ModelFile RawFile(string fileName)
        {
            FileStream file = new FileStream(fileName, FileMode.Open);
            byte[] buffer = new byte[file.Length];
            file.Read(buffer);
            file.Close();

            return new ModelFile(buffer);
        }

        /// <summary>
        /// Calls the specified model with the given request.
        /// </summary>
        /// <typeparam name="T">The type of the model to call.</typeparam>
        /// <param name="request">The model request.</param>
        /// <returns>The model result.</returns>
        public ModelResult? CallModel<T>(ModelRequest request) where T : IModel
        {
            IModel? model = Application.Router.Models.FirstOrDefault(x => x is T);
            if (model == null)
                return null;

            return model.OnRequest(request);
        }

        /// <summary>
        /// Skips this model from processing.
        /// </summary>
        /// <returns>The skip result.</returns>
        public static SkipResult Skip()
        {
            return new SkipResult();
        }

        /// <summary>
        /// Redirects the request to the specified URL.
        /// </summary>
        /// <param name="url">The URL to redirect to.</param>
        /// <returns>The model result for redirection.</returns>
        public static ModelResult Redirect(string url)
        {
            WebHeaderCollection headers = new()
            {
                { "Location", url }
            };
            return new ModelResult()
            {
                Headers = headers,
                StatusCode = 302
            };
        }

        /// <summary>
        /// Redirects the request back to URL of Referer header.
        /// </summary>
        /// <param name="request">The model request.</param>
        /// <returns>The model result for redirection back.</returns>
        public static ModelResult RedirectBack(ModelRequest request)
        {
            return Redirect(request.Headers["Referer"]!);
        }

        /// <summary>
        /// Initiates a file download with the specified data and file name.
        /// </summary>
        /// <param name="data">The data to be downloaded.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <returns>The model result for file download.</returns>
        public static ModelResult Download(byte[] data, string fileName)
        {
            WebHeaderCollection headers = new()
            {
                { "Content-Disposition", $"attachment; filename={fileName}" }
            };
            return new ModelResult(data)
            {
                Headers = headers,
            };
        }

        /// <summary>
        /// Removes the current model from the application.
        /// </summary>
        protected void RemoveModel()
        {
            Application.RemoveModel(this);
        }
    }
}
