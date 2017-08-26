using System;

namespace Sift.Common
{
    public interface IVoipProvider
    {
        event EventHandler<Caller> CallerStart;
        event EventHandler<Caller> CallerEnd;

        event EventHandler<Destination> DestinationStart;
        event EventHandler<Destination> DestinationEnd;

        bool Connected { get; }

        void Call(Caller from, string number);
        void Connect();
        void Ring(string channelId);
        void Busy(string channelId);
        void Hangup(string channelId);
    }
}
