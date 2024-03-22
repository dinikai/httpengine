namespace HttpEngine.Core
{
    public class MultipartFile
    {
        public string Name { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] Data { get; set; }

        public MultipartFile(string name, string filename, string contentType, byte[] data)
        {
            Name = name;
            FileName = filename;
            ContentType = contentType;
            Data = data;
        }

        public MultipartFile(string name, string filename, string contentType, Stream stream)
            : this(name, filename, contentType, Array.Empty<byte>())
        {
            var data = new List<byte>();
            while (stream.Position < stream.Length)
            {
                data.Add((byte)stream.ReadByte());
            }
            Data = data.ToArray();
        }
    }
}
