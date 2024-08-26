using Sfs2X.Protocol.Serialization;
using System.Collections.Generic;

namespace sfs2x.extensions.games.shooter.variable
{
    public class ItemRoomInfo : SerializableSFSType
    {
        public List<float> ItemPosition = new List<float>();
        public string ItemType;
        public int ItemIndex;

        public ItemRoomInfo(List<float> position, string type, int index)
        {
            ItemPosition = position;
            ItemType = type;
            ItemIndex = index;
        }
    } 
}