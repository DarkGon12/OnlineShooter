using Sfs2X.Entities.Data;
using Sfs2X.Entities;
using Sfs2X.Requests;
using UnityEngine;
using Zenject;
using Sfs2X;

namespace Sendlers
{
    public class TransformSender : MonoBehaviour
    {
        private GlobalManager _globalManager;

        private SmartFox _sfs;

        [Inject]
        public void Construct(GlobalManager globalManager)
        {
            _globalManager = globalManager;
        }

        public void SendTransform(SF2X_CharacterTransform chtransform)
        {
            if (_sfs == null)
                _sfs = _globalManager.GetSfsClient();

            Room room = _sfs.LastJoinedRoom;
            ISFSObject data = new SFSObject();
            chtransform.ToSFSObject(data);
            ExtensionRequest request = new ExtensionRequest("sendTransform", data, room, true); // True flag = UDP
            _sfs.Send(request);
        }
    }
}