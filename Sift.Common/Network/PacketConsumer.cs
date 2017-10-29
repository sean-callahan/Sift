using System;
using System.Diagnostics;

namespace Sift.Common.Network
{
    /*public abstract class PacketConsumer
    {
        public const string App = "sift";
        public const byte Version = 0;

        protected abstract NetPeer Peer { get; }

        protected void GotMessage(object peer)
        {
            var im = ((NetPeer)peer).ReadMessage();
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
                    if (peer.GetType() == typeof(NetClient))
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

            NetworkUser user = new NetworkUser();
            user.Decode(msg);

            RequestUserLogin?.Invoke(sender, user);
        }

        private void handlePacket(PacketType type, NetIncomingMessage msg)
        {
            Debug.WriteLine("Got a " + type);

            switch (type)
            {
                case PacketType.UpdateAppState:
                    UpdateAppState?.Invoke(msg.SenderConnection, new UpdateAppState(msg));
                    break;
                case PacketType.InitializeLine:
                    InitializeLine?.Invoke(msg.SenderConnection, new InitializeLine(msg));
                    break;
                case PacketType.LoginRequest:
                    LoginRequest?.Invoke(msg.SenderConnection, new LoginRequest(msg));
                    break;
                case PacketType.DumpLine:
                    DumpLine?.Invoke(msg.SenderConnection, new DumpLine(msg));
                    break;
                case PacketType.ScreenLine:
                    ScreenLine?.Invoke(msg.SenderConnection, new ScreenLine(msg));
                    break;
                case PacketType.HoldLine:
                    HoldLine?.Invoke(msg.SenderConnection, new HoldLine(msg));
                    break;
                case PacketType.LineRequest:
                    LineRequest?.Invoke(msg.SenderConnection, new LineRequest(msg));
                    break;
                case PacketType.AirLine:
                    AirLine?.Invoke(msg.SenderConnection, new AirLine(msg));
                    break;
                case PacketType.ErrorPacket:
                    Error?.Invoke(msg.SenderConnection, new ErrorPacket(msg));
                    break;
                case PacketType.UpdateSettings:
                    UpdateSettings?.Invoke(msg.SenderConnection, new UpdateSettings(msg));
                    break;
                case PacketType.RequestSettings:
                    RequestSettings?.Invoke(msg.SenderConnection, new RequestSettings(msg));
                    break;
                case PacketType.LineMetadata:
                    LineMetadata?.Invoke(msg.SenderConnection, new LineMetadata(msg));
                    break;
            }
        }


        public event EventHandler<string> Disconnected;
        public event EventHandler ConnectionSuccess;
        
        public event EventHandler<UpdateAppState> UpdateAppState;
        public event EventHandler<InitializeLine> InitializeLine;

        public event EventHandler<LoginRequest> LoginRequest;
        public event EventHandler<DumpLine> DumpLine;
        public event EventHandler<ScreenLine> ScreenLine;
        public event EventHandler<HoldLine> HoldLine;
        public event EventHandler<LineRequest> LineRequest;
        public event EventHandler<AirLine> AirLine;
        public event EventHandler<NetworkUser> RequestUserLogin;

        public event EventHandler<ErrorPacket> Error;
        public event EventHandler<UpdateSettings> UpdateSettings;
        public event EventHandler<RequestSettings> RequestSettings;
        public event EventHandler<LineMetadata> LineMetadata;
    }*/
}
