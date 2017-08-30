using System;

namespace Sift.Common.Util
{
    public static class Encoding
    {
        public static string Base64Encode(string input)
        {
            var b = System.Text.Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(b);
        }

        public static string Base64Decode(string input)
        {
            var b = Convert.FromBase64String(input);
            return System.Text.Encoding.UTF8.GetString(b);
        }
    }
}
