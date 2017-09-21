using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sift.Common.Network;
using Sift.Server.Util;

namespace Sift.Server.Db
{
    internal class Setting
    {
        [Key]
        [MaxLength(32)]
        public string Key { get; set; }

        [MaxLength(32)]
        public string Category { get; set; }
        
        public byte[] Value { get; set; }

        internal Setting()
        { }

        internal Setting(string cat, string key, object value)
        {
            Category = cat;
            Key = key;
            Value = DbUtil.Serialize(value);
        }

        internal NetworkSetting ToNetworkSetting() => new NetworkSetting(Key, Category, Value);
    }
}
