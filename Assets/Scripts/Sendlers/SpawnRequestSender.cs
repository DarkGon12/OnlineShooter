using System.Threading.Tasks;
using Sfs2X.Entities.Data;
using Sfs2X.Entities;
using Sfs2X.Requests;
using UnityEngine;
using Zenject;
using Sfs2X;

namespace Sendlers
{
    public class SpawnRequestSender : MonoBehaviour
    {
        [SerializeField] private int _waitToRespawn = 5;

        private PlayerManager _playerManager;
        private GlobalManager _globalManager;

        private SmartFox _sfs;

        [SerializeField] private Color[] colorarray = {   new Color32(239, 190, 125, 255),
                                    new Color32(255, 109, 106, 255),
                                    new Color32(139, 211, 230, 255),
                                    new Color32(177, 162, 202, 255),
                                    new Color32(220, 220, 220, 255),
                                    new Color32(57, 57, 57, 255) };

        [Inject]
        private void Construct(PlayerManager playerManager, GlobalManager globalManager)
        {
            _playerManager = playerManager;
            _globalManager = globalManager;         
        }

        public void SendSpawnRequest()
        {
            if (_sfs == null)
                _sfs = _globalManager.GetSfsClient();
            else
                Debug.LogError("Smart fox server not initialize, check global manager");

            int colors1 = Random.Range(0, colorarray.Length);
            int prefab1 = Random.Range(0, _playerManager.playerPrefab.Length);
            string startWeapon = "Glock";
            Room room = _sfs.LastJoinedRoom;
            ISFSObject data = new SFSObject();
            data.PutInt("prefab", prefab1);
            data.PutInt("color", colors1);
            data.PutUtfString("weapon", startWeapon);
            ExtensionRequest request = new ExtensionRequest("spawnMe", data, room);

            _sfs.Send(request);
        }

        public async void ResurrectPlayer()
        {
            await Task.Delay(_waitToRespawn * 1000);

            _playerManager.GetLocalPlayer().isDead = false;
            _playerManager.GetLocalPlayer().GetComponent<CharacterController>().enabled = false;
            SendSpawnRequest();
        }
    }
}