using Lidgren.Network;

namespace Sift.Common.Network
{
    public class RequestScreen : IPacket
    {
        public ushort Index;

        public PacketType Type => PacketType.RequestScreen;

        public RequestScreen(int index)
        {
            Index = (ushort)index;
        }

        public RequestScreen(NetIncomingMessage msg) => Decode(msg);

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
