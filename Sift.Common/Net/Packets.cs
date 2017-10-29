using System;
using System.Collections.Generic;
using System.IO;

namespace Sift.Common.Net
{
    public enum PacketType : byte
    {
        InitializeClient,
        LineAction,
        InitializeLine,
        LineStateChanged,
        LineMetadata,
        Error,
        Settings,
        SettingsQuery,
        Login,
    }

    public struct InitializeClient : ISdpPacket
    {
        public byte Lines;
        public VoipProviders Provider;

        public PacketType Type => PacketType.InitializeClient;

        public void ReadFrom(BinaryReader reader)
        {
            Lines = reader.ReadByte();
            Provider = (VoipProviders)reader.ReadByte();
        }

        public void WriteTo(BinaryWriter writer)
        {
            writer.Write(Lines);
            writer.Write((byte)Provider);
        }
    }

    public struct InitializeLine : ISdpPacket
    {
        public byte Index;
        public string Id;
        public string Number;
        //public long Created;
        public LineState State;

        public PacketType Type => PacketType.InitializeLine;

        public InitializeLine(Line line)
        {
            if (line == null)
                throw new ArgumentNullException("line");
            Index = line.Index;
            Id = line.Caller?.Id ?? string.Empty;
            Number = line.Caller?.Number ?? string.Empty;
            //Created = line.Caller != null ? line.Caller.Created.ToTimestamp() : 0;
            State = line.State;
        }

        public void ReadFrom(BinaryReader reader)
        {
            Index = reader.ReadByte();
            Id = reader.ReadString();
            Number = reader.ReadString();
            //Created = reader.ReadInt64();
            State = (LineState)reader.ReadByte();
        }

        public void WriteTo(BinaryWriter writer)
        {
            writer.Write(Index);
            writer.Write(Id);
            writer.Write(Number);
            //writer.Write(Created);
            writer.Write((byte)State);
        }
    }

    public struct LineStateChanged : ISdpPacket
    {
        public byte Index;
        public LineState State;

        public PacketType Type => PacketType.LineStateChanged;

        public LineStateChanged(Line line)
        {
            Index = line.Index;
            State = line.State;
        }

        public void ReadFrom(BinaryReader reader)
        {
            Index = reader.ReadByte();
            State = (LineState)reader.ReadByte();
        }

        public void WriteTo(BinaryWriter writer)
        {
            writer.Write(Index);
            writer.Write((byte)State);
        }
    }

    public struct LineMetadata : ISdpPacket
    {
        public byte Index;
        public string Name;
        public string Location;
        public string Comment;

        public PacketType Type => PacketType.LineMetadata;

        public LineMetadata(Line line)
        {
            if (line == null)
                throw new ArgumentNullException("line");
            Index = line.Index;
            Name = line.Caller?.Name ?? string.Empty;
            Location = line.Caller?.Location ?? string.Empty;
            Comment = line.Caller?.Comment ?? string.Empty;
        }

        public void ReadFrom(BinaryReader reader)
        {
            Index = reader.ReadByte();
            Name = reader.ReadString();
            Location = reader.ReadString();
            Comment = reader.ReadString();
        }

        public void WriteTo(BinaryWriter writer)
        {
            writer.Write(Index);
            writer.Write(Name);
            writer.Write(Location);
            writer.Write(Comment);
        }
    }

    public struct LineAction : ISdpPacket
    {
        public byte Index;
        public LineActions Action;

        public PacketType Type => PacketType.LineAction;

        public void ReadFrom(BinaryReader reader)
        {
            Index = reader.ReadByte();
            Action = (LineActions)reader.ReadByte();
        }

        public void WriteTo(BinaryWriter writer)
        {
            writer.Write(Index);
            writer.Write((byte)Action);
        }
    }

    public struct Error : ISdpPacket
    {
        public string Message;
        public string Detail;
        public bool Fatal;

        public PacketType Type => PacketType.Error;

        public void ReadFrom(BinaryReader reader)
        {
            Message = reader.ReadString();
            Detail = reader.ReadString();
            Fatal = reader.ReadBoolean();
        }

        public void WriteTo(BinaryWriter writer)
        {
            writer.Write(Message);
            writer.Write(Detail);
            writer.Write(Fatal);
        }
    }

    public struct SettingsQuery : ISdpPacket
    {
        public string Category;
        public string Key;

        public PacketType Type => PacketType.SettingsQuery;

        public void ReadFrom(BinaryReader reader)
        {
            Category = reader.ReadString();
            Key = reader.ReadString();
        }

        public void WriteTo(BinaryWriter writer)
        {
            writer.Write(Category);
            writer.Write(Key);
        }
    }

    public struct Settings : ISdpPacket
    {
        public string Category;
        public string Key;
        public ICollection<NetworkSetting> Items;

        public PacketType Type => PacketType.Settings;

        public void ReadFrom(BinaryReader reader)
        {
            var items = new List<NetworkSetting>();
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                items.Add(new NetworkSetting(reader));
                if (i == 0)
                {
                    Key = items[i].Key;
                    Category = items[i].Category;
                }
            }
            Items = items;
        }

        public void WriteTo(BinaryWriter writer)
        {
            writer.Write(Items.Count);
            foreach (var item in Items)
            {
                item.Encode(writer);
            }
        }
    }
}
