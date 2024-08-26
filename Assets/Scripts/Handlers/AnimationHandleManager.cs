using Sfs2X.Entities.Data;
using UnityEngine;
using Sfs2X;
using Zenject;

namespace Handlers
{
    public class AnimationHandleManager : MonoBehaviour
    {
        private PlayerManager _playerManager;

        [Inject]
        private void Construct(PlayerManager playerManager)
        {
            _playerManager = playerManager;
        }

        public void HandleAnimation(ISFSObject sfsobject, SmartFox sfs)
        {
            int userId = sfsobject.GetInt("id");
            string msg = sfsobject.GetUtfString("msg");
            if (userId != sfs.MySelf.Id)
            {
                if (_playerManager.GetOtherPlayer().ContainsKey(userId))
                {
                    FPSController remotePlayerController = _playerManager.GetOtherPlayer()[userId];
                    remotePlayerController.AnimationSyncAsync(msg);
                }
            }
        }
    }
}