using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class ConsoleManager : MonoBehaviour
{
    [SerializeField] private FPSController _fpsConetoller;
    [Space(5)]
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private Transform _suggestionContent;
    [SerializeField] private Transform _consoleContent;
    [SerializeField] private Button _sendButton;

    private CanvasGroup _canvasGroup;
    private bool _lastCursorState;

    public void SetFPSController(FPSController controller) => _fpsConetoller = controller;

    private void Awake() => Console.InitalizeConsole(_consoleContent, _suggestionContent, _inputField);

    private void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();

        _inputField.onValueChanged.AddListener(Console.UpdateSuggestions);
        _sendButton.onClick.AddListener(Send);
        DontDestroyOnLoad(gameObject);
    }

    private void Send()
    {
        Console.Send(_inputField.text, ConsoleMessageType.standart);
        Console.UpdateConsole();
        _inputField.text = "";
    }

    private void Update()
    {
        if (Input.GetButtonDown("Tilde"))
        {
            if (_canvasGroup.alpha == 0)
                OpenConsole();
            else
                CloseConsole();
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            Send();
    }

    private void OpenConsole()
    {
        _canvasGroup.alpha = 1;
        _canvasGroup.blocksRaycasts = true;

        _inputField.Select();
        _inputField.ActivateInputField();

        if (Cursor.lockState == CursorLockMode.Locked)
            _lastCursorState = true;
        else
            _lastCursorState = false;

        if (_fpsConetoller != null)
        {
            _fpsConetoller.LoockMode = false;
            _fpsConetoller.MoveMode = false;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void CloseConsole()
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.blocksRaycasts = false;

        _inputField.DeactivateInputField();

        if (!_lastCursorState)
            Cursor.lockState = CursorLockMode.None;
        else
            Cursor.lockState = CursorLockMode.Locked;

        if (_fpsConetoller != null)
        {
            _fpsConetoller.LoockMode = true;
            _fpsConetoller.MoveMode = true;
        }

        Cursor.visible = _lastCursorState;
    }    
}