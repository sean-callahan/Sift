using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;

namespace Sift.Common.Network
{
    public class RequestSettings : IPacket
    {
        public string Category;
        public string Key;

        public RequestSettings()
        {
        }

        public RequestSettings(NetIncomingMessage msg) => Decode(msg);

        public PacketType Type => PacketType.RequestSettings;

        public void Decode(NetIncomingMessage msg)
        {
            Category = msg.ReadString();
            Key = msg.ReadString();
        }

        public void Encode(NetOutgoingMessage msg)
        {
            msg.Write(Category);
            msg.Write(Key);
        }
    }
}
