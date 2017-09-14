using System;

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

            if (type == GroupType.Holding)
                asterisk.Client.Bridges.StartMoh(Id.ToString(), "default");
        }
        
        public void Add(Caller c)
        {
            try
            {
                asterisk.Client.Bridges.AddChannel(Id.ToString(), c.Id);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public void Remove(Caller c)
        {
            try
            {
                asterisk.Client.Bridges.RemoveChannel(Id.ToString(), c.Id);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
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
