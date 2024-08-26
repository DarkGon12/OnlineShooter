using Sfs2X;
using Sfs2X.Entities.Data;

public interface IPlayerManager
{
    void HandleSpawnPlayer(ISFSObject playerData, SmartFox sfs);
    void UpdatePlayerTransform(int userId, SF2X_CharacterTransform transform);
}
