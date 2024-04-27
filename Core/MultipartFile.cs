namespace HttpEngine.Core
{
    /// <summary>
    /// Represents a multipart file in the HTTP engine.
    /// </summary>
    public class MultipartFile
    {
        /// <summary>
        /// Gets or sets the name of the multipart file.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the content type of the multipart file.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the data of the multipart file.
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipartFile"/> class with the specified name, filename, content type, and data.
        /// </summary>
        /// <param name="name">The name of the multipart file.</param>
        /// <param name="filename">The name of the file.</param>
        /// <param name="contentType">The content type of the multipart file.</param>
        /// <param name="data">The data of the multipart file.</param>
        public MultipartFile(string name, string filename, string contentType, byte[] data)
        {
            Name = name;
            FileName = filename;
            ContentType = contentType;
            Data = data;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipartFile"/> class with the specified name, filename, content type, and stream.
        /// </summary>
        /// <param name="name">The name of the multipart file.</param>
        /// <param name="filename">The name of the file.</param>
        /// <param name="contentType">The content type of the multipart file.</param>
        /// <param name="stream">The stream containing the data of the multipart file.</param>
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
