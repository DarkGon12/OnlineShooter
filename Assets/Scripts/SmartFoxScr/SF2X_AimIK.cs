using UnityEngine;

public class SF2X_AimIK : MonoBehaviour
{
    public Transform spine;
    private Camera cam;
    public bool isPlayer;
    public bool shoot;
    public GameObject muzzleFlash;
    public Transform barrelLocation;
    private GameObject flash;
    private Animator animator;
    private float destroyTimer = 0.1f;

    private void Awake()
    {
        cam = Camera.main;
        animator = GetComponentInParent<Animator>();
        spine = animator.GetBoneTransform(HumanBodyBones.Spine).transform;
    }

    private void LateUpdate()
    {
        if (this.isPlayer)
        {
            Vector3 mainCamPos = cam.transform.position;
            Vector3 dir = cam.transform.forward;
            Ray ray = new Ray(mainCamPos, dir);
            spine.LookAt(ray.GetPoint(40), Vector3.up);
        }
    }

    private void Update()
    {
        if (shoot)
        {
            shoot = false;
            flash = Instantiate(muzzleFlash, barrelLocation.position, barrelLocation.rotation);
            Destroy(flash, destroyTimer);
        }
    }
}