using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;

namespace Sift.Common.Network
{
    public interface ICodec
    {
        void Decode(NetIncomingMessage msg);
        void Encode(NetOutgoingMessage msg);
    }
}
