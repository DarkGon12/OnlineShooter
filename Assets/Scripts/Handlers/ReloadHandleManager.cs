using Sfs2X.Entities.Data;
using UnityEngine;
using Sfs2X;
using Zenject;

namespace Handlers
{
    public class ReloadHandleManager : MonoBehaviour
    {
        [SerializeField] private AudioClip _reloadAudioClip;

        private PlayerManager _playerManager;

        [Inject]
        private void Construct(PlayerManager playerManager)
        {
            _playerManager = playerManager;
        }

        public void HandleReload(ISFSObject sfsobject, SmartFox sfs)
        {
            int userId = sfsobject.GetInt("id");
            if (userId != sfs.MySelf.Id)
            {
                FPSController remotePlayerController = _playerManager.GetOtherPlayer()[userId];
                remotePlayerController.AnimationSyncAsync("reload");
                remotePlayerController.GetComponent<AudioSource>().PlayOneShot(_reloadAudioClip);
            }
        }
    }
}