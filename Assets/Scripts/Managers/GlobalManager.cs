using UnityEngine;
using UnityEngine.SceneManagement;

using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Util;


public class GlobalManager : MonoBehaviour
{
    private SmartFox sfs;
    private string connLostMsg;

    #region
    private void Awake() => Application.runInBackground = true;

    private void Update()
    {
        if (sfs != null)
            sfs.ProcessEvents();
    }

    private void OnApplicationQuit()
    {
        if (sfs != null && sfs.IsConnected)
            sfs.Disconnect();
    }
    #endregion

    #region
    public string ConnectionLostMessage
    {
        get
        {
            string m = connLostMsg;
            connLostMsg = null;
            return m;
        }
    }
   
    public SmartFox CreateSfsClient()
    {
        sfs = new SmartFox();
        sfs.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
        return sfs;
    }

    public SmartFox CreateSfsClient(UseWebSocket useWebSocket)
    {
        sfs = new SmartFox(useWebSocket);
        sfs.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
        return sfs;
    }

    public SmartFox GetSfsClient()
    {
        return sfs;
    }
    #endregion

    private void OnConnectionLost(BaseEvent evt)
    {
        sfs.RemoveEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
        sfs = null;

        string connLostReason = (string)evt.Params["reason"];

        Debug.Log("Connection to SmartFoxServer lost; reason is: " + connLostReason);

        if (SceneManager.GetActiveScene().name != "Login")
        {
            if (connLostReason != ClientDisconnectionReason.MANUAL)
            {
                connLostMsg = "An unexpected disconnection occurred.\n";

                if (connLostReason == ClientDisconnectionReason.IDLE)
                    connLostMsg += "It looks like you have been idle for too much time.";
                else if (connLostReason == ClientDisconnectionReason.KICK)
                    connLostMsg += "You have been kicked by an administrator or moderator.";
                else if (connLostReason == ClientDisconnectionReason.BAN)
                    connLostMsg += "You have been banned by an administrator or moderator.";
                else
                    connLostMsg += "The reason of the disconnection is unknown.";
            }

            SceneManager.LoadScene("Login");
        }
    }
}
