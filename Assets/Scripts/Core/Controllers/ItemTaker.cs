using UnityEngine.UI;
using UnityEngine;

public class ItemTaker : MonoBehaviour
{
    [SerializeField] private RectTransform infoObject = null;
    [SerializeField] private Text infoText = null;

    [SerializeField] private float takeRange = 2.5f;
    [SerializeField] private LayerMask playerMask = new LayerMask();

    private Transform m_MainCameraTransform = null;
    private string m_TakeButton = null;

    private void Start()
    {
        m_MainCameraTransform = Camera.main.transform;
        m_TakeButton = $"<color=\"#FFAA00\">{ButtonsManager.Instance.TakeItemButton}</color>";
    }

    private void Update()
    {
        if (Physics.Raycast(m_MainCameraTransform.position, m_MainCameraTransform.forward, out RaycastHit hit, takeRange, playerMask))
        {
            if (hit.transform.tag == "Item")
            {
                if (hit.transform.GetComponent<ItemInfo>() != null)
                {
                    ItemInfo item = (ItemInfo)hit.transform.GetComponent<ItemInfo>();

                    DisplayInfoText(item.GetItemName());
                    TakeEntity(item.GetItemId());
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

    private void TakeEntity(int itemId)
    {
        if (Input.GetKeyDown(ButtonsManager.Instance.TakeItemButton))
        {
            NetworkManager.Instance.SendPickUpItem(itemId);
        }
    }

    private void DisplayInfoText(string itemName)
    {
        string info = $"<color=\"#FFAA00\">{itemName}</color>";

        infoObject.GetComponent<Image>().enabled = true;
        infoText.text = "[" + m_TakeButton + "]" + " - Take " + info;
    }
}
