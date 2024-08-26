using System.Collections.Generic;
using static NetworkManager;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using Sfs2X.Entities;
using UnityEngine;
using Zenject;
using Sfs2X;

public class PlayerManager : MonoBehaviour, IPlayerManager
{
    private FPSController _localPlayerController;
    private FPSController _remotePlayerController;

    private Dictionary<int, FPSController> _players = new Dictionary<int, FPSController>();
    private Dictionary<int, FPSController> _otherPlayer = new Dictionary<int, FPSController>();

    private GameObject _playerObj;

    [SerializeField] private GameObject[] playerEnemyPrefab;
    [SerializeField] public GameObject[] playerPrefab;

    [SerializeField] public InterpolationMode NetworkSyncMode = InterpolationMode.Complex;

    private DiContainer _container;

    [Inject]
    private void Construct(DiContainer container, GlobalManager globalManager)
    {
        _container = container;
    }

    public void HandleSpawnPlayer(ISFSObject playerObject, SmartFox sfs)
    {
        ISFSObject playerData = playerObject.GetSFSObject("player");
        int userId = playerData.GetInt("id");
        int score = playerData.GetInt("score");
        int prefab = playerData.GetInt("prefab");
        int colors = playerData.GetInt("color");
        int weapon = playerData.GetInt("weapon");

        SF2X_CharacterTransform chtransform = SF2X_CharacterTransform.FromSFSObject(playerData);
        User user = sfs.UserManager.GetUserById(userId);

        if (userId == sfs.MySelf.Id)
        {
            if (_playerObj == null)
            {
                _playerObj = Instantiate(playerPrefab[prefab]);
                _playerObj.name = user.Name;
                _container.InjectGameObject(_playerObj);

                _playerObj.transform.localEulerAngles = chtransform.AngleRotationFPS;
                _playerObj.transform.position = chtransform.Position;

                _localPlayerController = _playerObj.GetComponent<FPSController>();
                _localPlayerController.isPlayer = true;
            }
            else
            {
                _localPlayerController.transform.position = chtransform.Position;
                _localPlayerController.GetComponent<BloodEffectController>().Damage(100);
                _localPlayerController.GetComponent<CharacterController>().enabled = true;
                _localPlayerController.enabled = true;

                _localPlayerController.WeaponChanger.ChangeWeapon("Glock", true);

                SwitchWeapon switchWeapon = _localPlayerController.WeaponChanger.GetWeaponHolder().GetComponent<SwitchWeapon>();
                switchWeapon.SelectWeapon();

                SendTransform(_localPlayerController.lastState, sfs);
            }
        }
        else
        {
            GameObject playerObj = Instantiate(playerEnemyPrefab[prefab]);
            playerObj.name = user.Name;

            playerObj.transform.localEulerAngles = chtransform.AngleRotationFPS;
            playerObj.transform.position = chtransform.Position;

            _remotePlayerController = playerObj.GetComponent<FPSController>();
            _remotePlayerController.isPlayer = false;

            playerObj.GetComponent<FPSController>().userid = userId;

            if (NetworkSyncMode == InterpolationMode.Complex)
                playerObj.AddComponent<SF2X_SyncManager>();

            _remotePlayerController.WeaponChanger.ChangeWeapon("Glock", false);

            _otherPlayer[userId] = playerObj.GetComponent<FPSController>();
        }
    }

    public void SendTransform(SF2X_CharacterTransform chtransform, SmartFox sfs)
    {
        Room room = sfs.LastJoinedRoom;
        ISFSObject data = new SFSObject();
        chtransform.ToSFSObject(data);
        ExtensionRequest request = new ExtensionRequest("sendTransform", data, room, true);
        sfs.Send(request);
    }

    public void UpdatePlayerTransform(int userId, SF2X_CharacterTransform transform)
    {
        if (_players.ContainsKey(userId))
        {
            var player = _players[userId];
            player.ReceiveTransform(transform);
        }
    }

    public void DestroyEnemy(int id)
    {
        if (_otherPlayer.ContainsKey(id))
        {
            FPSController remotePlayerController = _otherPlayer[id];
            if (remotePlayerController)
                Destroy(remotePlayerController.gameObject, 15f);
        }
    }

    public GameObject GetPlayerObject() => _playerObj;

    public FPSController GetLocalPlayer() => _localPlayerController;

    public Dictionary<int, FPSController> GetOtherPlayer() => _otherPlayer;
}