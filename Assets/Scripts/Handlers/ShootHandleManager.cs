using Sfs2X.Entities.Data;
using UnityEngine;
using Sfs2X;
using Zenject;

namespace Handlers
{
    public class ShootHandleManager : MonoBehaviour
    {
        private PlayerManager _playerManager;

        [Inject]
        private void Construct(PlayerManager playerManager)
        {
            _playerManager = playerManager;
        }

        public void HandleShotFired(ISFSObject sfsobject, SmartFox sfs)
        {
            int userId = sfsobject.GetInt("id");
            float hitX = sfsobject.GetFloat("hitX");
            float hitY = sfsobject.GetFloat("hitY");
            float hitZ = sfsobject.GetFloat("hitZ");

            if (userId != sfs.MySelf.Id)
            {
                FPSController remotePlayerController = _playerManager.GetOtherPlayer()[userId];

                remotePlayerController.AnimationSyncAsync("shoot");
                AudioSource source = remotePlayerController.GetComponent<AudioSource>();
                source.PlayOneShot(remotePlayerController.m_Shoot[Random.Range(0, remotePlayerController.m_Shoot.Length)]);

                Vector3 hitPosition = new Vector3(hitX, hitY, hitZ);
                Collider[] colliders = Physics.OverlapSphere(hitPosition, 0.15f);

                foreach (Collider collider in colliders)
                {
                    GameObject foundObject = collider.gameObject;

                    Impact impactComponent = foundObject.GetComponent<Impact>();
                    if (impactComponent != null)
                    {
                        GameObject vfx = Instantiate(impactComponent.impactPrefab, new Vector3(hitX, hitY, hitZ), Quaternion.identity);
                        Destroy(vfx, 5f);
                    }
                }
            }
        }
    }
}