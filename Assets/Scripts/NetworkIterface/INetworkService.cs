using Sfs2X;
using Sfs2X.Entities;
using Sfs2X.Requests;

public interface INetworkService
{
    void SendRequest(ExtensionRequest request);
    void SubscribeToEvents(SmartFox sfs);
    void UnsubscribeFromEvents();
    Room GetLastJoinedRoom();
}
