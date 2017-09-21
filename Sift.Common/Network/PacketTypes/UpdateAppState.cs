using Lidgren.Network;

namespace Sift.Common.Network
{
    public class UpdateAppState : IPacket
    {
        public ushort LineCount;
        public VoipProviders ProviderType;

        public PacketType Type => PacketType.UpdateAppState;

        public UpdateAppState(int lineCount, VoipProviders providerType)
        {
            LineCount = (ushort)lineCount;
            ProviderType = providerType;
        }

        public UpdateAppState(NetIncomingMessage msg) => Decode(msg);

        public void Decode(NetIncomingMessage msg)
        {
            LineCount = msg.ReadUInt16();
            ProviderType = (VoipProviders)msg.ReadByte();
        }

        public void Encode(NetOutgoingMessage msg)
        {
            msg.Write(LineCount);
            msg.Write((byte)ProviderType);
        }
    }
}
