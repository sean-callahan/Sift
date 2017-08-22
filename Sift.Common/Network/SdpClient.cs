using System.Net;

using Lidgren.Network;

namespace Sift.Common.Network
{
    public class SdpClient : PacketConsumer
    {
        public const string App = "sift";

        protected override NetPeer Peer => Client;

        public NetClient Client { get; }

        public bool Connected => Client.ConnectionStatus == NetConnectionStatus.Connected;

        public SdpClient()
        {
            NetPeerConfiguration config = new NetPeerConfiguration(App);
            config.AutoFlushSendQueue = false;

            Client = new NetClient(config);
        }

        public void Connect(string address, int port)
        {
            Client.Start();
            Client.Connect(new IPEndPoint(IPAddress.Parse(address), port));
        }

        public void Send(IPacket packet)
        {
            NetOutgoingMessage msg = Client.CreateMessage();
            msg.Write((byte)packet.Type);
            packet.Encode(msg);
            Client.SendMessage(msg, NetDeliveryMethod.ReliableUnordered);
            Client.FlushSendQueue();
        }
    }
}
