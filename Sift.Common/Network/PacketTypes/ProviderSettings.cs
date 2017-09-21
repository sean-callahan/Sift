using System;
using Lidgren.Network;

namespace Sift.Common.Network
{
    public class ProviderSettings : ICodec
    {


        public void Decode(NetIncomingMessage msg)
        {
            throw new NotImplementedException();
        }

        public void Encode(NetOutgoingMessage msg)
        {
            throw new NotImplementedException();
        }
    }
}
