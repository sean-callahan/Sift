using System;

using AsterNET.ARI.Models;
using Sift.Common;

namespace Sift.Server
{
    public class AsteriskGroup : IGroup
    {
        public Guid Id { get; }

        public IVoipProvider Provider { get; }

        public GroupType Type { get; }

        private Asterisk asterisk;
        private Bridge bridge;

        public AsteriskGroup(IVoipProvider provider, GroupType type)
        {
            Id = Guid.NewGuid();
            Provider = provider;
            Type = type;

            asterisk = ((Asterisk)provider);

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
                    bridgeType = "";
                    break;
            }
            
            bridge = asterisk.Client.Bridges.Create(bridgeType, Id.ToString(), Asterisk.AppName);

            if (type == GroupType.Holding)
                asterisk.Client.Bridges.StartMoh(bridge.Id, "default");
        }
        
        public void Add(Caller c)
        {
            asterisk.Client.Bridges.AddChannel(bridge.Id, c.Id);
        }

        public void Remove(Caller c)
        {
            asterisk.Client.Bridges.RemoveChannel(bridge.Id, c.Id);
        }
    }
}
