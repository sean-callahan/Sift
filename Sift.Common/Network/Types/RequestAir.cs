using Lidgren.Network;

namespace Sift.Common.Network
{
    public class RequestAir : IPacket
    {
        public ushort Index;

        public PacketType Type => PacketType.RequestAir;

        public RequestAir(int index)
        {
            Index = (ushort)index;
        }

        public RequestAir(NetIncomingMessage msg) => Decode(msg);

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
