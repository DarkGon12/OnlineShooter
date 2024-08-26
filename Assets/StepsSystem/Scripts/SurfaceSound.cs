using UnityEngine;

public class SurfaceSound : MonoBehaviour
{
    [SerializeField] private AudioClip[] _footstepSound;

    public AudioClip GetFootstepSound() => _footstepSound[Random.Range(0,_footstepSound.Length + 1)];    
}