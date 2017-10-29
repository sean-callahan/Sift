using System;

using WebSocketSharp;

namespace Sift.Common.Net
{
    public class SdpClient : IDisposable
    {
        public WebSocket Client { get; private set; }
        public SdpManager Manager { get; }

        public bool IsConnected => Client != null ? Client.IsAlive : false;

        private const string ServicePath = "/sift";

        public SdpClient()
        {
            Manager = new SdpManager();
        }

        private void Client_OnMessage(object sender, MessageEventArgs e)
        {
            if (!e.IsBinary)
                return; // Invalid data format. Skipping message.

            var packet = SdpCodec.Decode(e.RawData);
            if (packet == null)
            {
                // failed to decode packets
                return;
            }

            Manager.IncomingPackets.Enqueue(new Tuple<string, ISdpPacket>(string.Empty, packet));
        }

        public void Send(ISdpPacket packet)
        {
            if (!(Client.IsAlive && Client.ReadyState == WebSocketState.Open))
                return;

            byte[] data = SdpCodec.Encode(packet);
            if (data.Length == 0)
                return;
            Client.SendAsync(data, null);
        }
       
        public void Connect(string host, int port)
        {
            if (Client != null)
                throw new ArgumentException("Already called Connect.");
            Client = new WebSocket($"ws://{host}:{port}{ServicePath}");
            Client.OnMessage += Client_OnMessage;
            Client?.Connect();
        }

        public void Disconnect()
        {
            Client?.Close();
        }

        public void Dispose()
        {
            Client?.Close();
        }
    }
}
