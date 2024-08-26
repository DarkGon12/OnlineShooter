using UnityEngine;

public class ItemInfo : MonoBehaviour
{
    [SerializeField] private string _itemName;
    [SerializeField] private int _itemId;

    public void SetItemInfo(string itemName, int itemId)
    {
        _itemName = itemName;
        _itemId = itemId;
    }

    public string GetItemName() => _itemName;

    public int GetItemId() => _itemId; 
}