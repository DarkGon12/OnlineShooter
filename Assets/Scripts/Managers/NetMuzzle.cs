using UnityEngine;

public class NetMuzzle : MonoBehaviour
{
    [SerializeField] private GameObject[] m_Muzzle;
    [SerializeField] private GameObject BulletTrailPrefab;
    [SerializeField] private Transform m_MuzzlePosition;
    [SerializeField] private Transform StartShootPosition;

    public void SpawnMuzzle()
    {
        GameObject currentMuzzle = m_Muzzle[Random.Range(0, m_Muzzle.Length)];
        GameObject spawnedMuzzle = Instantiate(currentMuzzle, m_MuzzlePosition.position, m_MuzzlePosition.rotation);
        spawnedMuzzle.transform.localScale = new Vector3(1, 1, 1);
        Destroy(spawnedMuzzle, 2.0f);
    }


    public void SpawnBulletTraill() => Instantiate(BulletTrailPrefab, StartShootPosition.position, StartShootPosition.rotation);
}