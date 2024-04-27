namespace HttpEngine.Core
{
    /// <summary>
    /// Represents the arguments of a multipart/form-data HTTP request in the HTTP engine.
    /// </summary>
    public class MultipartRequestArguments : RequestArguments
    {
        /// <summary>
        /// Gets or sets the array of files that given with request.
        /// </summary>
        public MultipartFile[] Files { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipartRequestArguments"/> class with the specified dictionary of arguments and array of multipart files.
        /// </summary>
        /// <param name="arguments">The dictionary of request arguments.</param>
        /// <param name="files">The array of multipart files.</param>
        public MultipartRequestArguments(Dictionary<string, string> arguments, MultipartFile[] files) : base(arguments)
        {
            Files = files;
        }
    }
}
