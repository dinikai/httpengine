using System.Text;

namespace HttpEngine.Core
{
    public struct HttpApplicationBuilderOptions
    {
        /// <summary>
        /// Gets or sets the router associated with the HTTP application.
        /// </summary>
        public Router? Router { get; set; }

        /// <summary>
        /// Gets or sets the hosts that the HTTP application will listen to.
        /// </summary>
        public string[]? Hosts { get; set; }

        /// <summary>
        /// Gets or sets the layout for the HTTP application.
        /// </summary>
        public Layout? Layout { get; set; }

        /// <summary>
        /// Gets or sets the directory where resources are located.
        /// </summary>
        public string? ResourcesDirectory { get; set; }

        /// <summary>
        /// Gets or sets the directory where public files are located.
        /// </summary>
        public string? PublicDirectory { get; set; }

        /// <summary>
        /// Gets or sets the cache control options for the HTTP application.
        /// </summary>
        public CacheControl? CacheControl { get; set; }

        /// <summary>
        /// Gets or sets the handler for the HTTP application.
        /// </summary>
        public string? Handler { get; set; }

        /// <summary>
        /// Gets or sets the encoding for the content of the HTTP application.
        /// </summary>
        public Encoding? ContentEncoding { get; set; }
    }
}
