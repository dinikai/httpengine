namespace HttpEngine.Core
{
    /// <summary>
    /// Represents a mapping between an HTTP method, a route, and a function in the HTTP engine.
    /// </summary>
    public class Map
    {
        /// <summary>
        /// Gets or sets the HTTP method associated with the map.
        /// </summary>
        public HttpMethod? Method { get; set; }

        /// <summary>
        /// Gets or sets the route associated with the map.
        /// </summary>
        public string Route { get; set; }

        /// <summary>
        /// Gets or sets the function associated with the map.
        /// </summary>
        public Func<ModelRequest, ModelResult> Func { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Map"/> class with the specified HTTP method, route, and function.
        /// </summary>
        /// <param name="method">The HTTP method associated with the map.</param>
        /// <param name="route">The route associated with the map.</param>
        /// <param name="func">The function associated with the map.</param>
        public Map(HttpMethod? method, string route, Func<ModelRequest, ModelResult> func)
        {
            Method = method;
            Route = route;
            Func = func;
        }
    }
}
