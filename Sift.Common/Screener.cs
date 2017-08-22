namespace Sift.Common
{
    public class Screener
    {
        public string Id { get; set; }

        public string Extension { get; }

        public Screener(string exten)
        {
            Extension = exten;
            Id = string.Empty;
        }
    }
}
