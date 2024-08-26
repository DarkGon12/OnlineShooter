using UnityEngine;

public class ChangeView : MonoBehaviour
{
    [SerializeField] private MeshRenderer[] _meshes;
    [SerializeField] private SkinnedMeshRenderer _body;   

    public void ChangeTo()
    {
        for (int i = 0; i < _meshes.Length; i++) 
        {
            _meshes[i].enabled = false;
        }

        _body.enabled = false;
    }

    public void ChangeThre()
    {
        for (int i = 0; i < _meshes.Length; i++)
        {
            _meshes[i].enabled = true;
        }

        _body.enabled = true;
    }
}