using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using Color = UnityEngine.Color;
using System.Threading.Tasks;
using Sfs2X.Entities.Data;
using UnityEngine.UI;
using Sfs2X.Entities;
using Sfs2X.Requests;
using UnityEngine;
using Services;
using Zenject;
using Sfs2X;
using TMPro;
using Sendlers;

public class NetworkManager : MonoBehaviour
{
    private NetworkService _networkService;
    private PlayerManager _playerManager;
    private SpawnRequestSender _spawnRequestSender;

    #region Inject
    [Inject]
    public void Construct(NetworkService networkService, PlayerManager playerManager, GlobalManager globalManager, SpawnRequestSender spawnRequest)
    {
        _globalManager = globalManager;
        _networkService = networkService;
        _playerManager = playerManager;
        _spawnRequestSender = spawnRequest;
    }
    #endregion

    void Awake() => instance = this;

    void Start()
    {
        _sfs = _globalManager.GetSfsClient();

        if (_sfs == null)
        {
            SceneManager.LoadScene("lobby");
            return;
        }

        _networkService.SubscribeToEvents(_sfs);
        _spawnRequestSender.SendSpawnRequest();

        if (NetworkSyncMode == InterpolationMode.Complex)
            _sfs.EnableLagMonitor(true, 1, 10);
    }

    void OnDestroy()
    {
        _networkService.UnsubscribeFromEvents();
    }

    #region  Global Definitions
    public static NetworkManager Instance
    {
        get
        {
            return instance;
        }
    }

    public object OnRoomVarsUpdate { get; private set; }

    private static NetworkManager instance;
    public int PlayerRoomCount;
    public List<string> PlayerNameInRoom = new List<string>();
    public string LocalPlayerName;

    private GlobalManager _globalManager;
    private SmartFox _sfs;

    private GameObject playerObj;

    [SerializeField] private TextMeshProUGUI _pingText;
    [SerializeField] private GameObject _startWeaponPrefab;
    private Slider _healthBar;
    private TextMeshProUGUI _deadInfo;
    private GameObject _killTab;
    private TABManager _tabManager;
    private FPSController localPlayerController;
    private FPSController remotePlayerController;
    [SerializeField] private GameObject _remotePlayerDeadPrefabs;

    public int clientServerLag;
    public double lastServerTime = 0;
    public double lastLocalTime = 0;
    private int playerHealth;

    private Dictionary<int, FPSController> recipients = new Dictionary<int, FPSController>();
    private Dictionary<int, GameObject> items = new Dictionary<int, GameObject>();
    #endregion

    #region  Player Character Definitions

    [SerializeField] public GameObject[] playerPrefab;
    [SerializeField] private GameObject[] playerEnemyPrefab;
    [SerializeField]
    public Color[] colorarray = {   new Color32(239, 190, 125, 255),
                                    new Color32(255, 109, 106, 255),
                                    new Color32(139, 211, 230, 255),
                                    new Color32(177, 162, 202, 255),
                                    new Color32(220, 220, 220, 255),
                                    new Color32(57, 57, 57, 255) };

    public GameObject GetPlayerObject() => playerObj;

    #endregion

    #region  Network and Input Definitions
    public enum InterpolationMode
    {
        Simple,
        Complex
    }
    [SerializeField] public InterpolationMode NetworkSyncMode = InterpolationMode.Complex;
    #endregion

    #region  Audio Definitions
    [SerializeField] public AudioClip[] gunshot;
    [SerializeField] public AudioClip reload;
    [SerializeField] public AudioClip footstep;
    [SerializeField] public AudioClip landing;
    [SerializeField] public AudioClip wounded;
    private AudioSource audioSource;
    private AudioSource remoteAudioSource;
    private int waitToRespawn = 5;
    #endregion

    public void InitializeWeaponSound(AudioClip[] shot, AudioClip reload)
    {
        this.gunshot = shot;
        this.reload = reload;
    }

    #region Network Send Methods
    /*
        public void SendSpawnRequest()
        {
            int colors1 = Random.Range(0, colorarray.Length);
            int prefab1 = Random.Range(0, playerPrefab.Length);
            string startWeapon = "Glock";
            Room room = _sfs.LastJoinedRoom;
            ISFSObject data = new SFSObject();
            data.PutInt("prefab", prefab1);
            data.PutInt("color", colors1);
            data.PutUtfString("weapon", startWeapon);
            ExtensionRequest request = new ExtensionRequest("spawnMe", data, room);
            _sfs.Send(request);
        }

        public void SendShot(int target, Vector3 hit, string bodyType)
        {
            Room room = _sfs.LastJoinedRoom;
            ISFSObject data = new SFSObject();
            data.PutInt("target", target);
            data.PutFloat("hitX", hit.x);
            data.PutFloat("hitY", hit.y);
            data.PutFloat("hitZ", hit.z);
            data.PutUtfString("bodyType", bodyType);
            ExtensionRequest request = new ExtensionRequest("shot", data, room);
            _sfs.Send(request);
        }
   

    public void SendReload()
    {
        Room room = _sfs.LastJoinedRoom;
        ExtensionRequest request = new ExtensionRequest("reload", new SFSObject(), room);
        _sfs.Send(request);
    } 
    */

    public void SendAnimationState(string message)
    {
        Room room = _sfs.LastJoinedRoom;
        ISFSObject data = new SFSObject();
        data.PutUtfString("msg", message);
        ExtensionRequest request = new ExtensionRequest("sendAnim", data, room);
        _sfs.Send(request);
    }

    /*
    public void SendWeaponChange(string weapon)
    {
        Room room = _sfs.LastJoinedRoom;
        ISFSObject data = new SFSObject();
        data.PutUtfString("weapon", weapon);
        ExtensionRequest request = new ExtensionRequest("changeWeapon", data, room);
        _sfs.Send(request);
        Debug.Log("send: " + weapon);
    }
    */

    public void SendTransform(SF2X_CharacterTransform chtransform)
    {
        Room room = _sfs.LastJoinedRoom;
        ISFSObject data = new SFSObject();
        chtransform.ToSFSObject(data);
        ExtensionRequest request = new ExtensionRequest("sendTransform", data, room, true); // True flag = UDP
        _sfs.Send(request);
    }

    public void TimeSyncRequest()
    {
        Room room = _sfs.LastJoinedRoom;
        ExtensionRequest request = new ExtensionRequest("getTime", new SFSObject(), room);
        _sfs.Send(request);
    }
/*
    public async void ResurrectPlayer()
    {
        await Task.Delay(waitToRespawn * 1000);

        localPlayerController.isDead = false;
        localPlayerController.GetComponent<CharacterController>().enabled = false;
        SendSpawnRequest();
    }
*/   
    public void SendPickUpItem(int index)
    {
        Room room = _sfs.LastJoinedRoom;
        ISFSObject data = new SFSObject();
        data.PutInt("itemId", index);
        ExtensionRequest request = new ExtensionRequest("pickUpItem", data, room);
        _sfs.Send(request);
    }

    public void SendChatMessage(string message)
    {
        Room room = _sfs.LastJoinedRoom;
        ISFSObject data = new SFSObject();
        data.PutUtfString("msg", message);
        ExtensionRequest request = new ExtensionRequest("chatMessage", data, room);
        _sfs.Send(request);
    }
/*
    private void LoadItemToRoom()
    {
        Room room = _sfs.LastJoinedRoom;

        Debug.Log("LoadItem");

        if (room == null)
        {
            Debug.LogError("Room is null");
            return;
        }

        var variables = room.GetVariables();
        Debug.Log("Room Variables Count: " + variables.Count);

        foreach (RoomVariable roomVar in room.GetVariables())
        {
            Debug.Log("LoadItemList");
            if (roomVar.Name.StartsWith("item_"))
            {
                Debug.Log("FindItem_");
                ISFSObject itemData = roomVar.GetSFSObjectValue();
                ItemRoomInfo loadItem = (ItemRoomInfo)itemData.GetClass("item");
                Debug.Log("Initiate class");
                GameObject itemObj = null;
                switch (loadItem.ItemType)
                {
                    case "Ammo":
                        itemObj = Instantiate(Resources.Load<GameObject>("Item/Entity/AmmoBox"), new Vector3(loadItem.ItemPosition[0], loadItem.ItemPosition[1], loadItem.ItemPosition[2]), Quaternion.identity);
                        Debug.Log("LoadAmmo");
                        break;
                    case "HealthPack":
                        itemObj = Instantiate(Resources.Load<GameObject>("Item/Entity/HealthBox"), new Vector3(loadItem.ItemPosition[0], loadItem.ItemPosition[1], loadItem.ItemPosition[2]), Quaternion.identity);
                        Debug.Log("LoadHealth");
                        break;
                    case "AK47":
                        itemObj = Instantiate(Resources.Load<GameObject>("Item/Weapons/AK47"), new Vector3(loadItem.ItemPosition[0], loadItem.ItemPosition[1], loadItem.ItemPosition[2]), Quaternion.identity);
                        Debug.Log("LoadAk47");
                        break;
                    case "SVD":
                        itemObj = Instantiate(Resources.Load<GameObject>("Item/Weapons/SVD"), new Vector3(loadItem.ItemPosition[0], loadItem.ItemPosition[1], loadItem.ItemPosition[2]), Quaternion.identity);
                        Debug.Log("LoadSVD");
                        break;
                }
                Debug.Log("switchEnd");
                if (itemObj != null)
                {
                    Debug.Log("Item != null");
                    itemObj.GetComponent<ItemInfo>().SetItemInfo(loadItem.ItemType, loadItem.ItemIndex);
                    items[loadItem.ItemIndex] = itemObj;
                    Debug.Log("ItemFinishLoad");
                }
            }
        }
    }
*/
    #endregion
}