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
        public static User Create(Program program, string username, string password)
        {
            User u = User.Create(username, password);
            u.Hash = BCrypt.Net.BCrypt.HashPassword(password);
            using (IDbConnection db = program.Database)
            {
                db.Open();
                IDbCommand cmd = DbUtil.CreateCommand(db,
                    "INSERT INTO users (name, passwd) VALUES (@user, @pass)",
                    new Dictionary<string, object>
                    {
                        { "@user", u.Username },
                        { "@pass", u.Hash },
                    }
                );
                int affected = cmd.ExecuteNonQuery();
                if (affected < 1)
                    return null;
                return u;
            }
        }

        public static bool Login(Program program, User u)
        {
            if (u == null)
                return false;
            using (IDbConnection db = program.Database)
            {
                db.Open();
                IDbCommand cmd = DbUtil.CreateCommand(db,
                "SELECT passwd FROM users WHERE name=@user LIMIT 1;",
                new Dictionary<string, object> { { "@user", u.Username } });
                using (IDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string hash = (string)reader["passwd"];
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
