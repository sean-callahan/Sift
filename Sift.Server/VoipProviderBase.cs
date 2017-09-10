using System;
using System.Collections.Generic;

using IniParser.Model;

using Sift.Common;

namespace Sift.Server
{
    internal abstract class VoipProviderBase : IVoipProvider
    {
        public VoipProviderBase(KeyDataCollection data)
        {
        }

        public abstract VoipProviders Type { get; }
        public abstract string Name { get; }

        public abstract bool Connected { get; }

        public abstract event EventHandler<VoipProviderConnectionState> ConnectionStateChanged;
        public abstract event EventHandler<Caller> CallerStart;
        public abstract event EventHandler<Caller> CallerEnd;
        public abstract event EventHandler<Destination> DestinationStart;
        public abstract event EventHandler<Destination> DestinationEnd;

        public abstract void Busy(string channelId);
        public abstract void Call(Caller from, string number);
        public abstract void Connect();
        public abstract void Hangup(string channelId);
        public abstract void Ring(string channelId);
    }
}
