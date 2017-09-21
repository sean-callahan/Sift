using System.Data;
using System.Linq;

using Sift.Common.Network;
using Sift.Server.Db;

namespace Sift.Server
{
    internal class LoginManager
    {
        public static bool UserExists(string username)
        {
            using (var ctx = new UserContext())
            {
                return ctx.Users.Where(u => u.Username == username).Count() > 0;
            }
        }

        public static User Create(string username, string password, int? id = null)
        {
            User user = new User
            {
                Username = username,
            };
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            using (var ctx = new UserContext())
            {
                ctx.Users.Add(user);
                ctx.SaveChanges();
                return user;
            }
        }

        public static bool Login(NetworkUser netUser)
        {
            if (netUser == null || string.IsNullOrWhiteSpace(netUser.Username) || string.IsNullOrWhiteSpace(netUser.Password))
                return false;
            using (var ctx = new UserContext())
            {
                User user = ctx.Users.Where(u => u.Username == netUser.Username).FirstOrDefault();
                if (user == null)
                    return false;
                return BCrypt.Net.BCrypt.Verify(netUser.Password, user.PasswordHash);
            }
        }
    }
}
