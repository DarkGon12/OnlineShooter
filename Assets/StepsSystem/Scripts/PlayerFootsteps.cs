using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    public AudioSource audioSource;
    public Transform footTransform;
    public float raycastDistance = 1.5f; 

    void Start()
    {
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();     
    }

    public void PlayFootstep()
    {
        RaycastHit hit;
        if (Physics.Raycast(footTransform.position, Vector3.down, out hit, raycastDistance))
        {
            SurfaceSound surfaceSound = hit.collider.GetComponent<SurfaceSound>();
            if (surfaceSound != null)
            {
                AudioClip footstepSound = surfaceSound.GetFootstepSound();
                if (footstepSound != null)
                {
                    audioSource.PlayOneShot(footstepSound);
                }
            }
        }
    }
}