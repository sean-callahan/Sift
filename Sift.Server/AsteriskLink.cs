using System;

using AsterNET.ARI.Models;

using Sift.Common;

namespace Sift.Server
{
    internal class AsteriskLink : IProviderLink<Asterisk>, IDisposable
    {
        public Guid Id { get; }

        public Asterisk Provider { get; }

        public Caller Originator { get; }

        public IDestination Destination { get; }

        private Program program;
        private Bridge bridge;

        private bool ended = false;
        public event EventHandler End;

        public AsteriskLink(Program program, Asterisk asterisk, Caller c, string exten)
        {
            if (program.LinkedCallers.ContainsKey(c))
                program.LinkedCallers[c].Dispose();

            program.LinkedCallers[c] = this;

            Id = Guid.NewGuid();
            Provider = asterisk;
            Originator = c;
            Destination = new Destination(exten);
            
            this.program = program;

            bridge = asterisk.Client.Bridges.Create("mixing", Id.ToString(), Asterisk.AppName);
            Provider.Client.OnBridgeDestroyedEvent += Client_OnBridgeDestroyedEvent;
        }

        private void Client_OnBridgeDestroyedEvent(AsterNET.ARI.IAriClient sender, BridgeDestroyedEvent e)
        {
            if (e.Bridge.Id == bridge.Id)
                Dispose();
        }

        public void Start()
        {
            Provider.Call(Originator, Destination.EndPoint);
            Provider.DestinationStart += Asterisk_ScreenerStart;
        }

        private void Asterisk_ScreenerStart(object sender, Destination e)
        {
            if (Destination == null || e.EndPoint != "SIP/" + Destination.EndPoint)
                return;

            Destination.Id = e.Id;
            Provider.Client.Bridges.AddChannel(bridge.Id, e.Id);

            Console.WriteLine("Screener start " + e.Id);

            Provider.Client.Channels.Answer(Originator.Id);
            Provider.Client.Bridges.AddChannel(bridge.Id, Originator.Id);

            Provider.DestinationEnd += Asterisk_ScreenerEnd;
            Provider.CallerEnd += Asterisk_CallerEnd;
            Provider.DestinationStart -= Asterisk_ScreenerStart;
        }

        private void Asterisk_CallerEnd(object sender, Caller e)
        {
            try
            {
                Provider.Client.Channels.Hangup(Destination.Id);
            }
            catch (Exception)
            {

            }
            Provider.CallerEnd -= Asterisk_CallerEnd;
        }

        private void Asterisk_ScreenerEnd(object sender, Destination e)
        {
            if (e.EndPoint != "SIP/" + Destination.EndPoint)
                return;
            try
            {
                Provider.Client.Bridges.RemoveChannel(bridge.Id, e.Id);
                Provider.Client.Bridges.RemoveChannel(bridge.Id, Originator.Id);
            }
            catch (Exception)
            {
            }

            Provider.DestinationEnd -= Asterisk_ScreenerEnd;

            Dispose();
        }

        public void Dispose()
        {
            try
            {
                foreach (string cid in bridge.Channels)
                    Provider.Client.Bridges.RemoveChannel(bridge.Id, cid);
                try { Provider.Hangup(Destination.Id); } catch (Exception) { }
                program.LinkedCallers.Remove(Originator);
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
