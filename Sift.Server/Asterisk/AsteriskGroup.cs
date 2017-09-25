using System;
using AsterNET.ARI;
using AsterNET.ARI.Models;

using Sift.Common;

namespace Sift.Server.Asterisk
{
    public class AsteriskGroup : IGroup
    {
        public Guid Id { get; }

        public IVoipProvider Provider { get; }

        public GroupType Type { get; }

        private AsteriskProvider asterisk;

        public AsteriskGroup(IVoipProvider provider, GroupType type)
        {
            Id = Guid.NewGuid();
            Provider = provider;
            Type = type;

            asterisk = ((AsteriskProvider)provider);

            string bridgeType;
            switch (type)
            {
                case GroupType.Holding:
                    bridgeType = "holding";
                    break;
                case GroupType.Mixing:
                    bridgeType = "mixing";
                    break;
                default:
                    throw new Exception("Unsupported group type");
            }
            
            asterisk.Client.Bridges.Create(bridgeType, Id.ToString(), AsteriskProvider.AppName);
        }
        
        public void Add(Caller c)
        {
            try
            {
                Bridge b = asterisk.Client.Bridges.Get(Id.ToString());
                if (b != null && b.Channels.Count < 1)
                    asterisk.Client.Bridges.StartMoh(Id.ToString(), "default");
            }
            catch (Exception ex)
            {
                Logger.Log(ex, Logger.Level.Warning);
            }

            try
            {
                asterisk.Client.Bridges.AddChannel(Id.ToString(), c.Id);
            }
            catch (Exception ex)
            {
                Logger.Log(ex, Logger.Level.Warning);
            }
        }

        public void Remove(Caller c)
        {
            try
            {
                Bridge b = asterisk.Client.Bridges.Get(Id.ToString());
                if (b != null && b.Channels.Count == 1)
                    asterisk.Client.Bridges.StopMoh(Id.ToString());
            }
            catch (Exception ex)
            {
                Logger.Log(ex, Logger.Level.Warning);
            }

            try
            {
                asterisk.Client.Bridges.RemoveChannel(Id.ToString(), c.Id);
            }
            catch (AriException ex)
            {
                if (ex.StatusCode != 404)
                    Logger.Log(ex, Logger.Level.Warning);
            }
            catch (Exception ex)
            {
                Logger.Log(ex, Logger.Level.Warning);
            }

            
        }

        public bool Contains(Caller c)
        {
            Bridge b = asterisk.Client.Bridges.Get(Id.ToString());
            if (b == null)
                return false;
            return b.Channels.Contains(c.Id);
        }
    }
}
