using System;

using AsterNET.ARI.Models;

using Sift.Common;

namespace Sift.Server.Asterisk
{
    internal class AsteriskLink : IProviderLink<AsteriskProvider>, IDisposable
    {
        public Guid Id { get; }

        public AsteriskProvider Provider { get; }

        public Caller Originator { get; }

        public IDestination Destination { get; }

        private Program program;

        public AsteriskLink(Program program, AsteriskProvider asterisk, Caller c, string exten)
        {
            Id = Guid.NewGuid();
            Provider = asterisk;
            Originator = c;
            Destination = new Destination(exten);
            
            this.program = program;

            asterisk.Client.Bridges.Create("mixing", Id.ToString(), AsteriskProvider.AppName);
        }

        public void Start()
        {
            Provider.Call(Originator, Destination.EndPoint);
            Provider.DestinationStart += Asterisk_DestinationStart;
        }

        private void Asterisk_DestinationStart(object sender, Destination e)
        {
            if (Destination == null || e.EndPoint != "SIP/" + Destination.EndPoint)
                return;

            Logger.Log("[" + Originator.Number + "->" + e.EndPoint + "] Destination picked up.");

            Destination.Id = e.Id;
            Provider.Client.Bridges.AddChannel(Id.ToString(), e.Id);

            Provider.Client.Channels.Answer(Originator.Id);
            Provider.Client.Bridges.AddChannel(Id.ToString(), Originator.Id);

            Provider.DestinationEnd += Asterisk_DestinationEnd;
            Provider.CallerEnd += Asterisk_CallerEnd;
            Provider.DestinationStart -= Asterisk_DestinationStart;
        }

        private void Asterisk_CallerEnd(object sender, Caller e)
        {
            if (e.Id != Originator.Id)
                return;
            try
            {
                Provider.Client.Channels.Hangup(Destination.Id);
            }
            catch (Exception ex)
            {
                Logger.Log(ex, Logger.Level.Warning);
            }

            Logger.Log("[" + Originator.Number + "->" + Destination.EndPoint + "] Caller hung up.");

            Provider.CallerEnd -= Asterisk_CallerEnd;
        }

        private void Asterisk_DestinationEnd(object sender, Destination e)
        {
            if (e.EndPoint != "SIP/" + Destination.EndPoint)
                return;
            try
            {
                Provider.Client.Bridges.RemoveChannel(Id.ToString(), e.Id);
                Provider.Client.Bridges.RemoveChannel(Id.ToString(), Originator.Id);
            }
            catch (Exception ex)
            {
                Logger.Log(ex, Logger.Level.Warning);
            }

            Logger.Log("[" + Originator.Number + "->" + e.EndPoint + "] Destination hung up.");

            Provider.DestinationEnd -= Asterisk_DestinationEnd;

            Dispose();
        }

        public void Dispose()
        {
            try
            {
                Bridge bridge = Provider.Client.Bridges.Get(Id.ToString());
                if (bridge == null)
                    return;

                foreach (string cid in bridge.Channels)
                    Provider.Client.Bridges.RemoveChannel(Id.ToString(), cid);

                try { Provider.Hangup(Destination.Id); } catch (Exception) { }

                program.LinkedCallers.Remove(Originator);

                Provider.CallerEnd -= Asterisk_CallerEnd;
                Provider.DestinationEnd -= Asterisk_DestinationEnd;

                Provider.Client.Bridges.Destroy(Id.ToString());
                Logger.Log("[" + Originator.Number + "->" + Destination.EndPoint + "] Bridge destroyed.");
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }
    }
}
