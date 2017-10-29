using System;
using System.IO;

namespace Sift.Common.Net
{
    internal static class SdpCodec
    {
        public static ISdpPacket Decode(byte[] data)
        {
            using (var stream = new MemoryStream(data))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                PacketType opcode = (PacketType)reader.ReadByte();
                if (!SdpManager.PacketTypes.TryGetValue(opcode, out Type type))
                    throw new InvalidDataException("Unknown opcode.");

                ISdpPacket packet = (ISdpPacket)Activator.CreateInstance(type);
                packet.ReadFrom(reader);

                return packet;
            }
        }

        public static byte[] Encode(ISdpPacket packet)
        {
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write((byte)packet.Type);
                packet.WriteTo(writer);

                return stream.ToArray();
            }
        }
    }
}
