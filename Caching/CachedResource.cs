namespace HttpEngine.Caching
{
    internal class CachedResource
    {
        public string FileName { get; set; }
        public byte[] Data { get; set; }

        public CachedResource(string fileName, byte[] data)
        {
            FileName = fileName;
            Data = data;
        }
    }
}
