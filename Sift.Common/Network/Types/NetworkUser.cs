
using Sift.Common.Util;

namespace Sift.Common.Network
{
    /*public class NetworkUser : ICodec
    {
        public int Id { get; set; }
        public string Username { get; set; }

        private string encoded;
        public string Password
        {
            get => Encoding.Base64Decode(encoded);
            set => encoded = Encoding.Base64Encode(value);
        }

        public void Decode(NetIncomingMessage msg)
        {
            Id = msg.ReadVariableInt32();
            Username = msg.ReadString();
            encoded = msg.ReadString();
        }

        public void Encode(NetOutgoingMessage msg)
        {
            msg.WriteVariableInt32(Id);
            msg.Write(Username);
            msg.Write(encoded);
        }
    }*/
}
