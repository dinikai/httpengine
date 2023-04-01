namespace HttpEngine.Core
{
    internal class HttpApplicationBuilderOptions
    {
        public Router? Router { get; set; }
        public string? Ip { get; set; }

        public HttpApplicationBuilderOptions(Router? router) => Router = router;
        public HttpApplicationBuilderOptions(string? ip) => Ip = ip;

        public HttpApplicationBuilderOptions(Router? router, string? ip)
        {
            Router = router;
            Ip = ip;
        }
    }
}
