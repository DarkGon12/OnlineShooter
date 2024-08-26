using Sfs2X.Entities.Data;

public interface IItemManager
{
    void SpawnItem(ISFSObject itemData);
    void RemoveItem(int itemId);
}
