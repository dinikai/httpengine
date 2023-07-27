namespace HttpEngine.Core
{
    internal class LayoutModel : Model
    {
        public override ModelResponse OnRequest(ModelRequest request)
        {
            return new ModelResponse(File("_Layout.html", request));
        }
    }
}
