using System;

using Lidgren.Network;

namespace Sift.Common.Network
{
    public abstract class PacketConsumer
    {
        public event EventHandler<Exception> Error;

        protected abstract NetPeer Peer { get; }

        public void ReadMessages(object sender, EventArgs e)
        {
            while (Network.AppStillIdle)
            {
                NetIncomingMessage im;
                while ((im = Peer.ReadMessage()) != null)
                {
                    switch (im.MessageType)
                    {
                        case NetIncomingMessageType.StatusChanged:

                            break;
                        case NetIncomingMessageType.Data:
                            PacketType type = (PacketType)im.ReadByte();
                            try
                            {
                                handlePacket(type, im);
                            }
                            catch (Exception ex)
                            {
                                Error?.Invoke(this, ex);
                            }
                            break;
                        default:

                            break;
                    }
                    Peer.Recycle(im);
                }
            }
        }

        private void handlePacket(PacketType type, NetIncomingMessage msg)
        {
            switch (type)
            {
                case PacketType.UpdateAppState:
                    UpdateAppState?.Invoke(msg.SenderConnection, new UpdateAppState(msg));
                    break;
                case PacketType.UpdateLineState:
                    UpdateLineState?.Invoke(msg.SenderConnection, new UpdateLineState(msg));
                    break;
                case PacketType.LoginRequest:
                    LoginRequest?.Invoke(msg.SenderConnection, new LoginRequest(msg));
                    break;
                case PacketType.RequestDump:
                    RequestDump?.Invoke(msg.SenderConnection, new RequestDump(msg));
                    break;
                case PacketType.RequestScreen:
                    RequestScreen?.Invoke(msg.SenderConnection, new RequestScreen(msg));
                    break;
            }
        }

        public event EventHandler<UpdateAppState> UpdateAppState;
        public event EventHandler<UpdateLineState> UpdateLineState;
        public event EventHandler<LoginRequest> LoginRequest;
        public event EventHandler<RequestDump> RequestDump;
        public event EventHandler<RequestScreen> RequestScreen;
    }
}
