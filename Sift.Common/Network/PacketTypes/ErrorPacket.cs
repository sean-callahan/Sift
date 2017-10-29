using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sift.Common.Network
{
    /*public class ErrorPacket : IPacket
    {
        public string Message;
        public string StackTrace;

        public PacketType Type => PacketType.ErrorPacket;

        public ErrorPacket()
        {
        }

        public ErrorPacket(Exception e)
        {
            Message = e.Message;
            StackTrace = e.StackTrace;
        }

        public ErrorPacket(NetIncomingMessage msg) => Decode(msg);

        public void Decode(NetIncomingMessage msg)
        {
            Message = msg.ReadString();
            StackTrace = msg.ReadString();
        }

        public void Encode(NetOutgoingMessage msg)
        {
            msg.Write(Message);
            msg.Write(StackTrace);
        }
    }*/
}
