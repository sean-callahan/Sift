using System.IO;

namespace Sift.Common.Net
{
    public interface ISdpPacket
    {
        PacketType Type { get; }
        void ReadFrom(BinaryReader reader);
        void WriteTo(BinaryWriter writer);
    }
}
