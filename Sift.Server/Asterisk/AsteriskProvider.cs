using System;
using System.Collections.Generic;

using AsterNET.ARI;
using AsterNET.ARI.Middleware;
using AsterNET.ARI.Models;
using IniParser.Model;

using Sift.Common;

namespace Sift.Server.Asterisk
{
    internal class AsteriskProvider : VoipProviderBase
    {
        public const string AppName = "Sift";

        public override VoipProviders Type => VoipProviders.Asterisk;
        public override string Name => VoipProviderFactory.PrettyNames[Type];

        public override event EventHandler<Caller> CallerStart;
        public override event EventHandler<Caller> CallerEnd;

        public override event EventHandler<Destination> DestinationStart;
        public override event EventHandler<Destination> DestinationEnd;
        public override event EventHandler<VoipProviderConnectionState> ConnectionStateChanged;

        public override bool Connected => Client.Connected;

        public AriClient Client { get; }

        private readonly Dictionary<string, Caller> callerRegistry = new Dictionary<string, Caller>();

        public readonly Dictionary<string, Destination> DestinationRegistry = new Dictionary<string, Destination>();

        public AsteriskProvider(KeyDataCollection data) : base(data)
        {
            string host = data["host"];
            int port = int.Parse(data["port"]);
            string user = data["username"];
            string secret = data["secret"];
            string app = data["app"];

            Client = new AriClient(new StasisEndpoint(host, port, user, secret), app);
            Client.OnConnectionStateChanged += Client_OnConnectionStateChanged;

            Client.OnStasisStartEvent += Client_OnStasisStartEvent;
            Client.OnStasisEndEvent += Client_OnStasisEndEvent;
        }

        private void Client_OnConnectionStateChanged(object sender)
        {
            VoipProviderConnectionState state;
            switch (Client.ConnectionState)
            {
                case ConnectionState.None:
                    state = VoipProviderConnectionState.None;
                    break;
                case ConnectionState.Connecting:
                    state = VoipProviderConnectionState.Connecting;
                    break;
                case ConnectionState.Open:
                    state = VoipProviderConnectionState.Open;
                    break;
                case ConnectionState.Closing:
                    state = VoipProviderConnectionState.Closing;
                    break;
                case ConnectionState.Closed:
                    state = VoipProviderConnectionState.Closed;
                    break;
                default:
                    Console.WriteLine($"Asterisk: Received unknown connection state {Client.ConnectionState}");
                    return;
            }
            ConnectionStateChanged?.Invoke(this, state);
        }

        public override void Connect() => Client.Connect();

        private void Client_OnStasisStartEvent(IAriClient sender, StasisStartEvent e)
        {
            int argI;
            if (e.Args.Count < 1 || !int.TryParse(e.Args[0], out argI) || callerRegistry.ContainsKey(e.Channel.Id))
                return;
            StasisArgs arg = (StasisArgs)argI;
            switch (arg)
            {
                case StasisArgs.Caller:
                    Caller c = new Caller(e.Channel.Id)
                    {
                        Number = e.Channel.Caller.Number,
                        Created = e.Channel.Creationtime
                    };
                    Logger.Log(c, "New caller with ID: " + e.Channel.Id);
                    CallerStart?.Invoke(this, c);
                    callerRegistry[e.Channel.Id] = c;
                    break;
                case StasisArgs.Screener:
                    string name = e.Channel.Name;
                    Destination s = new Destination(name.Substring(0, name.IndexOf('-')))
                    {
                        Id = e.Channel.Id
                    };
                    DestinationStart?.Invoke(this, s);
                    DestinationRegistry[e.Channel.Id] = s;
                    break;
                default:
                    throw new NotSupportedException("Stasis argument unsupported");
            }
        }

        private void Client_OnStasisEndEvent(IAriClient sender, StasisEndEvent e)
        {
            if (callerRegistry.ContainsKey(e.Channel.Id))
            {
                Caller c = callerRegistry[e.Channel.Id];
                Logger.Log(c, "Caller hungup.");
                CallerEnd?.Invoke(this, c);
                callerRegistry.Remove(e.Channel.Id);
                return;
            }
            if (DestinationRegistry.ContainsKey(e.Channel.Id))
            {
                DestinationEnd?.Invoke(this, DestinationRegistry[e.Channel.Id]);
                DestinationRegistry.Remove(e.Channel.Id);
                return;
            }
        }

        public override void Ring(string channelId)
        {
            Client.Channels.Ring(channelId);
        }

        public override void Hangup(string channelId)
        {
            Client.Channels.Hangup(channelId);
        }

        public override void Call(Caller from, string number)
        {
            number = "SIP/" + number;
            Client.Channels.Originate(number, app: "hello-world", appArgs: ((int)StasisArgs.Screener).ToString(), callerId: from.Number);
        }

        public override void Busy(string channelId)
        {
        }

        private enum StasisArgs
        {
            None,
            Caller,
            Screener,
        }
    }

    
}
