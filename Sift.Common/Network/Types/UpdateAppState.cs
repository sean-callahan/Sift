using Lidgren.Network;

namespace Sift.Common.Network
{
    public class UpdateAppState : IPacket
    {
        public ushort LineCount;

        public PacketType Type => PacketType.UpdateAppState;

        public UpdateAppState(int lineCount)
        {
            LineCount = (ushort)lineCount;
        }

        public UpdateAppState(NetIncomingMessage msg) => Decode(msg);

        public void Decode(NetIncomingMessage msg)
        {
            LineCount = msg.ReadUInt16();
        }

        public void Encode(NetOutgoingMessage msg)
        {
            msg.Write(LineCount);
        }
    }
}
