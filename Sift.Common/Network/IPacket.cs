using Lidgren.Network;

namespace Sift.Common.Network
{
    public interface IPacket : ICodec
    {
        PacketType Type { get; } 
    }
}
