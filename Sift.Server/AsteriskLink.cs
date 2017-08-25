using System;

using AsterNET.ARI.Models;
using Sift.Common;
using Sift.Common.Network;

namespace Sift.Server
{
    internal class AsteriskLink : IDisposable
    {
        public Guid Id { get; }
        public Caller Caller { get; }
        public Screener Screener { get; private set; }

        private Program program;
        private Asterisk asterisk;
        private Bridge bridge;

        private bool ended = false;
        public event EventHandler End;

        public AsteriskLink(Program program, Asterisk asterisk, Caller c)
        {
            if (program.LinkedCallers.ContainsKey(c))
                program.LinkedCallers[c].Dispose();

            program.LinkedCallers[c] = this;

            Id = Guid.NewGuid();
            Caller = c;

            this.asterisk = asterisk;
            this.program = program;

            bridge = asterisk.Client.Bridges.Create("mixing", Id.ToString(), Asterisk.AppName);
        }

        public void Dial(string exten)
        {
            Screener = new Screener(exten);

            asterisk.Call(Caller, exten);
            asterisk.ScreenerStart += Asterisk_ScreenerStart;
        }

        private void Asterisk_ScreenerStart(object sender, Screener e)
        {
            if (Screener == null || e.Extension != "SIP/"+Screener.Extension)
                return;

            Screener.Id = e.Id;
            asterisk.Client.Bridges.AddChannel(bridge.Id, e.Id);

            Console.WriteLine("Screener start " + e.Id);

            asterisk.Client.Channels.Answer(Caller.Id);
            asterisk.Client.Bridges.AddChannel(bridge.Id, Caller.Id);

            asterisk.ScreenerEnd += Asterisk_ScreenerEnd;
            asterisk.CallerEnd += Asterisk_CallerEnd;
            asterisk.ScreenerStart -= Asterisk_ScreenerStart;
        }

        private void Asterisk_CallerEnd(object sender, Caller e)
        {
            try
            {
                asterisk.Client.Channels.Hangup(Screener.Id);
            }
            catch (Exception)
            {

            }
            asterisk.CallerEnd -= Asterisk_CallerEnd;
        }

        private void Asterisk_ScreenerEnd(object sender, Screener e)
        {
            if (e.Extension != "SIP/" + Screener.Extension)
                return;
            try
            {
                asterisk.Client.Bridges.RemoveChannel(bridge.Id, e.Id);
                asterisk.Client.Bridges.RemoveChannel(bridge.Id, Caller.Id);
            }
            catch (Exception)
            {
            }

            asterisk.ScreenerEnd -= Asterisk_ScreenerEnd;

            Dispose();
        }

        public void Dispose()
        {
            try
            {
                foreach (string cid in bridge.Channels)
                    asterisk.Client.Bridges.RemoveChannel(bridge.Id, cid);
                asterisk.Client.Bridges.Destroy(bridge.Id);
                program.LinkedCallers.Remove(Caller);
                Console.WriteLine("bridge: " + Id + " destroyed");
                if (!ended)
                    End?.Invoke(this, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
