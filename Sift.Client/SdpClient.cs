using System.Threading;
using System.Windows;
using System.Windows.Threading;

using Sift.Common.Network;

namespace Sift.Client
{
    /*public class SdpClient : PacketConsumer
    {
        protected override NetPeer Peer => Client;

        public NetClient Client { get; }

        public bool Connected => Client.ConnectionStatus == NetConnectionStatus.Connected;

        public SdpClient()
        {
            NetPeerConfiguration config = new NetPeerConfiguration(App);
            config.AutoFlushSendQueue = true;
            config.ConnectionTimeout = 5.0f;

            var context = new DispatcherSynchronizationContext(Application.Current.Dispatcher);
            SynchronizationContext.SetSynchronizationContext(context);

            Client = new NetClient(config);
            Client.RegisterReceivedCallback(new SendOrPostCallback(GotMessage));
            Client.Start();
        }

        public void Disconnect()
        {
            Client?.Disconnect("Client closed");
        }

        public void Connect(string address, int port, NetworkUser user)
        {
            Client.Connect(address, port, handshake(user));
        }

        private NetOutgoingMessage handshake(NetworkUser user)
        {
            NetOutgoingMessage msg = Peer.CreateMessage();
            msg.Write(App);
            msg.Write(Version);
            user.Encode(msg);
            return msg;
        }

        public void Send(IPacket packet)
        {
            NetOutgoingMessage msg = Client.CreateMessage();
            msg.Write((byte)packet.Type);
            packet.Encode(msg);
            Client.SendMessage(msg, NetDeliveryMethod.ReliableUnordered);
            Client.FlushSendQueue();
        }
    }*/
}
