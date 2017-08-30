using System;
using System.Linq;
using System.Text;
using LiteDB;
using Sift.Common;

namespace Sift.Server
{
    public class LoginManager
    {
        public static User Create(string username, string password)
        {
            User u = User.Create(username, password);
            u.Hash = BCrypt.Net.BCrypt.HashPassword(password);
            using (LiteDatabase db = new LiteDatabase(@".\Data.db"))
            {
                var col = db.GetCollection<User>("users");
                var result = col.Find(x => x.Username.Equals(username, StringComparison.InvariantCultureIgnoreCase));
                if (result.Count() > 0)
                    return null;
                col.Insert(u);
            }
            return u;
        }

        public static bool Login(User u)
        {
            if (u == null)
                return false;
            using (LiteDatabase db = new LiteDatabase(@".\Data.db"))
            {
                var col = db.GetCollection<User>("users");

                var result = col.Find(x => x.Username.Equals(u.Username)).FirstOrDefault();
                if (result == null)
                    return false;

                return BCrypt.Net.BCrypt.Verify(u.Password, result.Hash);
            }
        }

        private static string Base64Decode(string data)
        {
            var b = Convert.FromBase64String(data);
            return Encoding.UTF8.GetString(b);
        }
    }
}
