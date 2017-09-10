using Lidgren.Network;

namespace Sift.Common.Network
{
    public class SettingsChanged : IPacket
    {
        public PacketType Type => PacketType.SettingsChanged;

        public string Category { get; private set; }
        public SettingsCollection Settings { get; private set; }

        public SettingsChanged(string category)
        {
            Category = category;
            Settings = new SettingsCollection();
        }

        public SettingsChanged(string category, SettingsCollection settings)
        {
            Category = category;
            Settings = new SettingsCollection();
        }

        public SettingsChanged(NetIncomingMessage msg) => Decode(msg);

        public void Decode(NetIncomingMessage msg)
        {
            Category = msg.ReadString();
            Settings = new SettingsCollection();
            Settings.Decode(msg);
        }

        public void Encode(NetOutgoingMessage msg)
        {
            msg.Write(Category);
            Settings?.Encode(msg);
        }
    }
}
