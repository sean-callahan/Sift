using System;

namespace Sift.Common.Network
{
    /*public class LineMetadata : IPacket
    {
        public byte Index;
        public LineState State;
        public string Name;
        public string Location;
        public string Comment;

        public PacketType Type => PacketType.LineMetadata;

        public LineMetadata(Line line)
        {
            if (line == null)
                throw new ArgumentNullException("line");
            Index = line.Index;
            State = line.State;
            Name = line.Caller?.Name;
            Location = line.Caller?.Location;
            Comment = line.Caller?.Comment;
        }

        public LineMetadata(NetIncomingMessage msg) => Decode(msg);

        public void Decode(NetIncomingMessage msg)
        {
            Index = msg.ReadByte();
            State = (LineState)msg.ReadByte();
            Name = msg.ReadString();
            Location = msg.ReadString();
            Comment = msg.ReadString();
        }

        public void Encode(NetOutgoingMessage msg)
        {
            msg.Write(Index);
            msg.Write((byte)State);
            msg.Write(Name);
            msg.Write(Location);
            msg.Write(Comment);
        }
    }*/
}
