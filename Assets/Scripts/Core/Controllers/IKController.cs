//using DitzelGames.FastIK;
using System.Collections;
using UnityEngine;
/*
public class IKController : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _weaponHolder;
    [SerializeField] private Transform _handPostion;
    [SerializeField] private Transform _holderPosition;
    [Space]
    [SerializeField] private FastIKFabric _leftHand;
    [SerializeField] private FastIKFabric _rightHand;
    [Space]
    [field: SerializeField] private WeaponInHand _weapon;

    private Vector3 _defaultHolderPos;

    private Coroutine _idleCoroutine;
    private Coroutine _runCoroutine;

    private bool _ikActive;

    private void Start()
    {
        _defaultHolderPos = _weaponHolder.transform.localPosition;
    }

    public void FixedUpdate()
    {
        if (_weapon._activeWeapon != null)
            _ikActive = true;
        else
            _ikActive = false;

        if (!_ikActive) return;

        AnimatorStateInfo stateBodyInfo = _animator.GetCurrentAnimatorStateInfo(0);
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(1);

        if (stateInfo.IsName("idle") || !stateBodyInfo.IsName("idle") && _idleCoroutine == null)
            _idleCoroutine = StartCoroutine(AnimationPlayIdle());

       // if (stateInfo.IsName("runForward") || stateInfo.IsName("runBackward") || stateInfo.IsName("runLeft") || stateInfo.IsName("runRight"))
        //    _runCoroutine = StartCoroutine(AnimationPlayRun());

        if (!stateInfo.IsName("idle") || !stateBodyInfo.IsName("idle"))
            WeaponHolderToHand();
    }

    private void WeaponHolderToHand()
    {
        FabrikState(false);
        _weaponHolder.SetParent(_handPostion);
        _weaponHolder.transform.localPosition = Vector3.zero;
        _weapon._activeWeapon.transform.localRotation = Quaternion.Euler(0, 0, -90);
    }

    private void TransformToIKPostion()
    {
        FabrikState(true);
        _weaponHolder.SetParent(_holderPosition);
        _weaponHolder.transform.localPosition = _defaultHolderPos;
        _weapon._activeWeapon.transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    private void FabrikState(bool state)
    {
        _leftHand.enabled = state;
        _rightHand.enabled = state;
    }

    private IEnumerator AnimationPlayIdle()
    {
        TransformToIKPostion();
        while (true)
        {
            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(1);
            if (!stateInfo.IsName("idle"))
                break;

            for (int i = 0; i < _weapon._weaponAnimationPosition.Length; i++)
            {
                Vector3 startPosition = _weaponHolder.localPosition;
                Vector3 targetPosition = _weapon._weaponAnimationPosition[i];
                float elapsedTime = 0f;
                float duration = _weapon._animationSpeed;

                while (elapsedTime < duration)
                {
                    _weaponHolder.localPosition = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
                    elapsedTime += Time.deltaTime;

                    stateInfo = _animator.GetCurrentAnimatorStateInfo(1);
                    if (!stateInfo.IsName("idle"))
                        break;
                    yield return null;
                }

                _weaponHolder.localPosition = targetPosition;
            }
        }

        _idleCoroutine = null;
    }
   
    /*
    private IEnumerator AnimationPlayRun()
    {
        TransformToIKPostion();
        while (true)
        {
            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(1);
            if (!stateInfo.IsName("runForward") || !stateInfo.IsName("runBackward") || !stateInfo.IsName("runLeft") || !stateInfo.IsName("runRight"))
                break;

            for (int i = 0; i < _weapon.WeaponRunPosition.Length; i++)
            {
                Vector3 startPosition = _weaponHolder.localPosition;
                Vector3 targetPosition = _weapon.WeaponRunPosition[i];
                float elapsedTime = 0f;
                float duration = _weapon._animationSpeed;

                while (elapsedTime < duration)
                {
                    _weaponHolder.localPosition = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
                    elapsedTime += Time.deltaTime;

                    stateInfo = _animator.GetCurrentAnimatorStateInfo(1);
                    if (!stateInfo.IsName("runForward") || !stateInfo.IsName("runBackward") || !stateInfo.IsName("runLeft") || !stateInfo.IsName("runRight"))
                        break;
                    yield return null;
                }

                _weaponHolder.localPosition = targetPosition;
            }
        }
        _runCoroutine = null;
    }
    
}
*/