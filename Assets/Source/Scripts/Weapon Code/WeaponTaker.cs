using Sendlers;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class WeaponTaker : MonoBehaviour
{
    [SerializeField] private RectTransform infoObject = null;
    [SerializeField] private Text infoText = null;
    [SerializeField] private Transform weaponContainer = null;
    [SerializeField] private float takeRange = 2.5f;
    [SerializeField] private LayerMask playerMask = new LayerMask();

    private Transform m_MainCameraTransform = null;
    private string m_TakeButton = null;

    private WeaponSender _weaponSender;
    private DiContainer _container;

    [Inject]
    private void Construct(WeaponSender fireSender, DiContainer container)
    {
        _weaponSender = fireSender;
        _container = container;
    }

    private void Start()
    {
        m_MainCameraTransform = Camera.main.transform;
        m_TakeButton = $"<color=\"#FFAA00\">{ButtonsManager.Instance.TakeItemButton}</color>";
    }

    private void Update()
    {
        if (Physics.Raycast(m_MainCameraTransform.position, m_MainCameraTransform.forward, out RaycastHit hit, takeRange, playerMask))
        {
            if (hit.transform.tag == "WeaponItem")
            {
                if (hit.transform.GetComponent<WeaponItem>() != null)
                {
                    WeaponItem currentWeaponItem = hit.transform.GetComponent<WeaponItem>();
                    ItemInfo itemInfo = hit.transform.GetComponent<ItemInfo>();

                    DisplayInfoText(currentWeaponItem);
                    Take(currentWeaponItem, itemInfo);
                }
            }
            else
            {
                infoObject.GetComponent<Image>().enabled = false;
                infoText.text = null;
            }
        }
        else
        {
            infoObject.GetComponent<Image>().enabled = false;
            infoText.text = null;
        }
    }

    private void Take(WeaponItem currentWeapon, ItemInfo info)
    {
        if (Input.GetKeyDown(ButtonsManager.Instance.TakeItemButton))
        {
            var weapon = Instantiate(currentWeapon.Prefab, weaponContainer);
            _container.InjectGameObject(weapon);

            SwitchWeapon switchWeapon = weaponContainer.GetComponent<SwitchWeapon>();
            switchWeapon.SelectWeapon();
            _weaponSender.SendWeaponChange(currentWeapon.Name);
            NetworkManager.Instance.SendPickUpItem(info.GetItemId());

            Destroy(currentWeapon.gameObject);
        }
    }

    private void DisplayInfoText(WeaponItem currentWeapon)
    {
        string weaponName = $"<color=\"#FFAA00\">{currentWeapon.Name}</color>";

        infoObject.GetComponent<Image>().enabled = true;
        infoText.text = "[" + m_TakeButton + "]" + " - Take " + weaponName;
    }
}
