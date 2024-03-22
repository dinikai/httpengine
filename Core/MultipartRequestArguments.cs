namespace HttpEngine.Core
{
    public class MultipartRequestArguments : RequestArguments
    {
        public MultipartFile[] Files { get; set; }

        public MultipartRequestArguments(Dictionary<string, string> arguments, MultipartFile[] files) : base(arguments)
        {
            Files = files;
        }
    }
}
