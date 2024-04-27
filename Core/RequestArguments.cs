namespace HttpEngine.Core
{

    /// <summary>
    /// Represents the arguments of an HTTP request in the HTTP engine.
    /// </summary>
    public class RequestArguments
    {
        /// <summary>
        /// Gets or sets the dictionary of request arguments.
        /// </summary>
        public Dictionary<string, string> Arguments { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestArguments"/> class with the specified dictionary of arguments.
        /// </summary>
        /// <param name="arguments">The dictionary of request arguments.</param>
        public RequestArguments(Dictionary<string, string> arguments)
        {
            Arguments = arguments;
        }
    }
}
