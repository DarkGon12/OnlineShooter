using Sfs2X.Entities.Data;
using UnityEngine;
using Zenject;
using Sfs2X;

namespace Handlers
{
    public class WeaponHandleManager : MonoBehaviour
    {
        private PlayerManager _playerManager;

        [Inject]
        private void Construct(PlayerManager playerManager)
        {
            _playerManager = playerManager;
        }

        public void HandleWeaponChange(ISFSObject sfsobject, SmartFox sfs)
        {
            int userId = sfsobject.GetInt("id");
            string weapon = sfsobject.GetUtfString("weapon");

            if (userId != sfs.MySelf.Id)
            {
                FPSController remotePlayerController = _playerManager.GetOtherPlayer()[userId];

                if (weapon != "Glock")
                    remotePlayerController.WeaponChanger.ChangeWeapon(weapon, false);
                else
                    remotePlayerController.WeaponChanger.ChangeToPistol(false);
            }
            else
            {
                FPSController localPlayer = _playerManager.GetLocalPlayer();

                if (weapon != "Glock")
                    localPlayer.WeaponChanger.ChangeWeapon(weapon, true);
                else
                    localPlayer.WeaponChanger.ChangeToPistol(true);
            }
        }
    }
}