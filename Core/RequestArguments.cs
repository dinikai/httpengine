namespace HttpEngine.Core
{

    /// <summary>
    /// Represents the arguments of an HTTP request in the HTTP engine.
    /// </summary>
    public class RequestArguments
    {
        /// <summary>
        /// Gets request argument by key if it exists, otherwise returns null.
        /// </summary>
        public string? this[string key]
        {
            get
            {
                if (Arguments.ContainsKey(key))
                    return Arguments[key];
                else
                    return null;
            }
        }

        internal Dictionary<string, string> Arguments { get; set; }

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
