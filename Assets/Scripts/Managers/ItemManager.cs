using Sfs2X.Entities.Data;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour, IItemManager
{
    private Dictionary<int, GameObject> items = new Dictionary<int, GameObject>();

    public void SpawnItem(ISFSObject itemData)
    {
        // Логика спавна предметов
    }

    public void RemoveItem(int itemId)
    {
        if (items.ContainsKey(itemId))
        {
            Destroy(items[itemId]);
            items.Remove(itemId);
        }
    }
}
