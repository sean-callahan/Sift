using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using Sift.Common;
using Sift.Server.Util;

namespace Sift.Server
{
    internal class LoginManager
    {
        public static bool HasUsername(DatabaseEngine engine, string username)
        {
            using (IDbConnection db = engine.NewConnection())
            {
                db.Open();
                IDbCommand cmd = DbUtil.CreateCommand(db,
                    "SELECT 1 FROM users WHERE username=@user",
                    new Dictionary<string, object> { { "@user", username } });
                using (IDataReader reader = cmd.ExecuteReader())
                {
                    return reader.Read();
                }
            }
        }

        public static User Create(DatabaseEngine engine, string username, string password, int? id = null)
        {
            User u = User.Create(username, password);
            u.Hash = BCrypt.Net.BCrypt.HashPassword(password);
            using (IDbConnection db = engine.NewConnection())
            {
                db.Open();
                string query = "INSERT INTO users (username, password) VALUES (@user, @pass)";
                var cmdParams = new Dictionary<string, object>
                {
                        { "@user", u.Username },
                        { "@pass", u.Hash },
                };
                if (id.HasValue)
                {
                    query = "INSERT INTO users (id, username, password) VALUES (@id, @user, @pass)";
                    cmdParams.Add("@id", id);
                }
                IDbCommand cmd = DbUtil.CreateCommand(db, query, cmdParams);
                int affected = cmd.ExecuteNonQuery();
                if (affected < 1)
                    return null;
                return u;
            }
        }

        public static bool Login(DatabaseEngine engine, User u)
        {
            if (u == null)
                return false;
            using (IDbConnection db = engine.NewConnection())
            {
                db.Open();
                IDbCommand cmd = DbUtil.CreateCommand(db,
                "SELECT password FROM users WHERE username=@user LIMIT 1;",
                new Dictionary<string, object> { { "@user", u.Username } });
                using (IDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string hash = (string)reader["password"];
                        return BCrypt.Net.BCrypt.Verify(u.Password, hash);
                    }
                    return false;
                }
            }
        }

        private static string Base64Decode(string data)
        {
            var b = Convert.FromBase64String(data);
            return Encoding.UTF8.GetString(b);
        }
    }
}
