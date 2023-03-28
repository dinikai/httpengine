namespace HttpEngine.Core
{
    internal class HttpApplicationBuilderOptions
    {
        public Router Router { get; set; }

        public HttpApplicationBuilderOptions(Router router)
        {
            Router = router;
        }
    }
}
