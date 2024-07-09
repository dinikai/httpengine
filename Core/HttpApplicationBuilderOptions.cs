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
        /// Gets or sets the hosts that the application will listen to.
        /// </summary>
        public string[]? Hosts { get; set; }

        /// <summary>
        /// Gets or sets the default layout for the application.
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
        /// Gets or sets the cache control options for the application.
        /// </summary>
        public CacheControl? CacheControl { get; set; }

        /// <summary>
        /// Gets or sets the handler parameter name for the application.
        /// </summary>
        public string? Handler { get; set; }

        /// <summary>
        /// Gets or sets the encoding for the pages of the application.
        /// </summary>
        public Encoding? ContentEncoding { get; set; }

        /// <summary>
        /// Gets or sets caching of resource files.
        /// </summary>
        public bool ResourceCaching { get; set; }
    }
}
