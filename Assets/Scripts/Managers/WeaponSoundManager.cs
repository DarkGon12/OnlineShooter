using UnityEngine;

public class WeaponSoundManager : MonoBehaviour 
{
    [SerializeField] private FPSController _fpsController;

    public AudioClip[] SlencerShoot;
    public AudioClip[] Shoot;
    public AudioClip Reload;

    public void OnEnable()
    {
        _fpsController.m_SlencerShoot = SlencerShoot;
        _fpsController.m_Shoot = Shoot;
        _fpsController.m_Reload = Reload;
    }
}