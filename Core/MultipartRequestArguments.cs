namespace HttpEngine.Core
{
    public class MultipartRequestArguments : RequestArguments
    {
        public Dictionary<string, byte[]> Files { get; set; }

        public MultipartRequestArguments(Dictionary<string, string> arguments, Dictionary<string, byte[]> files) : base(arguments)
        {
            Files = files;
        }
    }
}
