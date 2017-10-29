using System.Collections.Generic;

namespace Sift.Common.Network
{
    /*public class UpdateSettings : IPacket
    {
        public string Category { get; private set; }

        public string Key { get; private set;  }

        public ICollection<NetworkSetting> Items { get; private set; }

        public UpdateSettings(ICollection<NetworkSetting> items)
        {
            Items = items;
        }

        public UpdateSettings(NetIncomingMessage msg) => Decode(msg);

        public PacketType Type => PacketType.UpdateSettings;

        public void Decode(NetIncomingMessage msg)
        {
            var items = new List<NetworkSetting>();
            int count = msg.ReadVariableInt32();
            for (int i = 0; i < count; i++)
            {
                items.Add(new NetworkSetting(msg));
                if (i == 0)
                {
                    Key = items[i].Key;
                    Category = items[i].Category;
                }
            }
            Items = items;
        }

        public void Encode(NetOutgoingMessage msg)
        {
            msg.WriteVariableInt32(Items.Count);
            foreach (var item in Items)
            {
                item.Encode(msg);
            }
        }
    }*/
}
