using Sfs2X.Entities.Data;
using UnityEngine;
using Zenject;
using Sfs2X;

namespace Handlers
{
    public class TransformHandleManager : MonoBehaviour
    {
        private PlayerManager _playerManager;

        [Inject]
        private void Construct(PlayerManager playerManager)
        {
            _playerManager = playerManager;
        }

        public void HandleTransform(ISFSObject sfsobject, SmartFox sfs)
        {
            int userId = sfsobject.GetInt("id");
            SF2X_CharacterTransform chtransform = SF2X_CharacterTransform.FromSFSObject(sfsobject);
            if (userId != sfs.MySelf.Id)
            {
                if (_playerManager.GetOtherPlayer().ContainsKey(userId))
                {
                    FPSController remotePlayerController = _playerManager.GetOtherPlayer()[userId];
                    remotePlayerController.ReceiveTransform(chtransform);
                }
            }
        }

        public void HandleNoTransform(ISFSObject sfsobject, SmartFox sfs)
        {
            int userId = sfsobject.GetInt("id");
            SF2X_CharacterTransform chtransform = SF2X_CharacterTransform.FromSFSObject(sfsobject);

            if (userId == sfs.MySelf.Id)
                chtransform.ResetTransform(this._playerManager.GetPlayerObject().transform);
        }
    }
}