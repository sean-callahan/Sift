using System;

using WebSocketSharp;
using WebSocketSharp.Server;

namespace Sift.Common.Net
{
    internal class SdpService : WebSocketBehavior
    {
        public SdpManager Manager { get; }

        public SdpService(SdpManager manager)
        {
            Manager = manager;
        }

        protected override void OnOpen()
        {
            Manager.NewConnection(ID);
        }

        protected override void OnError(ErrorEventArgs e)
        {
            Manager.Exception(e.Exception);
        }

        protected override void OnClose(CloseEventArgs e)
        {
            Manager.NewDisconnection(ID);
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            if (!e.IsBinary)
                return; // Invalid data format. Skipping message.

            var packet = SdpCodec.Decode(e.RawData);
            if (packet == null)
            {
                // failed to decode packet
                return;
            }

            Manager.IncomingPackets.Enqueue(new Tuple<string, ISdpPacket>(ID, packet));
        }
    }
}
