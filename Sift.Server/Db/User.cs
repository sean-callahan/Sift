using System;
using System.Linq;
using System.Security.Cryptography;
using JsonFlatFileDataStore;

namespace Sift.Server.Db
{
    public class User
    {
        public const string File = "users.json";

        public string Id { get; set; }
        public string Username { get; set; }
        public string Hash { get; set; }

        public static bool Exists(string username)
        {
            var store = new DataStore(File);
            var col = store.GetCollection<User>();
            return col.AsQueryable()
                .Where(u => u.Username.Equals(username, StringComparison.InvariantCultureIgnoreCase))
                .Count() > 0;
        }

        public static void Store(User u)
        {
            if (Exists(u.Username))
                throw new ArgumentException("Username already exists.");

            u.Id = Guid.NewGuid().ToString();

            var store = new DataStore(File);
            var col = store.GetCollection<User>();
            col.InsertOne(u);
        }

        public static string HashPassword(string password)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            return Convert.ToBase64String(hashBytes);
        }

        public static bool ComparePasswordAndHash(string password, string hash)
        {
            byte[] hashBytes = Convert.FromBase64String(hash);
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] compare = pbkdf2.GetBytes(20);
            for (int i = 0; i < 20; i++)
            {
                if (hashBytes[i + 16] != compare[i])
                    return false;
            }
            return true;
        }
    }
}
