namespace Sift.Common.Network
{
    /*public class UpdateAppState : IPacket
    {
        public byte Lines;
        public VoipProviders ProviderType;

        public PacketType Type => PacketType.UpdateAppState;

        public UpdateAppState(byte lines, VoipProviders providerType)
        {
            Lines = lines;
            ProviderType = providerType;
        }

        public UpdateAppState(NetIncomingMessage msg) => Decode(msg);

        public void Decode(NetIncomingMessage msg)
        {
            Lines = msg.ReadByte();
            ProviderType = (VoipProviders)msg.ReadByte();
        }

        public void Encode(NetOutgoingMessage msg)
        {
            msg.Write(Lines);
            msg.Write((byte)ProviderType);
        }
    }*/
}
