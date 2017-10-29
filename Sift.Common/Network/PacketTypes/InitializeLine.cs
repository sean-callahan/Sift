using System;

using Sift.Common.Util;

namespace Sift.Common.Network
{
    /*public class InitializeLine : IPacket
    {
        public byte Index;
        public string Id;
        public string Number;
        public long Created;

        public PacketType Type => PacketType.InitializeLine;

        public InitializeLine(Line line)
        {
            if (line == null)
                throw new ArgumentNullException("line");
            Index = line.Index;
            Id = line.Caller?.Id;
            Number = line.Caller?.Number;
            if (line.Caller != null)
                Created = line.Caller.Created.ToTimestamp();
        }

        public InitializeLine(NetIncomingMessage msg) => Decode(msg);

        public void Decode(NetIncomingMessage msg)
        {
            Index = msg.ReadByte();
            Id = msg.ReadString();
            Number = msg.ReadString();
            Created = msg.ReadInt64();
        }

        public void Encode(NetOutgoingMessage msg)
        {
            msg.Write(Index);
            msg.Write(Id);
            msg.Write(Number);
            msg.Write(Created);
        }
    }*/
}
