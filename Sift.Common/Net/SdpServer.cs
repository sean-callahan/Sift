using System.Collections.Generic;

using WebSocketSharp.Server;

namespace Sift.Common.Net
{
    public class SdpServer
    {
        private const string ServicePath = "/sift";

        public WebSocketServer Listener { get; }
        public WebSocketServiceHost Service => Listener?.WebSocketServices[ServicePath];

        public HashSet<string> Connections = new HashSet<string>();

        public SdpManager Manager { get; }

        public SdpServer(int port)
        {
            Listener = new WebSocketServer(port, false);
            Manager = new SdpManager();
            Manager.Connection += Manager_Connection;
            Manager.Disconnected += Manager_Disconnected;
            Listener.AddWebSocketService(ServicePath, () => new SdpService(Manager));
        }

        private void Manager_Disconnected(string id)
        {
            Connections.Remove(id);
        }

        private void Manager_Connection(string id)
        {
            Connections.Add(id);
        }

        public void Start()
        {
            Listener.Start();
        }

        public void SendTo(string id, ISdpPacket packet)
        {
            byte[] data = SdpCodec.Encode(packet);
            if (data.Length == 0)
                return;
            Service.Sessions.SendToAsync(data, id, null);
        }

        public void Broadcast(ISdpPacket packet)
        {
            byte[] data = SdpCodec.Encode(packet);
            if (data.Length == 0)
                return;
            Service.Sessions.BroadcastAsync(data, null);
        }
    }
}
