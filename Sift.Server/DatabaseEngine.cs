using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;

using IniParser.Model;
using MySql.Data.MySqlClient;

using Sift.Server.Db;

namespace Sift.Server
{
    internal struct DatabaseEngine
    {
        private enum Types
        {
            None,
            Sqlite,
            MySql,
        }

        private static Dictionary<string, Types> names = new Dictionary<string, Types>()
        {
            { "sqlite", Types.Sqlite },
            { "mysql", Types.MySql },
        };
        private static Dictionary<Types, Func<KeyDataCollection, string>> parsers =
            new Dictionary<Types, Func<KeyDataCollection, string>>()
            {
                { Types.Sqlite, parseSqlite },
                { Types.MySql, parseMySql },
            };

        public string ConnectionString { get; }

        private Types engine;

        public DatabaseEngine(KeyDataCollection data)
        {
            string type = data["engine"].Trim().ToLower();
            if (!names.TryGetValue(type, out engine))
                throw new Exception("unsupported database engine");
            ConnectionString = parsers[engine](data);
        }

        public DbConnection CreateConnection()
        {
            switch (engine)
            {
                case Types.MySql:
                    return new MySqlConnection(ConnectionString);
                case Types.Sqlite:
                    return new SQLiteConnection(ConnectionString);
                default:
                    return null;
            }
        }

        private static string parseSqlite(KeyDataCollection data)
        {
            KeyData file = data.GetKeyData("file");
            if (file == null)
                throw new Exception("missing file value");
            return $"Data Source={file.Value.Trim()};Version=3;";
        }

        internal void Initialize()
        {
            using (var ctx = new SettingContext())
            {
                foreach (var setting in SettingDefaults.settings)
                {
                    var result = ctx.Settings.Where(s => s.Category == setting.Category && s.Key == setting.Key).FirstOrDefault();
                    if (result == null)
                    {
                        ctx.Settings.Add(setting);
                    }
                }
                ctx.SaveChanges();
            }
        }

        private static string parseMySql(KeyDataCollection data)
        {
            MySqlConnectionStringBuilder b = new MySqlConnectionStringBuilder();
            b.Port = uint.Parse(data["port"]);
            b.UserID = data["user"];
            b.Server = data["host"];
            b.Database = data["database"];
            b.Password = data["password"];
            return b.ToString();
        }
    }
}
