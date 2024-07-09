namespace HttpEngine.Core
{
    public abstract class Middleware
    {
        public abstract ModelResult OnRequest(ModelRequest request, ModelResult previous);
    }
}
