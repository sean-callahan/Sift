using Lidgren.Network;

namespace Sift.Common.Network
{
    public class SdpServer : PacketConsumer
    {
        public const string App = "sift";

        private NetServer server;

        protected override NetPeer Peer => server;

        public SdpServer(int port)
        {
            NetPeerConfiguration config = new NetPeerConfiguration(App);
            config.Port = port;
            server = new NetServer(config);
        }
        
        public void Start() => server?.Start();

        public void Shutdown() => server?.Shutdown("Remote endpoint closed");

        public void Broadcast(IPacket packet)
        {
            NetOutgoingMessage msg = server.CreateMessage();
            msg.Write((byte)packet.Type);
            packet.Encode(msg);
            server.SendToAll(msg, NetDeliveryMethod.ReliableUnordered);
        }

        public void Send(NetConnection conn, IPacket packet)
        {
            if (packet == null)
                return;
            NetOutgoingMessage msg = server.CreateMessage();
            msg.Write((byte)packet.Type);
            packet.Encode(msg);
            server.SendMessage(msg, conn, NetDeliveryMethod.ReliableUnordered);
        }
    }
}
