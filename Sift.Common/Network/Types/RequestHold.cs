using Lidgren.Network;

namespace Sift.Common.Network
{
    public class RequestHold : IPacket
    {
        public ushort Index;

        public PacketType Type => PacketType.RequestHold;

        public RequestHold(int index)
        {
            Index = (ushort)index;
        }

        public RequestHold(NetIncomingMessage msg) => Decode(msg);

        public void Decode(NetIncomingMessage msg)
        {
            Index = msg.ReadUInt16();
        }

        public void Encode(NetOutgoingMessage msg)
        {
            msg.Write(Index);
        }
    }
}
