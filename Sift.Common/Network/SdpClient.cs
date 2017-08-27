using Lidgren.Network;

namespace Sift.Common.Network
{
    public class SdpClient : PacketConsumer
    {
        protected override NetPeer Peer => Client;

        public NetClient Client { get; }

        public bool Connected => Client.ConnectionStatus == NetConnectionStatus.Connected;

        public SdpClient()
        {
            NetPeerConfiguration config = new NetPeerConfiguration(App);
            config.AutoFlushSendQueue = true;
            config.ConnectionTimeout = 5.0f;

            Client = new NetClient(config);
            Client.Start();
        }

        public void Disconnect()
        {
            Client?.Disconnect("Client closed");
        }

        public void Connect(string address, int port)
        {
            Client.Connect(address, port, handshake());
        }

        private NetOutgoingMessage handshake()
        {
            NetOutgoingMessage msg = Peer.CreateMessage();
            msg.Write(App);
            msg.Write(Version);
            return msg;
        }

        public void Send(IPacket packet)
        {
            NetOutgoingMessage msg = Client.CreateMessage();
            msg.Write((byte)packet.Type);
            packet.Encode(msg);
            Client.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
            Client.FlushSendQueue();
        }
    }
}
