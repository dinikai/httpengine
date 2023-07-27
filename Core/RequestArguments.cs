namespace HttpEngine.Core
{
    public class RequestArguments
    {
        public Dictionary<string, string> Arguments { get; set; }

        public RequestArguments(Dictionary<string, string> arguments)
        {
            Arguments = arguments;
        }
    }
}
