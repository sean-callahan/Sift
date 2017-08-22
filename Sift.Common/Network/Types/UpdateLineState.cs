using System;

using Lidgren.Network;

namespace Sift.Common.Network
{
    public class UpdateLineState : IPacket
    {
        public ushort Index;
        public LineState State;
        public string Id;
        public string Number;
        public string Name;
        public string Location;
        public string Comment;

        public PacketType Type => PacketType.UpdateLineState;

        public UpdateLineState(Line line)
        {
            if (line == null)
                throw new ArgumentNullException("line");
            Index = (ushort)line.Index;
            State = line.State;
            Id = line.Caller?.Id;
            Number = line.Caller?.Number;
            Name = line.Caller?.Name;
            Location = line.Caller?.Location;
            Comment = line.Caller?.Comment;
        }

        public UpdateLineState(NetIncomingMessage msg) => Decode(msg);

        public void Decode(NetIncomingMessage msg)
        {
            Index = msg.ReadUInt16();
            State = (LineState)msg.ReadByte();
            Id = msg.ReadString();
            Number = msg.ReadString();
            Name = msg.ReadString();
            Location = msg.ReadString();
            Comment = msg.ReadString();
        }

        public void Encode(NetOutgoingMessage msg)
        {
            msg.Write(Index);
            msg.Write((byte)State);
            msg.Write(Id);
            msg.Write(Number);
            msg.Write(Name);
            msg.Write(Location);
            msg.Write(Comment);
        }
    }
}
