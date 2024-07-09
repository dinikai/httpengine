using System.Text;

namespace HttpEngine.Stubs
{
    internal static class Stubs
    {
        public static byte[] Stub(StubType type, string message)
        {
            string color = "#f22323";
            string background = "#ffeded";

            switch (type)
            {
                case StubType.Warning:
                    color = "#ff7800";
                    background = "#ffe4c8";
                    break;
                case StubType.Information:
                    color = "#009fff";
                    background = "#d8f4fd";
                    break;
            }

            return Encoding.UTF8.GetBytes($@"<div style=""width: 100%; font-size: 12pt;
color: {color}; background: {background}; padding: 10px;"">HttpEngine:<br>{message}</div>");
        }

        public static byte[] ResourceNotFound(string resource)
        {
            return Stub(StubType.Error, $"Resource not found: {resource}");
        }
    }
}
