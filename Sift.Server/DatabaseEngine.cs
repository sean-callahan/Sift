using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using IniParser.Model;

using MySql.Data.MySqlClient;
using Sift.Server.Util;

namespace Sift.Server
{
    public struct DatabaseEngine
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

        public void Initialize()
        {
            using (IDbConnection db = NewConnection())
            {
                db.Open();

                string fileName;
                switch (engine)
                {
                    case Types.Sqlite:
                        fileName = "sqlite.txt";
                        break;
                    case Types.MySql:
                        fileName = "mysql.txt";
                        break;
                    default:
                        fileName = "";
                        break;
                }

                using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream("Sift.Server.Schema." + fileName))
                {
                    using (StreamReader r = new StreamReader(s))
                    {
                        while (r.Peek() >= 0)
                        {
                            string query = r.ReadLine();
                            DbUtil.CreateCommand(db, query).ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        public IDbConnection NewConnection()
        {
            switch (engine)
            {
                case Types.Sqlite:
                    return new SQLiteConnection(ConnectionString);
                case Types.MySql:
                    return new MySqlConnection(ConnectionString);
                default:
                    throw new Exception("Unsupported database engine");
            }
        }

        private static string parseSqlite(KeyDataCollection data)
        {
            KeyData file = data.GetKeyData("file");
            if (file == null)
                throw new Exception("missing file value");
            return $"Data Source={file.Value.Trim()};Version=3;";
        }

        private static string parseMySql(KeyDataCollection data)
        {
            return $"server={data["host"]};user={data["user"]};database={data["database"]};port={data["port"]};password={data["password"]}";
        }
    }
}
