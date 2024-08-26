using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Requests;
using Sfs2X.Util;

public class LoginSceneController : BaseSceneController
{
	public string host = "127.0.0.1";
	public int tcpPort = 9933;
    public int UdpPort = 9933;
	public string zone = "BasicExamples";
	public bool debug = false;

	public InputField nameInput;
	public Button loginButton;
    public Text errorText;

	private SmartFox sfs;

	private void Start()
	{
		nameInput.Select();
		nameInput.ActivateInputField();

		string connLostMsg = _gm.ConnectionLostMessage;
		if (connLostMsg != null)
			errorText.text = connLostMsg;
	}

	#region
	public void OnNameInputEndEdit()
	{
		if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
			Connect();
	}

	public void OnLoginButtonClick()
	{
		if (nameInput.text.Length < 4)
		{
			errorText.text = "Слишком короткий ник";
			return;
		}
		else if (nameInput.text.Length > 11)
		{
			errorText.text = "Слишком длинный ник";
			return;
		}
		else
			Connect();
	}
	#endregion

	#region
	private void EnableUI(bool enable)
	{
		nameInput.interactable = enable;
		loginButton.interactable = enable;
    }

	private void Connect()
	{
		EnableUI(false);

		errorText.text = "";

		ConfigData cfg = new ConfigData();
		cfg.Host = host;
		cfg.Port = tcpPort;
		cfg.UdpHost = host;
		cfg.UdpPort = UdpPort;
		cfg.Zone = zone;
		cfg.Debug = debug;

		sfs = _gm.CreateSfsClient();
		sfs.Logger.EnableConsoleTrace = debug;
		AddSmartFoxListeners();
		sfs.Connect(cfg);
	}

	private void AddSmartFoxListeners()
	{
		sfs.AddEventListener(SFSEvent.CONNECTION, OnConnection);
		sfs.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
		sfs.AddEventListener(SFSEvent.LOGIN, OnLogin);
		sfs.AddEventListener(SFSEvent.LOGIN_ERROR, OnLoginError);
        sfs.AddEventListener(SFSEvent.UDP_INIT, OnUdpInit);
    }

	override protected void RemoveSmartFoxListeners()
	{
		if (sfs != null)
		{
			sfs.RemoveEventListener(SFSEvent.CONNECTION, OnConnection);
			sfs.RemoveEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
			sfs.RemoveEventListener(SFSEvent.LOGIN, OnLogin);
			sfs.RemoveEventListener(SFSEvent.LOGIN_ERROR, OnLoginError);
            sfs.RemoveEventListener(SFSEvent.UDP_INIT, OnUdpInit);
        }
	}

	override protected void HideModals()
	{
		// No modals used by this scene
	}
	#endregion

	#region
	private void OnConnection(BaseEvent evt)
	{
		if ((bool)evt.Params["success"])
		{
			Console.Send("Сервер API версия: " + sfs.Version, ConsoleMessageType.standart);
			Console.Send("Сервер мод: " + sfs.ConnectionMode, ConsoleMessageType.standart);

			sfs.Send(new LoginRequest(nameInput.text));
		}
		else
		{
			errorText.text = "Соединение не установленно";
			EnableUI(true);
		}
	}

	private void OnConnectionLost(BaseEvent evt)
	{
		RemoveSmartFoxListeners();

		string reason = (string)evt.Params["reason"];
		
		if (reason != ClientDisconnectionReason.MANUAL)
			errorText.text = "Соединение разорванно: " + reason;

		EnableUI(true);
    }

    private void OnLogin(BaseEvent evt) => sfs.InitUDP();
    
	private void OnLoginError(BaseEvent evt)
	{
		sfs.Disconnect();
		errorText.text = "Авторизация не удалась:\n" + (string)evt.Params["errorMessage"];
		EnableUI(true);
	}

	private void OnUdpInit(BaseEvent evt)
	{
		if ((bool)evt.Params["success"])
            SceneManager.LoadScene("Lobby");	
		else
		{
			sfs.Disconnect();
			errorText.text = "Ошибка инициализации UDProtocol:\n" + (string)evt.Params["errorMessage"];
			EnableUI(true);
		}
	}
    #endregion
}