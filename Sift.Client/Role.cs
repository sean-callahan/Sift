using System.Collections.Generic;

namespace Sift.Client
{
    public enum Role
    {
        None,
        Host,
        Screener,
    }

    internal static class RoleExtensions
    {
        private static readonly IReadOnlyDictionary<Role, string> names = new Dictionary<Role, string>()
        {
            { Role.None, string.Empty },
            { Role.Host, "Host" },
            { Role.Screener, "Screener" },
        };

        public static string ToString(this Role r)
        {
            return names[r];
        }
    }
}
