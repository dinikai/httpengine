namespace HttpEngine.Core
{
    public class HttpApplicationBuilderOptions
    {
        public Router? Router { get; set; }
        public string? Host { get; set; }
        public Layout? Layout { get; set; }
        public string? PublicDirectory { get; set; }
        public string? StaticDirectory { get; set; }
        public CacheControl? CacheControl { get; set; }
    }
}
