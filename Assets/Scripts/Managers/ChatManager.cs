using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class ChatManager : MonoBehaviour
{
    [SerializeField] private float _inactiveHeight = 200f;
    [SerializeField] private float _activeHeight = 0f;

    [SerializeField] private Transform _messageContent;
    [SerializeField] private TMP_InputField _chatInput;
    [SerializeField] private RectTransform _chatContent;
    [SerializeField] private ScrollRect _scrollRect;

    private ConsoleManager _consoleManager;
    private CanvasGroup _consoleGroup;

    private FPSController _fpsController;

    private void Start()
    {
        _consoleManager = FindObjectOfType<ConsoleManager>();
        _consoleGroup = _consoleManager.GetComponent<CanvasGroup>();
        _fpsController = GetComponent<FPSController>();
        SetChatHeight(_inactiveHeight, false);
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.T) && _consoleGroup.alpha == 0)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = false;
            _chatInput.ActivateInputField();
            _chatInput.Select();
            SetChatHeight(_activeHeight, true);
            _fpsController.MoveMode = false;
            _fpsController.LoockMode = false;
        }

        if (_fpsController.MoveMode == false && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            SendMessage();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = true;
            SetChatHeight(_inactiveHeight,false);
            _fpsController.MoveMode = true;
            _fpsController.LoockMode = true;
        }
    }

    public void SendMessage()
    {
        if (_chatInput.text != "")
        {
            NetworkManager.Instance.SendChatMessage(_chatInput.text);
            _chatInput.text = "";
        }
    }

    public void CreateMessage(string user, string msg)
    {
        GameObject message = Instantiate(Resources.Load<GameObject>("Message/ChatMessage"), _messageContent);
        TextMeshProUGUI messageText = message.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        
        message.transform.SetSiblingIndex(0);
        messageText.text = $"{user}: {msg}";

        AdjustMessageHeight(message, messageText);
        _scrollRect.verticalNormalizedPosition = 0f;
    }

    public void SendSystem(string msg)
    {
        GameObject message = Instantiate(Resources.Load<GameObject>("Message/ChatMessage"), _messageContent);
        TextMeshProUGUI messageText = message.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        messageText.text = $"Server: {msg}";
        messageText.color = Color.yellow;

        message.transform.SetSiblingIndex(0);

        AdjustMessageHeight(message, messageText);
    }

    private void AdjustMessageHeight(GameObject message, TextMeshProUGUI messageText)
    {
        LayoutElement layoutElement = message.GetComponent<LayoutElement>();
        layoutElement.preferredHeight = messageText.preferredHeight + 10;
    }

    private void SetChatHeight(float height, bool isActive)
    {
        _chatContent.sizeDelta = new Vector2(_chatContent.sizeDelta.x, height);
        if (isActive)
        {
            _chatContent.anchorMin = new Vector2(0, 0);
            _chatContent.anchorMax = new Vector2(1, 0);
            _chatContent.pivot = new Vector2(0.5f, 0);
            _chatContent.anchoredPosition = new Vector2(0, 0);
        }
        else
        {
            _chatContent.anchorMin = new Vector2(0, 0);
            _chatContent.anchorMax = new Vector2(1, 0);
            _chatContent.pivot = new Vector2(0.5f, 0);
            _chatContent.anchoredPosition = new Vector2(0, 0);
        }
        _scrollRect.verticalNormalizedPosition = 0f;
    }
}