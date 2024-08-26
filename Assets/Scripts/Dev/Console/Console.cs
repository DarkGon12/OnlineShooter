using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public static class Console
{
    public static List<GameObject> MessageList = new List<GameObject>();
    private static Transform _consoleContent, _suggestionContent;

    public static int MessageLimit{  get; private set; }

    private static TMP_InputField _inputField;
    private static GameObject _suggestionPrefab;
    private static List<GameObject> _suggestions = new List<GameObject>();

    public static void InitalizeConsole(Transform content, Transform sugContent, TMP_InputField field)
    {
        _suggestionPrefab = Resources.Load<GameObject>("Console/suggestion");
        _suggestionContent = sugContent;
        _consoleContent = content;
        _inputField = field;

        MessageLimit = 20;
        Send("Консоль инициализированна", ConsoleMessageType.standart);
    }

    private static void ClearSuggestions()
    {
        foreach (GameObject suggestion in _suggestions)
            GameObject.Destroy(suggestion);
        
        _suggestions.Clear();
    }

    public static void UpdateSuggestions(string input)
    {
        ClearSuggestions();
        if (string.IsNullOrEmpty(input) || !input.StartsWith("/")) return;
        if (input.Length < 1) return;

        string normalizedInput = "/" + input.Substring(1).ToLower();
        List<string> commands = ConsoleCommands.GetAvailableCommands();
        foreach (string command in commands)
        {
           if (command.ToLower().StartsWith(normalizedInput))
            {
                GameObject suggestion = MonoBehaviour.Instantiate(_suggestionPrefab, _suggestionContent);
                TextMeshProUGUI suggestionText = suggestion.GetComponentInChildren<TextMeshProUGUI>();
                suggestionText.text = command;
                _suggestions.Add(suggestion);

                suggestion.GetComponent<Button>().onClick.AddListener(() =>
                {
                    _inputField.text = command;
                    _inputField.Select();
                });
            }
        }
    }

    public static void UpdateConsole()
    {
        if (MessageList.Count >= MessageLimit)
        {
            MonoBehaviour.Destroy(MessageList[9]);
            MessageList.RemoveAt(9);
        }
    }

    public static void Send(string message, ConsoleMessageType type)
    {
        if (message.Length == 0)
            return;
        if (message == "`" || message == "~")
            return;
        if (message[0].ToString() == "/")
        {
            ConsoleCommands.ExecuteCommand(message);
            return;
        }

        Color messageCollor = new Color(0, 0, 0, 1);

        switch (type)
        {
            case ConsoleMessageType.standart:
                messageCollor = Color.white;
                break;
            case ConsoleMessageType.warning:
                messageCollor = Color.yellow;
                break;
            case ConsoleMessageType.error:
                messageCollor = Color.red;
                break;
            case ConsoleMessageType.critical:
                messageCollor = Color.cyan;
                break;
        }

        GameObject messagePrefab = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Console/message"), _consoleContent);
        TextMeshProUGUI context = (TextMeshProUGUI)messagePrefab.GetComponentInChildren<TextMeshProUGUI>();
        MessageList.Add(messagePrefab);

        context.text = message;
        context.color = messageCollor;
    }
}

public enum ConsoleMessageType
{
    standart,
    warning,
    error,
    critical
}