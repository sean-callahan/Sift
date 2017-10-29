using System.Net;
using System.Threading;

using Sift.Common.Network;

namespace Sift.Server
{
    /*internal class SdpServer : PacketConsumer
    {
        private NetServer server;

        protected override NetPeer Peer => server;

        public SdpServer(int port)
        {
            NetPeerConfiguration config = new NetPeerConfiguration(App);
            config.AutoFlushSendQueue = true;
            config.Port = port;
            config.BroadcastAddress = IPAddress.Any;
            config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);

            if (SynchronizationContext.Current == null)
                SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

            server = new NetServer(config);
            server.RegisterReceivedCallback(new SendOrPostCallback(GotMessage));
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
    }*/
}
