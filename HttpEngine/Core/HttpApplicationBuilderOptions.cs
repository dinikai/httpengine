namespace HttpEngine.Core
{
    internal class HttpApplicationBuilderOptions
    {
        public Router? Router { get; set; }
        public string? Ip { get; set; }
        public string? Layout { get; set; }
    }
}
