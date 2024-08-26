using Sfs2X.Entities.Data;
using UnityEngine;
using Zenject;
using Sfs2X;

namespace Handlers
{
    public class ChatHandleManager : MonoBehaviour
    {
        private PlayerManager _playerManager;
        private GameState _gameState;

        [Inject]
        private void Construct(PlayerManager playerManager, GameState gameState)
        {
            _playerManager = playerManager;
            _gameState = gameState;
        }

        public void HandleReciveChat(ISFSObject sfsobject, SmartFox sfs)
        {
            int userId = sfsobject.GetInt("id");
            string msg = sfsobject.GetUtfString("msg");

            FPSController localPlayer = _playerManager.GetLocalPlayer();

            if (userId == sfs.MySelf.Id)
                localPlayer.ChatManager.CreateMessage(_gameState.PlayerName, msg);
            else
            {
                FPSController remotePlayerController = _playerManager.GetOtherPlayer()[userId];
                localPlayer.ChatManager.CreateMessage(remotePlayerController.name, msg);
            }
        }
    }
}