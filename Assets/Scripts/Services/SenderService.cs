using Sfs2X.Entities.Data;
using Sfs2X.Entities;
using Sfs2X.Requests;
using UnityEngine;
using Sfs2X;

namespace Services
{
    /*
    public class SenderService : MonoBehaviour
    {
        public void SendSpawnRequest(SmartFox sfs)
        {
            int colors1 = Random.Range(0, colorarray.Length);
            int prefab1 = Random.Range(0, playerPrefab.Length);
            string startWeapon = "Glock";
            Room room = sfs.LastJoinedRoom;
            ISFSObject data = new SFSObject();
            data.PutInt("prefab", prefab1);
            data.PutInt("color", colors1);
            data.PutUtfString("weapon", startWeapon);
            ExtensionRequest request = new ExtensionRequest("spawnMe", data, room);

            sfs.Send(request);
        }
    }
    */
}