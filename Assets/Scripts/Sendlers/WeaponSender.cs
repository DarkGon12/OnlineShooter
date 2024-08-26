using Sfs2X.Entities.Data;
using Sfs2X.Entities;
using Sfs2X.Requests;
using UnityEngine;
using Zenject;
using Sfs2X;

namespace Sendlers
{
    public class WeaponSender : MonoBehaviour
    {
        private GlobalManager _globalManager;
        private SmartFox _sfs;

        [Inject]
        private void Construct(GlobalManager globalManager)
        {
            _sfs = globalManager.GetSfsClient();
            _globalManager = globalManager;
        }

        public void SendShot(int target, Vector3 hit, string bodyType)
        {
            if (_sfs == null)
                _sfs = _globalManager.GetSfsClient();

            Room room = _sfs.LastJoinedRoom;
            ISFSObject data = new SFSObject();
            data.PutInt("target", target);
            data.PutFloat("hitX", hit.x);
            data.PutFloat("hitY", hit.y);
            data.PutFloat("hitZ", hit.z);
            data.PutUtfString("bodyType", bodyType);
            ExtensionRequest request = new ExtensionRequest("shot", data, room);
            _sfs.Send(request);
        }

        public void SendReload()
        {
            if (_sfs == null)
                _sfs = _globalManager.GetSfsClient();

            Room room = _sfs.LastJoinedRoom;
            ExtensionRequest request = new ExtensionRequest("reload", new SFSObject(), room);
            _sfs.Send(request);
        }

        public void SendWeaponChange(string weapon)
        {
            if (_sfs == null)
                _sfs = _globalManager.GetSfsClient();

            Room room = _sfs.LastJoinedRoom;
            ISFSObject data = new SFSObject();
            data.PutUtfString("weapon", weapon);
            ExtensionRequest request = new ExtensionRequest("changeWeapon", data, room);
            _sfs.Send(request);
            Debug.Log("send: " + weapon);
        }
    }
}