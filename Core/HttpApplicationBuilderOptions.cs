namespace HttpEngine.Core
{
    public class HttpApplicationBuilderOptions
    {
        public Router? Router { get; set; }
        public string? Host { get; set; }
        public string? Layout { get; set; }
    }
}
