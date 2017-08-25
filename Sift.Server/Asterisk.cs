using System;
using System.Collections.Generic;

using AsterNET.ARI;
using AsterNET.ARI.Models;
using Sift.Common;

namespace Sift.Server
{
    internal class Asterisk : IVoipProvider
    {
        public const string AppName = "Sift";

        public event EventHandler<Caller> CallerStart;
        public event EventHandler<Caller> CallerEnd;

        public event EventHandler<Screener> ScreenerStart;
        public event EventHandler<Screener> ScreenerEnd;

        public bool Connected => Client.Connected;

        public AriClient Client { get; }

        private Dictionary<string, Caller> callerRegistry = new Dictionary<string, Caller>();
        public Dictionary<string, Screener> ScreenerRegistry = new Dictionary<string, Screener>();

        private Dictionary<Guid, Group> groupRegistry = new Dictionary<Guid, Group>();

        public Asterisk(string addr, int port, string user, string secret, string app)
        {
            Client = new AriClient(new StasisEndpoint(addr, port, user, secret), app);

            Client.OnStasisStartEvent += Client_OnStasisStartEvent;
            Client.OnStasisEndEvent += Client_OnStasisEndEvent;
        }

        public void Connect() => Client.Connect();

        private void Client_OnStasisStartEvent(IAriClient sender, StasisStartEvent e)
        {
            int argI;
            if (e.Args.Count < 1 || !int.TryParse(e.Args[0], out argI) || callerRegistry.ContainsKey(e.Channel.Id))
                return;
            StasisArgs arg = (StasisArgs)argI;
            Console.WriteLine("stasis start with arg " + arg);
            switch (arg)
            {
                case StasisArgs.Caller:
                    Caller c = new Caller(e.Channel.Id);
                    c.Number = e.Channel.Caller.Number;
                    c.Created = e.Channel.Creationtime;
                    CallerStart?.Invoke(this, c);
                    callerRegistry[e.Channel.Id] = c;
                    break;
                case StasisArgs.Screener:
                    string name = e.Channel.Name;
                    Screener s = new Screener(name.Substring(0, name.IndexOf('-')));
                    s.Id = e.Channel.Id;
                    ScreenerStart?.Invoke(this, s);
                    ScreenerRegistry[e.Channel.Id] = s;
                    break;
                default:
                    throw new NotSupportedException("Stasis argument unsupported");
            }
        }

        private void Client_OnStasisEndEvent(IAriClient sender, StasisEndEvent e)
        {
            if (callerRegistry.ContainsKey(e.Channel.Id))
            {
                CallerEnd?.Invoke(this, callerRegistry[e.Channel.Id]);
                callerRegistry.Remove(e.Channel.Id);
                return;
            }
            if (ScreenerRegistry.ContainsKey(e.Channel.Id))
            {
                ScreenerEnd?.Invoke(this, ScreenerRegistry[e.Channel.Id]);
                ScreenerRegistry.Remove(e.Channel.Id);
                return;
            }
        }

        public void Ring(string channelId)
        {
            Client.Channels.Ring(channelId);
        }

        public void Hangup(string channelId)
        {
            Client.Channels.Hangup(channelId);
        }

        public void Call(Caller from, string number)
        {
            number = "SIP/" + number;
            Client.Channels.Originate(number, app: "hello-world", appArgs: ((int)StasisArgs.Screener).ToString(), callerId: from.Number);
        }

        public void Busy(string channelId)
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
