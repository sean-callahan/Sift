using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using Lidgren.Network;

using Sift.Common.Network;

namespace Sift.Common
{
    public class SettingsCollection : ICodec
    {
        private IDictionary<string, object> dict = new Dictionary<string, object>();

        private static BinaryFormatter bf = new BinaryFormatter();

        public object this[string key]
        {
            get
            {
                object value;
                if (dict.TryGetValue(key, out value))
                    return value;
                return null;
            }
            set
            {
                dict[key] = value;
            }
        }

        public SettingsCollection(SettingsCollection other) => copyInto(other.dict);

        public SettingsCollection(ICollection<KeyValuePair<string, object>> items) => copyInto(items);

        public SettingsCollection(params KeyValuePair<string, object>[] items) => copyInto(items);

        private void copyInto(ICollection<KeyValuePair<string, object>> items)
        {
            foreach (KeyValuePair<string, object> item in items)
                dict.Add(item);
        }

        public void Decode(NetIncomingMessage msg)
        {
            int count = msg.ReadVariableInt32();
            for (int i = 0; i < count; i++)
            {
                string key = msg.ReadString();
                int len = msg.ReadVariableInt32();
                using (MemoryStream s = new MemoryStream(msg.ReadBytes(len)))
                {
                    dict[key] = bf.Deserialize(s);
                }
            }
        }

        public void Encode(NetOutgoingMessage msg)
        {
            msg.Write(dict.Count);
            foreach (KeyValuePair<string, object> item in dict)
            {
                using (MemoryStream s = new MemoryStream())
                {
                    bf.Serialize(s, item.Value);

                    msg.Write(item.Key);
                    msg.WriteVariableInt32((int)s.Length);
                    msg.Write(s.ToArray());
                }
            }
        }

        public static IReadOnlyDictionary<VoipProviders, string> Category = new Dictionary<VoipProviders, string>
        {
            { VoipProviders.Asterisk, "asterisk" },
        };
    }
}
