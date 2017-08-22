using System;

using AsterNET.ARI.Models;
using Sift.Common;

namespace Sift.Server
{
    public class AsteriskGroup : Group
    {
        private Asterisk asterisk;
        private Bridge bridge;

        public AsteriskGroup(IVoipProvider provider, GroupType type) : base(provider, type)
        {
            Id = Guid.NewGuid();
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
        }

        public override void Add(Caller c)
        {
            asterisk.Client.Bridges.AddChannel(bridge.Id, c.Id);
        }

        public override void Remove(Caller c)
        {
            asterisk.Client.Bridges.RemoveChannel(bridge.Id, c.Id);
        }
    }
}
