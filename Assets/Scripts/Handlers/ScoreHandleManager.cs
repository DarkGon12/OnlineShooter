using Sfs2X.Entities.Data;
using UnityEngine;
using Sfs2X;
using Zenject;

namespace Handlers
{
    public class ScoreHandleManager : MonoBehaviour
    {
        private PlayerManager _playerManager;

        [Inject]
        private void Construct(PlayerManager playerManager)
        {
            _playerManager = playerManager;
        }

        public void HandleScoreChange(ISFSObject sfsobject, SmartFox sfs)
        {
            int userId = sfsobject.GetInt("id");
            int score = sfsobject.GetInt("score");
            /*
            if (userId == sfs.MySelf.Id)
                _tabManager.ChangeScore(_playerManager.GetLocalPlayer().name, score);

            if (userId != sfs.MySelf.Id)
            {
                FPSController remotePlayerController = _playerManager.GetOtherPlayer()[userId];
                _tabManager.ChangeScore(remotePlayerController.name, score);
            }
            */
        }
    }
}