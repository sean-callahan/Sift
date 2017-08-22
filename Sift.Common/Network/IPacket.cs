using Lidgren.Network;

namespace Sift.Common.Network
{
    public interface IPacket
    {
        PacketType Type { get; } 
        void Decode(NetIncomingMessage msg);
        void Encode(NetOutgoingMessage msg);
    }
}
