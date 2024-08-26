using Sfs2X.Entities.Data;
using UnityEngine;
using Zenject;
using Sfs2X;

namespace Handlers
{
    public class HealthHandleManager : MonoBehaviour
    {
        private FPSController _localPlayer;
        private PlayerManager _playerManager;

        [Inject]
        private void Construct(PlayerManager playerManager)
        {
            _localPlayer = playerManager.GetLocalPlayer();
            _playerManager = playerManager;
        }

        public void HandleHealthChange(ISFSObject sfsobject, SmartFox sfs)
        {
            int userId = sfsobject.GetInt("id");
            int health = sfsobject.GetInt("health");

            if (userId == sfs.MySelf.Id)
            {
                if (health < _playerManager.GetLocalPlayer().PlayerHealth)
                {
                    if (_localPlayer.isDead == false)
                        _localPlayer.AnimationSyncAsync("wounded");

                    _localPlayer.PlayerHealth = health;
                }
                _localPlayer.HealthBar.value = health;
                _localPlayer.GetComponent<BloodEffectController>().Damage(health);
            }
            else
            {
                if (_playerManager.GetOtherPlayer().ContainsKey(userId))
                {
                    FPSController remotePlayerController = _playerManager.GetOtherPlayer()[userId];
                    if (_localPlayer.isDead == false)
                        remotePlayerController.AnimationSyncAsync("wounded");
                }
            }
        }
    }
}