using System;
using Lidgren.Network;
using LiteDB;
using Sift.Common.Network;
using Sift.Common.Util;

namespace Sift.Common
{
    public class User : ICodec
    {
        public string Username { get; private set; }
        public string Hash { get; set; }

        private string base64Password;
        [BsonIgnore]
        public string Password
        {
            get { return Encoding.Base64Decode(base64Password); }
            set { base64Password = Encoding.Base64Encode(value); }
        }

        public static User Create(string username, string password)
        {
            return new User()
            {
                Username = username,
                Password = password,
            };
        }

        public void Decode(NetIncomingMessage msg)
        {
            Username = msg.ReadString();
            base64Password = msg.ReadString();
        }

        public void Encode(NetOutgoingMessage msg)
        {
            msg.Write(Username);
            msg.Write(base64Password);
        }
    }
}
