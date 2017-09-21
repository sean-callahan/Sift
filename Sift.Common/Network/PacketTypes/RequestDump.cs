using Lidgren.Network;

namespace Sift.Common.Network
{
    public class RequestDump : IPacket
    {
        public ushort Index;

        public PacketType Type => PacketType.RequestDump;

        public RequestDump(int index)
        {
            Index = (ushort)index;
        }

        public RequestDump(NetIncomingMessage msg) => Decode(msg);

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
