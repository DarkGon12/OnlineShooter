using sfs2x.extensions.games.shooter.variable;
using System.Collections.Generic;
using Sfs2X.Entities.Variables;
using Sfs2X.Entities.Data;
using Sfs2X.Entities;
using Sfs2X.Requests;
using UnityEngine;
using Zenject;
using Sfs2X;

namespace Handlers
{
    public class ItemHandleManager : MonoBehaviour
    {
        private PlayerManager _playerManager;

        private Dictionary<int, GameObject> _items = new Dictionary<int, GameObject>();

        private SmartFox _sfs;

        [Inject]
        private void Construct(PlayerManager playerManager)
        {
            _playerManager = playerManager;
        }

        public void HandleItem(ISFSObject sfsobject, SmartFox sfs)
        {
            _sfs = sfs; 

            ISFSObject item = sfsobject.GetSFSObject("item");
            int id = item.GetInt("id");
            string itemType = item.GetUtfString("type");
            SF2X_CharacterTransform chtransform = SF2X_CharacterTransform.FromSFSObject(item);
            GameObject itemObj = null;

            switch (itemType)
            {
                case "Ammo":
                    itemObj = Instantiate(Resources.Load<GameObject>("Item/Entity/AmmoBox"));
                    break;
                case "HealthPack":
                    itemObj = Instantiate(Resources.Load<GameObject>("Item/Entity/HealthBox"));
                    break;
                case "AK47":
                    itemObj = Instantiate(Resources.Load<GameObject>("Item/Weapons/AK47"));
                    break;
                case "SVD":
                    itemObj = Instantiate(Resources.Load<GameObject>("Item/Weapons/SVD"));
                    break;
            }

            itemObj.GetComponent<ItemInfo>().SetItemInfo(itemType, id);
            itemObj.transform.position = chtransform.Position;
            _items[id] = itemObj;

            List<float> value = new List<float>() { chtransform.Position.x, chtransform.Position.y, chtransform.Position.z };
            ItemRoomInfo info = new ItemRoomInfo(value, itemType, id);
            SaveItemInRoom(info);
        }

        private void SaveItemInRoom(ItemRoomInfo info)
        {
            Room room = _sfs.LastJoinedRoom;

            SFSObject itemData = new SFSObject();
            itemData.PutClass("item", info);

            SFSRoomVariable roomVariable = new SFSRoomVariable("item_" + info.ItemIndex, itemData);
            roomVariable.IsPersistent = true;

            List<RoomVariable> roomVars = new List<RoomVariable> { roomVariable };

            _sfs.Send(new SetRoomVariablesRequest(roomVars, room));
        }

        public void HandleRemoveItem(ISFSObject sfsobject, SmartFox sfs)
        {
            int playerId = sfsobject.GetInt("playerId");
            ISFSObject item = sfsobject.GetSFSObject("item");
            int id = item.GetInt("id");
            string type = item.GetUtfString("type");
            if (_items.ContainsKey(id))
            {
                Destroy(_items[id]);
                _items.Remove(id);
            }
        }
    }
}