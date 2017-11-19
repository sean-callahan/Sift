using System.Linq;
using JsonFlatFileDataStore;
using Sift.Common.Net;
using Sift.Server.Util;

namespace Sift.Server.Db
{
    public class Setting
    {
        public const string File = "settings.json";

        public string Key { get; set; }
        public string Category { get; set; }
        public byte[] Value { get; set; }

        public Setting()
        { }

        public Setting(string cat, string key, object value)
        {
            Category = cat;
            Key = key;
            Value = DbUtil.Serialize(value);
        }

        private static IDocumentCollection<Setting> Open()
        {
            var store = new DataStore(File);
            return store.GetCollection<Setting>();
        }

        public static void Save(Setting s)
        {
            var db = Open();
            db.ReplaceOne(e => e.Category == s.Category && e.Key == s.Key, s, true);
        }

        public static void Save(Setting[] settings)
        {
            var db = Open();
            db.InsertMany(settings);
        }

        public static Setting Get(string category, string key)
        {
            var db = Open();
            return db.AsQueryable().Where(s => s.Category == category && s.Key == key).FirstOrDefault();
        }

        public static Setting[] Get(string category)
        {
            var db = Open();
            return db.AsQueryable().Where(s => s.Category == category).ToArray();
        }

        public static Setting[] Get()
        {
            var db = Open();
            return db.AsQueryable().ToArray();
        }

        internal NetworkSetting ToNetworkSetting() => new NetworkSetting(Key, Category, Value);

        public static readonly Setting[] Defaults = new Setting[]
        {
            new Setting { Category = "asterisk", Key = "screener_extension", Value = System.Text.Encoding.UTF8.GetBytes("2002") }
        };
    }
}
