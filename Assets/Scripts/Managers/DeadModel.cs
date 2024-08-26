using System.Collections.Generic;
using UnityEngine;

public class DeadModel : MonoBehaviour
{
    [SerializeField] private List<Collider> _colliders = new List<Collider>();
    [SerializeField] private List<Rigidbody> _rigids = new List<Rigidbody>();
    [SerializeField] private Animator _animator;

    public void Dead()
    {
        for (int i = 0; i < _colliders.Count; i++)
        {
            _colliders[i].enabled = true;
            _colliders[i].gameObject.layer = 2;
        }

        for(int i = 0; i < _rigids.Count; i++)
            _rigids[i].isKinematic = false;

        _animator.enabled = false;
    }
}