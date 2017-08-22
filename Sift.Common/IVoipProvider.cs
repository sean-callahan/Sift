using System;

namespace Sift.Common
{
    public interface IVoipProvider
    {
        event EventHandler<Caller> CallerStart;
        event EventHandler<Caller> CallerEnd;

        event EventHandler<Screener> ScreenerStart;
        event EventHandler<Screener> ScreenerEnd;

        void Call(Caller from, string number);
        void Connect();
        void Ring(Caller c);
        void Busy(Caller c);
        void Hangup(Caller c);
    }
}
