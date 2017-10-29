using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Sift.Common.Net
{
    public class SdpManager
    {
        public ConcurrentQueue<Tuple<string, ISdpPacket>> IncomingPackets = new ConcurrentQueue<Tuple<string, ISdpPacket>>();

        public void HandlePacket(string id, ISdpPacket packet)
        {
            switch (packet.Type)
            {
                case PacketType.InitializeClient:
                    InitializeClient?.Invoke(id, (InitializeClient)packet);
                    break;
                case PacketType.LineAction:
                    HandleAction(id, (LineAction)packet);
                    break;
                case PacketType.InitializeLine:
                    InitializeLine?.Invoke(id, (InitializeLine)packet);
                    break;
                case PacketType.LineStateChanged:
                    LineStateChanged?.Invoke(id, (LineStateChanged)packet);
                    break;
                case PacketType.LineMetadata:
                    LineMetadata?.Invoke(id, (LineMetadata)packet);
                    break;
                case PacketType.Error:
                    Error?.Invoke(id, (Error)packet);
                    break;
                case PacketType.Settings:
                    Settings?.Invoke(id, (Settings)packet);
                    break;
                case PacketType.SettingsQuery:
                    SettingsQuery?.Invoke(id, (SettingsQuery)packet);
                    break;
                default:
                    throw new NotImplementedException("Packet type doesn't have a registered handler.");
            }
        }

        private void HandleAction(string id, LineAction lineAction)
        {
            switch (lineAction.Action)
            {
                case LineActions.Air:
                    Air?.Invoke(id, lineAction.Index);
                    break;
                case LineActions.Dump:
                    Dump?.Invoke(id, lineAction.Index);
                    break;
                case LineActions.Screen:
                    Screen?.Invoke(id, lineAction.Index);
                    break;
                case LineActions.Hold:
                    Hold?.Invoke(id, lineAction.Index);
                    break;
            }
        }

        public event Action<string, InitializeClient> InitializeClient;
        public event Action<string, InitializeLine> InitializeLine;
        public event Action<string, LineStateChanged> LineStateChanged;
        public event Action<string, LineMetadata> LineMetadata;
        public event Action<string, Error> Error;
        public event Action<string, Settings> Settings;
        public event Action<string, SettingsQuery> SettingsQuery;

        // Line Actions
        public event Action<string, byte> Air;
        public event Action<string, byte> Dump;
        public event Action<string, byte> Screen;
        public event Action<string, byte> Hold;

        internal static readonly Dictionary<PacketType, Type> PacketTypes;

        public event Action<string> Connection;
        public event EventHandler<Exception> NetException;

        public event Action<string> Disconnected;

        internal void NewConnection(string id) => Connection?.Invoke(id);
        internal void Exception(Exception e) => NetException?.Invoke(this, e);
        internal void NewDisconnection(string id) => Disconnected?.Invoke(id);

        static SdpManager()
        {
            PacketTypes = new Dictionary<PacketType, Type>
            {
                { PacketType.InitializeClient, typeof(InitializeClient) },
                { PacketType.LineAction, typeof(LineAction) },
                { PacketType.InitializeLine, typeof(InitializeLine) },
                { PacketType.LineStateChanged, typeof(LineStateChanged) },
                { PacketType.LineMetadata, typeof(LineMetadata) },
                { PacketType.Error, typeof(Error) },
                { PacketType.Settings, typeof(Settings) },
            };
        }
    }
}
