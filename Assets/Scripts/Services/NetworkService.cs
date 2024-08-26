using UnityEngine.SceneManagement;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using Sfs2X.Entities;
using UnityEngine;
using Sfs2X.Core;
using Handlers;
using Zenject;
using Sfs2X;

namespace Services
{
    public class NetworkService : MonoBehaviour, INetworkService
    {
        private PlayerManager _playerManager;
        private AmmoHandleManager _ammoManager;
        private GlobalManager _globalManager;
        private ShootHandleManager _shootFireHandleManager;
        private ReloadHandleManager _reloadHandleManager;
        private HealthHandleManager _healthHandleManager;
        private TransformHandleManager _transformHandler;
        private AnimationHandleManager _animationHandler;
        private WeaponHandleManager _weaponHandleManager;
        private ScoreHandleManager _scoreHandlerManager;
        private ItemHandleManager _itemHandleManager;
        private KillHandleManager _killHandleManager;
        private ChatHandleManager _chatHandleManager;
        private TimeHandleManager _timeHandleManager;

        private SmartFox _sfs;

        private int _clientServerLag;

        [Inject]
        public void Construct(GlobalManager globalManager,
            PlayerManager playerManager, AmmoHandleManager ammoManager, TransformHandleManager transformHandler,
            HealthHandleManager healthManager, AnimationHandleManager animationHandler, KillHandleManager killHandler,
            ShootHandleManager shootFireManager, ReloadHandleManager reloadHandle, ScoreHandleManager scoreHandler,
            WeaponHandleManager weaponManager, ItemHandleManager itemManager, ChatHandleManager chatManager,
            TimeHandleManager timeManager)
        {
            _globalManager = globalManager;

            _shootFireHandleManager = shootFireManager;
            _weaponHandleManager = weaponManager;
            _healthHandleManager = healthManager;
            _transformHandler = transformHandler;
            _animationHandler = animationHandler;
            _reloadHandleManager = reloadHandle;
            _scoreHandlerManager = scoreHandler;
            _killHandleManager = killHandler;
            _itemHandleManager = itemManager;
            _chatHandleManager = chatManager;
            _timeHandleManager = timeManager;
            _playerManager = playerManager;
            _ammoManager = ammoManager;
        }

        public void SendRequest(ExtensionRequest request) => _sfs.Send(request);

        public Room GetLastJoinedRoom() => _sfs.LastJoinedRoom;

        public void SubscribeToEvents(SmartFox sfs)
        {
            _sfs = sfs;

            _sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);
            _sfs.AddEventListener(SFSEvent.USER_EXIT_ROOM, OnUserLeaveRoom);
            _sfs.AddEventListener(SFSEvent.USER_ENTER_ROOM, OnUserJoinRoom);
            _sfs.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
            _sfs.AddEventListener(SFSEvent.PING_PONG, OnPingPong);
        }

        public void UnsubscribeFromEvents()
        {
            _sfs.RemoveEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);
            _sfs.RemoveEventListener(SFSEvent.USER_EXIT_ROOM, OnUserLeaveRoom);
            _sfs.RemoveEventListener(SFSEvent.USER_ENTER_ROOM, OnUserJoinRoom);
            _sfs.RemoveEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
            _sfs.AddEventListener(SFSEvent.PING_PONG, OnPingPong);
        }

        private void OnExtensionResponse(BaseEvent evt)
        {
            string cmd = (string)evt.Params["cmd"];
            ISFSObject sfsobject = (SFSObject)evt.Params["params"];
            switch (cmd)
            {
                case "spawnPlayer":
                    {
                        _playerManager.HandleSpawnPlayer(sfsobject, _sfs);
                    }
                    break;
                case "transform":
                    {
                        _transformHandler.HandleTransform(sfsobject, _sfs);
                    }
                    break;
                case "notransform":
                    {
                        _transformHandler.HandleNoTransform(sfsobject, _sfs);
                    }
                    break;
                case "ammo":
                    {
                        _ammoManager.HandleAmmo(sfsobject, _sfs);
                    }
                    break;
                case "health":
                    {
                        _healthHandleManager.HandleHealthChange(sfsobject, _sfs);
                    }
                    break;
                case "anim":
                    {
                        _animationHandler.HandleAnimation(sfsobject, _sfs);
                    }
                    break;
                case "killed":
                    {
                        _killHandleManager.HandleKill(sfsobject, _sfs);
                    }
                    break;
                case "enemyShotFired":
                    {
                        _shootFireHandleManager.HandleShotFired(sfsobject, _sfs);
                    }
                    break;
                case "reloaded":
                    {
                        _reloadHandleManager.HandleReload(sfsobject, _sfs);
                    }
                    break;
                case "score":
                    {
                        _scoreHandlerManager.HandleScoreChange(sfsobject, _sfs);
                    }
                    break;
                case "weaponChanged":
                    {
                        _weaponHandleManager.HandleWeaponChange(sfsobject, _sfs);
                    }
                    break;
                case "spawnItem":
                    {
                        _itemHandleManager.HandleItem(sfsobject, _sfs);
                    }
                    break;
                case "removeItem":
                    {
                        _itemHandleManager.HandleRemoveItem(sfsobject, _sfs);
                    }
                    break;
                case "chatMessage":
                    {
                        _chatHandleManager.HandleReciveChat(sfsobject, _sfs);
                    }
                    break;
                case "time":
                    {
                        _timeHandleManager.HandleServerTime(sfsobject, _sfs);
                    }
                    break;
            }
        }

        private void OnUserJoinRoom(BaseEvent evt)
        {
            User user = (User)evt.Params["user"];
            Room room = (Room)evt.Params["room"];
            string info = (user.Name + " Присоеденился к комнате");
            Console.Send(info, ConsoleMessageType.standart);
            _playerManager.GetLocalPlayer().ChatManager.SendSystem(info);
        }

        private void OnUserLeaveRoom(BaseEvent evt)
        {
            User user = (User)evt.Params["user"];
            Room room = (Room)evt.Params["room"];
            string info = (user.Name + " Покинул комнату");
            Console.Send(info, ConsoleMessageType.standart);
            _playerManager.GetLocalPlayer().ChatManager.SendSystem(info);
        }

        private void OnConnectionLost(BaseEvent evt)
        {
            UnsubscribeFromEvents();
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene("Lobby");
            Console.Send("Соединение с сервером потерянно", ConsoleMessageType.warning);
        }

        public void OnPingPong(BaseEvent evt)
        {
            _clientServerLag = (int)evt.Params["lagValue"] / 2;
            _playerManager.GetLocalPlayer().PingText.text = "ping: " + _clientServerLag.ToString();
        }

        public void OnExitGame()
        {
            UnsubscribeFromEvents();
            _sfs.Send(new LeaveRoomRequest());
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene("Lobby");
        }

        public int GetClientServerLag() => _clientServerLag;

        void OnApplicationQuit() => OnExitGame();
    }
}