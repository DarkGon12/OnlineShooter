using Sfs2X.Entities.Data;
using Sfs2X.Entities;
using UnityEngine;
using Zenject;
using Sfs2X;

namespace Handlers
{
    public class KillHandleManager : MonoBehaviour
    {
        private PlayerManager _playerManager;

        [Inject]
        private void Construct(PlayerManager playerManager)
        {
            _playerManager = playerManager;
        }

        public void HandleKill(ISFSObject sfsobject, SmartFox sfs)
        {
            int userId = sfsobject.GetInt("id");
            int killerId = sfsobject.GetInt("killerId");
            int weaponId = sfsobject.GetInt("killerWeaponId");
            User user = sfs.UserManager.GetUserById(userId);
            User killer = sfs.UserManager.GetUserById(killerId);

            string localName = user.Name;
            string killerName = killer.Name;
            string weaponName = "";

            if (weaponId == 0)
                weaponName = "Глока";
            if (weaponId == 1)
                weaponName = "AK-47";
            if (weaponId == 2)
                weaponName = "СВД";

            if (userId != sfs.MySelf.Id)
            {
                if (_playerManager.GetOtherPlayer().ContainsKey(userId))
                {
                    FPSController remotePlayerController = _playerManager.GetOtherPlayer()[userId];

                    remotePlayerController.isDead = true;
                    remotePlayerController.GetComponent<DeadModel>().Dead();
                    remotePlayerController.enabled = false;
                    _playerManager.DestroyEnemy(userId);
                }
            }
            else
            {
                FPSController localPlayer = _playerManager.GetLocalPlayer();

                localPlayer.DeadPanel.SetActive(true);
                localPlayer.isDead = true;
                localPlayer.enabled = false;

                string deadInfo = $"вас убил {killerName} с {weaponName}";

            //    _deadInfo.text = deadInfo;
            }

            string info = $"{killerName} <color=#FF2000>убивает</color> {localName}";
          //  GameObject infoKill = Instantiate(Resources.Load<GameObject>("Message/message"), _killTab.transform);
          //  infoKill.transform.GetComponentInChildren<TextMeshProUGUI>().text = info;
        }
    }
}