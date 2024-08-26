using Sfs2X.Entities.Data;
using UnityEngine;
using Zenject;
using Sfs2X;

public class AmmoHandleManager : MonoBehaviour
{
    private PlayerManager _playerManager;

    [Inject]
    private void Construct(PlayerManager playerManager)
    {
        _playerManager = playerManager;
    }

    public void HandleAmmo(ISFSObject sfsobject, SmartFox sfs)
    {
        int userId = sfsobject.GetInt("id");
        int ammo = sfsobject.GetInt("ammo");
        int unloadAmmo = sfsobject.GetInt("unloadedAmmo");

        if (userId == sfs.MySelf.Id)
            _playerManager.GetLocalPlayer().WeaponChanger.WeaponAmmo(ammo, unloadAmmo);
    }
}
