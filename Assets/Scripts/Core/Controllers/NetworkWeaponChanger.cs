using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NetworkWeaponChanger : MonoBehaviour
{
    [SerializeField] private FPSController _fpsController;
    [SerializeField] private List<GameObject> _remoutePlayerWeaponList = new List<GameObject>();
    [SerializeField] private GameObject _localWeaponHolder;


    public GameObject GetWeaponHolder() => _localWeaponHolder;

    public void ChangeWeapon(string weapon, bool player)
    {
        if (player)
        {
            _fpsController.SetAnimator(_localWeaponHolder.transform.GetChild(2).GetComponent<Animator>());

            _localWeaponHolder.transform.GetChild(0).gameObject.SetActive(false);
            _localWeaponHolder.transform.GetChild(2).gameObject.SetActive(true);

            Destroy(_localWeaponHolder.transform.GetChild(1).gameObject);
        }
        if (!player)
        {
            for (int i = 0; i < _remoutePlayerWeaponList.Count; i++)
                _remoutePlayerWeaponList[i].SetActive(false);

            _remoutePlayerWeaponList.FirstOrDefault(w => w.name == weapon).SetActive(true);
        }
    }

    public void ChangeToPistol(bool player)
    {
        if (player)
        {
            _fpsController.SetAnimator(_localWeaponHolder.transform.GetChild(0).GetComponent<Animator>());
            _localWeaponHolder.transform.GetChild(0).gameObject.SetActive(true);
            _localWeaponHolder.transform.GetChild(1).gameObject.SetActive(false);
        }
        if (!player)
        {
            for (int i = 0; i < _remoutePlayerWeaponList.Count; i++)
                _remoutePlayerWeaponList[i].SetActive(false);

            _remoutePlayerWeaponList[0].SetActive(true);
        }
    }

    public void WeaponAmmo(int ammo, int unloadAmmo)
    {
        _localWeaponHolder.transform.GetChild(0).GetComponent<Weapon>().SetNewAmmoCount(ammo);
        _localWeaponHolder.transform.GetChild(0).GetComponent<Weapon>().SetNewAmmoReservCount(unloadAmmo);
        _localWeaponHolder.transform.GetChild(0).GetComponent<Weapon>().UpdateAmmoInScreen();
    }
}