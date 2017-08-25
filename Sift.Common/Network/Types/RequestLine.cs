using Lidgren.Network;

namespace Sift.Common.Network
{
    public class RequestLine : IPacket
    {
        public int Index;

        public PacketType Type => PacketType.RequestLine;

        public RequestLine(int index)
        {
            Index = index;
        }

        public RequestLine(NetIncomingMessage msg) => Decode(msg);

        public void Decode(NetIncomingMessage msg)
        {
            Index = msg.ReadInt32();
        }

        public void Encode(NetOutgoingMessage msg)
        {
            msg.Write(Index);
        }
    }
}
