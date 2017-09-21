using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using Lidgren.Network;

namespace Sift.Common.Network
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

        public byte[] Body { get; private set;  }

        private static readonly IFormatter serializer = new BinaryFormatter();

        public NetworkSetting(NetIncomingMessage msg) => Decode(msg);

        public NetworkSetting(string category, string key, byte[] value)
        {
            Category = category;
            Key = key;
            Body = value;
        }

        public void Decode(NetIncomingMessage msg)
        {
            Category = msg.ReadString();
            Key = msg.ReadString();
            int length = msg.ReadVariableInt32();
            Body = msg.ReadBytes(length);
        }

        public void Encode(NetOutgoingMessage msg)
        {
            msg.Write(Category);
            msg.Write(Key);
            msg.WriteVariableInt32(Body.Length);
            msg.Write(Body);
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
}
