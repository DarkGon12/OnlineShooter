using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;
using Sendlers;
using Zenject;

[RequireComponent(typeof(AudioSource))]
public class FPSController : MonoBehaviour 
{
    #region GlobalDefinition
    public GameObject DeadPanel;
    public Slider HealthBar;
    public GameObject KillTab;
    public TextMeshProUGUI PingText;
    public TABManager TabManager;
    public NetworkWeaponChanger WeaponChanger;
    public ChatManager ChatManager;
    //  public Transform spineTransform;

    public int PlayerHealth;
    public int userid;

    public bool LoockMode = true;
    public bool MoveMode = true;
    public bool isPlayer;
    public bool isMoving;
    public bool isAiming;
    public bool isDead;
    public bool isCrounch;

    public AudioClip[] m_SlencerShoot;
    public AudioClip[] m_Shoot;
    public AudioClip m_Reload;
    #endregion

    #region EditorDeifinition
    [SerializeField] private Camera _mainCam;
    [Space]
    [SerializeField] private float speed = 10.0f;
    [SerializeField] private float gravity = -20.0f;
    [SerializeField] private float jumpHeight = 2.0f;
    [SerializeField] private float runMultiplier = 2.0f;

    [SerializeField] private Transform cameraTransform = null;

    [Space, SerializeField, Range(0f, 90f)] private float jumpSlopeLimit = 0.0f;
    [Space, SerializeField] private float mouseSensitivity = 2.0f;
    [SerializeField] private Animator _animator;
    //[SerializeField] private IKController _ikController;
    #endregion

    #region definition
    private bool simpleLerp;

    private Vector2 relativeDirection;
    private Vector2 lastDirection;

    public static readonly float sendingPeriod = 0.03f;
    private float timeLastSending = 0.0f;

    public SF2X_CharacterTransform lastState;
    private SF2X_CharacterTransform newState;
    private SF2X_SyncManager syncManager;

    private CharacterController _characterController = null;
    private float _jumpMultiplier = 0.0f;
    private float _yVelocity = 0.0f;
    private float _originalSlopeLimit = 0.0f;
    private float _xRotation = 0.0f;

    private Vector3 colExtents;
    public float stepInterval = 0.5f;
    private float stepTimer = 0f;
    [SerializeField] private Collider _rightFoot;
    private PlayerFootsteps _footstepSystem;
    private TransformSender _transformSender;
    #endregion

    #region Animation
    public async Task AnimationSyncAsync(string value)
    {
        if (isPlayer)
            return;

        if (_animator)
        {
            if (value == "aiming")
            {
                this._animator.SetBool("isAiming", true);
            }
            if (value == "notaiming")
            {
                this._animator.SetBool("isAiming", false);
            }
            if (value == "reload")
            {
                this._animator.SetBool("reload", true);
                await Task.Delay(100);
                this._animator.SetBool("reload", false);
            }
            if (value == "shoot")
            {
                this._animator.SetBool("rifle_shoot", true);
                await Task.Delay(50);
                this._animator.SetBool("rifle_shoot", false);
            }
            if(value == "crounch")
            {
                if (_animator.GetBool("crounch"))
                    return;

                this._animator.SetBool("crounch", true);
            }
            switch (value)
            {
                case "idle":
                    {
                        AnimationReset();
                        this._animator.SetBool("idle", true);
                    }
                    break;
                case "notcrounch":
                    {
                        this._animator.SetBool("crounch", false);
                    }
                    break;
                case "runForward":
                    {
                        AnimationReset();
                        this._animator.SetBool("runForward", true);
                    }
                    break;
                case "runBackward":
                    {
                        AnimationReset();
                        this._animator.SetBool("runBackward", true);
                    }
                    break;
                case "runLeft":
                    {
                        AnimationReset();
                        this._animator.SetBool("runLeft", true);
                    }
                    break;
                case "runRight":
                    {
                        AnimationReset();
                        this._animator.SetBool("runRight", true);
                    }
                    break;
                case "ifFly":
                    {
                        AnimationReset();
                        this._animator.SetBool("isFly", true);
                    }
                    break;
            }
        }
    }

    private void Animate()
    {
        if (lastDirection != relativeDirection)
        {
            switch (relativeDirection.ToString())
            {
                case "(0.00, 1.00)":
                        NetworkManager.Instance.SendAnimationState("runForward");
                    break;
                case "(0.00, -1.00)":
                        NetworkManager.Instance.SendAnimationState("runBackward");
                    break;
                case "(-1.00, 0.00)":
                        NetworkManager.Instance.SendAnimationState("runLeft");
                    break;
                case "(1.00, 0.00)":
                        NetworkManager.Instance.SendAnimationState("runRight");
                    break;
                case "(0.00, 0.00)":
                        NetworkManager.Instance.SendAnimationState("idle");
                    break;
            }

            lastDirection = relativeDirection;
        }
    }

    public void AnimationReset()
    {
        if (_animator)
        {
            this._animator.SetBool("runForward", false);
            this._animator.SetBool("runBackward", false);
            this._animator.SetBool("runLeft", false);
            this._animator.SetBool("runRight", false);
            this._animator.SetBool("idle", false);
        }
    }

    public Animator GetAnimator() => _animator;
    public void SetAnimator(Animator anim) => _animator = anim;
    #endregion

    #region Main

    [Inject]
    public void Construct(TransformSender transformSender)
    {
        _transformSender = transformSender;
    }

    private void Start() 
    {
        if (isPlayer)
        {
            FindObjectOfType<ConsoleManager>().SetFPSController(this);

            _characterController = GetComponent<CharacterController>();
            _originalSlopeLimit = _characterController.slopeLimit;
            _footstepSystem = GetComponent<PlayerFootsteps>();
        }

        colExtents = _rightFoot.bounds.extents;
        syncManager = GetComponent<SF2X_SyncManager>();
        _jumpMultiplier = Mathf.Sqrt(jumpHeight * -2f * gravity);

        if (!isPlayer)
            return;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        CheckGroundAnim();
        Animate();

        if (!isPlayer)
            return;

        if (MoveMode)
        {
            Move();
            CheckGround();
            ToCrounch();
            SendTransform();
        }
    }

    private void ToCrounch()
    {
        if (!isPlayer)
            return;

        if(Input.GetKey(KeyCode.LeftControl))
        {
            isCrounch = true;
            NetworkManager.Instance.SendAnimationState("crounch");
            _mainCam.transform.localPosition = new Vector3(_mainCam.transform.localPosition.x, -0.75f, _mainCam.transform.localPosition.z);
            _characterController.center = new Vector3(0, 0.55f, 0);
            _characterController.height = 1.1f;
            speed = 1.5f;
        }
        else
        {
            isCrounch = false;
            NetworkManager.Instance.SendAnimationState("notcrounch");
            _mainCam.transform.localPosition = new Vector3(_mainCam.transform.localPosition.x, 0f, _mainCam.transform.localPosition.z);
            _characterController.center = new Vector3(0, 0.9f, 0);
            _characterController.height = 1.8f;
            speed = 3f;
        }
    }

    private void LateUpdate()
    {
        if (!isPlayer)
            CheckPosition();
        
        if (isPlayer && LoockMode)
            Look();
    }

    private void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void Move() 
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 move = (transform.right * x + transform.forward * z).normalized;
        move = move * speed * Time.deltaTime;

        if (Input.GetKey(ButtonsManager.Instance.RunningButton))
            move *= runMultiplier;
        
        if (Input.GetKeyDown(ButtonsManager.Instance.JumpButton) && _characterController.isGrounded) 
            _yVelocity += _jumpMultiplier;

        _yVelocity += gravity * Time.deltaTime;

        move.y = _yVelocity * Time.deltaTime;

        _characterController.Move(move);
        relativeDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (_characterController.isGrounded && relativeDirection.magnitude > 0)
        {
            float horizontalSpeed = new Vector3(move.x, 0, move.z).magnitude / Time.deltaTime;
            float stepIntervalDynamic = stepInterval * (speed / horizontalSpeed);

            float slopeAngle = Vector3.Angle(Vector3.up, _characterController.velocity);
            if (slopeAngle > 45)
            {
                stepIntervalDynamic *= 1.5f; 
            }

            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                _footstepSystem.PlayFootstep();
                stepTimer = stepIntervalDynamic;
            }
        }
        else
            stepTimer = 0f;
    }

    private void CheckGround()
    {
        if (_characterController.isGrounded || _characterController.collisionFlags == CollisionFlags.Above) _yVelocity = -0.1f;

        if (_characterController.isGrounded)
            _characterController.slopeLimit = _originalSlopeLimit;
        else
            _characterController.slopeLimit = jumpSlopeLimit;
    }

    private void CheckGroundAnim()
    {
        Ray ray = new Ray(this.transform.position + Vector3.up * 2 * colExtents.x, Vector3.down);
        _animator.SetBool("isFly", Physics.SphereCast(ray, colExtents.x, colExtents.x + 0.3f));
    }
    
    public void SetSensitivity(float newSensitivity) => mouseSensitivity = newSensitivity;
    #endregion

    #region  Transform Synchronisation
    public void SendTransform()
    {
        if (isPlayer)
        {
            if (timeLastSending >= sendingPeriod)
            {
                //  lastState = SF2X_CharacterTransform.FromTransform(this.transform, this.spineTransform.localRotation);
                lastState = SF2X_CharacterTransform.FromTransform(this.transform);
                _transformSender.SendTransform(lastState);
                timeLastSending = 0;
                return;
            }
            timeLastSending += Time.deltaTime;
        }
    }

    public void ReceiveTransform(SF2X_CharacterTransform chtransform)
    {
        if (!isPlayer)
        {
            switch (NetworkManager.Instance.NetworkSyncMode)
            {
                case NetworkManager.InterpolationMode.Simple:  //SimpleLerp
                    {
                        newState = chtransform;
                        simpleLerp = true;
                    }
                    break;
                case NetworkManager.InterpolationMode.Complex:  //Interpolation
                    {
                        if (syncManager)
                            syncManager.ReceivedTransform(chtransform);
                    }
                    break;
            }
        }
    }

    public void CheckPosition()
    {
        if (!isPlayer)
        {
            if (simpleLerp)
            {
                isMoving = true;
                if (this.transform.position != newState.Position)
                {
                    this.transform.position = Vector3.Lerp(this.transform.position, newState.Position, Time.deltaTime * 100.0f);
                }
                if (this.transform.localEulerAngles != newState.AngleRotationFPS)
                {
                    this.transform.localEulerAngles = newState.AngleRotationFPS;
                }
             //   Quaternion targetRotation = newState.SpineRotationFPS;
             //   if (spineTransform.localRotation != targetRotation)
             //   {
             //       spineTransform.localRotation = targetRotation;
             //   }
            }
          //  else
          //  {
          //      spineTransform.localRotation = syncManager.spineRotation;
          //  }
        }
    }
    #endregion
}