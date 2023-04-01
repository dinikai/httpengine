namespace Singlebase
{
    public class Singlebase
    {
        public string Source { get; set; }

        public Singlebase(string source)
        {
            Source = source;
        }

        /*public string[] Read(string table)
        {
            string data;

            using (var reader = new StreamReader(Source))
            {
                data = reader.ReadToEnd();
                for (int i = 0; i < data.Length; i++)
                {

                }
            }
        }*/
    }
}
