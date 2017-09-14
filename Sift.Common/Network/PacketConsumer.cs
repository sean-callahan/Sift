using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Lidgren.Network;

namespace Sift.Common.Network
{
    public abstract class PacketConsumer
    {
        public const string App = "sift";
        public const byte Version = 0;

        protected abstract NetPeer Peer { get; }

        public void TryReadMessage()
        {
            NetIncomingMessage im;
            while ((im = Peer.ReadMessage()) != null)
            {
                switch (im.MessageType)
                {
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.ErrorMessage:
                        Error?.Invoke(this, new ErrorPacket(new Exception(im.ReadString())));
                        break;
                    case NetIncomingMessageType.Data:
                        PacketType type = (PacketType)im.ReadByte();
                        try
                        {
                            handlePacket(type, im);
                        }
                        catch (Exception ex)
                        {
                            Error?.Invoke(this, new ErrorPacket(ex));
                        }
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();
                        string reason = im.ReadString();
                        if (Peer.GetType() == typeof(NetClient))
                        {
                            if (status == NetConnectionStatus.Connected)
                                ConnectionSuccess?.Invoke(im.SenderConnection, null);
                            if (status == NetConnectionStatus.Disconnected)
                                Disconnected?.Invoke(this, reason);
                        }
                        break;
                    case NetIncomingMessageType.ConnectionApproval:
                        vetConnection(im.SenderConnection, im);
                        break;
                    default:
                        break;
                }
                Peer.Recycle(im);
            }
        }

        private void vetConnection(NetConnection sender, NetIncomingMessage msg)
        {
            string app = msg.ReadString();
            byte version = msg.ReadByte();

            if (app != App)
            {
                sender.Deny("Not a Sift-compatible server.");
                return;
            }
            if (version != Version)
            {
                sender.Deny("Unsupported server version.");
                return;
            }

            User user = new User();
            user.Decode(msg);

            RequestUserLogin?.Invoke(sender, user);
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
                case PacketType.RequestHold:
                    RequestHold?.Invoke(msg.SenderConnection, new RequestHold(msg));
                    break;
                case PacketType.RequestLine:
                    RequestLine?.Invoke(msg.SenderConnection, new RequestLine(msg));
                    break;
                case PacketType.RequestAir:
                    RequestAir?.Invoke(msg.SenderConnection, new RequestAir(msg));
                    break;
                case PacketType.ErrorPacket:
                    Error?.Invoke(msg.SenderConnection, new ErrorPacket(msg));
                    break;
                case PacketType.SettingsChanged:
                    SettingsChanged?.Invoke(msg.SenderConnection, new SettingsChanged(msg));
                    break;
            }
        }


        public event EventHandler<string> Disconnected;
        public event EventHandler ConnectionSuccess;
        
        public event EventHandler<UpdateAppState> UpdateAppState;
        public event EventHandler<UpdateLineState> UpdateLineState;

        public event EventHandler<LoginRequest> LoginRequest;
        public event EventHandler<RequestDump> RequestDump;
        public event EventHandler<RequestScreen> RequestScreen;
        public event EventHandler<RequestHold> RequestHold;
        public event EventHandler<RequestLine> RequestLine;
        public event EventHandler<RequestAir> RequestAir;
        public event EventHandler<User> RequestUserLogin;

        public event EventHandler<ErrorPacket> Error;
        public event EventHandler<SettingsChanged> SettingsChanged;
    }
}
