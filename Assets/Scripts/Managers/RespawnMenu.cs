using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Sendlers;
using Zenject;

public class RespawnMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timeText;
    [SerializeField] private Button _respawnButton;

    [SerializeField] private FPSController _fpsController;
    [SerializeField] private GameObject _weaponHolder;
  //  [SerializeField] private Weapon _weaponScript;

    private int _timeToResp;

    private SpawnRequestSender _spawnSender;

    [Inject]
    private void Construct(SpawnRequestSender spawnSender)
    {
        _spawnSender = spawnSender;
    }

    private void Start()
    {
        _respawnButton.onClick.AddListener(Respawn);
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _respawnButton.enabled = true;
        _timeToResp = 5;
        _timeText.text = "Возрождение через " + _timeToResp;
        _fpsController.MoveMode = false;
        _fpsController.LoockMode = false;
        _weaponHolder.SetActive(false);
      //  _weaponScript.enabled = false;
    }

    private void Respawn()
    {
        _spawnSender.ResurrectPlayer();
        _respawnButton.enabled = false;
        StartCoroutine(WaitToRespawn());

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private IEnumerator WaitToRespawn()
    {
        while (true)
        {
            _timeToResp--;
            _timeText.text = "Возрождение через " + _timeToResp;
            yield return new WaitForSeconds(1);

            if (_timeToResp <= 0)
            {
                _fpsController.MoveMode = true;
                _fpsController.LoockMode = true;
                _weaponHolder.SetActive(true);
                gameObject.SetActive(false);
             //   _weaponScript.enabled = true;
              //  _weaponScript.SetWeaponState(true);
                yield return null;
            }
        }
    }
}