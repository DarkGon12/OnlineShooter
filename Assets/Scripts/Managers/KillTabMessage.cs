using UnityEngine;

public class KillTabMessage : MonoBehaviour
{
    private void Start() =>
        Destroy(gameObject, 8f);
}