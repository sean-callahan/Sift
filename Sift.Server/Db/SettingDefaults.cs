using System.Collections.Generic;

namespace Sift.Server.Db
{
    internal static class SettingDefaults
    {
        internal static IList<Setting> settings = new List<Setting>
        {
            new Setting("asterisk", "asterisk_screener_extension", 2000),
            new Setting("asterisk", "asterisk_hybrid_extensions", new int[] { 2001, 2002, 2003, 2004 }),
        };
    }
}
