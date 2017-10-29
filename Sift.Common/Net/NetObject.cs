using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using Sift.Common.Util;

namespace Sift.Common.Net
{
    public class NetworkSetting : ICodec
    {
        public string Category;
        public string Key;
        
        public object Value
        {
            get { return DecodeBody(Body); }
            set { Body = EncodeBody(value); }
        }

        public byte[] Body { get; private set; }

        private static readonly IFormatter serializer = new BinaryFormatter();

        public NetworkSetting(string category, string key, byte[] value)
        {
            Category = category;
            Key = key;
            Body = value;
        }

        public NetworkSetting(BinaryReader reader) => Decode(reader);

        public void Decode(BinaryReader reader)
        {
            Category = reader.ReadString();
            Key = reader.ReadString();
            int length = reader.ReadInt32();
            Body = reader.ReadBytes(length);
        }

        public void Encode(BinaryWriter writer)
        {
            writer.Write(Category);
            writer.Write(Key);
            writer.Write(Body.Length);
            writer.Write(Body);
        }

        private byte[] EncodeBody(object value)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Serialize(stream, value);
                return stream.ToArray();
            }
        }

        private object DecodeBody(byte[] value)
        {
            using (MemoryStream stream = new MemoryStream(value))
            {
                return serializer.Deserialize(stream);
            }
        }
    }

    public class NetworkUser : ICodec
    {
        public int Id { get; set; }
        public string Username { get; set; }

        private string encoded;
        public string Password
        {
            get => Encoding.Base64Decode(encoded);
            set => encoded = Encoding.Base64Encode(value);
        }

        public void Decode(BinaryReader msg)
        {
            Id = msg.ReadInt32();
            Username = msg.ReadString();
            encoded = msg.ReadString();
        }

        public void Encode(BinaryWriter writer)
        {
            writer.Write(Id);
            writer.Write(Username);
            writer.Write(encoded);
        }
    }
}
