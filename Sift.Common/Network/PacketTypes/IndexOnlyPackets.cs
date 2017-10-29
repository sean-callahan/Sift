using System;

namespace Sift.Common.Network
{/*
    public class RemoveLine : IndexOnlyPacket
    {
        public RemoveLine(NetIncomingMessage msg) : base(msg)
        {
        }

        public RemoveLine(byte index) : base(index)
        {
        }

        public override PacketType Type => PacketType.RemoveLine;
    }

    public class AirLine : IndexOnlyPacket
    {
        public AirLine(NetIncomingMessage msg) : base(msg)
        {
        }

        public AirLine(byte index) : base(index)
        {
        }

        public override PacketType Type => PacketType.AirLine;
    }

    public class DumpLine : IndexOnlyPacket
    {
        public DumpLine(NetIncomingMessage msg) : base(msg)
        {
        }

        public DumpLine(byte index) : base(index)
        {
        }

        public override PacketType Type => PacketType.DumpLine;
    }

    public class HoldLine : IndexOnlyPacket
    {
        public HoldLine(NetIncomingMessage msg) : base(msg)
        {
        }

        public HoldLine(byte index) : base(index)
        {
        }

        public override PacketType Type => PacketType.HoldLine;
    }

    public class LineRequest : IndexOnlyPacket
    {
        public LineRequest(NetIncomingMessage msg) : base(msg)
        {
        }

        public LineRequest(byte index) : base(index)
        {
        }

        public override PacketType Type => PacketType.LineRequest;
    }

    public class ScreenLine : IndexOnlyPacket
    {
        public ScreenLine(NetIncomingMessage msg) : base(msg)
        {
        }

        public ScreenLine(byte index) : base(index)
        {
        }

        public override PacketType Type => PacketType.ScreenLine;
    }

    public abstract class IndexOnlyPacket : IPacket
    {
        public byte Index;

        public abstract PacketType Type { get; }

        public IndexOnlyPacket(NetIncomingMessage msg) => Decode(msg);

        public IndexOnlyPacket(byte index)
        {
            Index = index;
        }

        public void Decode(NetIncomingMessage msg)
        {
            Index = msg.ReadByte();
        }

        public void Encode(NetOutgoingMessage msg)
        {
            msg.Write(Index);
        }
    }*/
}
