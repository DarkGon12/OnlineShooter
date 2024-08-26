using UnityEngine;
using System;

[Serializable]
public class WeaponInHand
{
    [field: SerializeField] public GameObject _activeWeapon { get; private set; }
    [field: SerializeField] public Vector3[] _weaponAnimationPosition { get; private set; }
    [field: SerializeField] public Vector3[] WeaponRunPosition { get; private set; }
    [field: SerializeField] public float _animationSpeed { get; private set; }
    [field: SerializeField] public string WeaponName { get; private set; }
}