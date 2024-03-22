using System.Text;

namespace HttpEngine.Core
{
    public struct HttpApplicationBuilderOptions
    {
        public Router? Router { get; set; }
        public string[]? Hosts { get; set; }
        public Layout? Layout { get; set; }
        public string? PublicDirectory { get; set; }
        public string? StaticDirectory { get; set; }
        public CacheControl? CacheControl { get; set; }
        public string? Handler { get; set; }
        public Encoding? ContentEncoding { get; set; }
    }
}
