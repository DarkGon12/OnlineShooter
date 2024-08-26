using System.Collections.Generic;
using UnityEngine.PostProcessing;
using System.Reflection;
using UnityEngine;
using System;
using Sendlers;
using Zenject;

public static class ConsoleCommands
{
    private static SpawnRequestSender _spawnSender;

    [Inject]
    private static void Construct(SpawnRequestSender spawnSender)
    {
        _spawnSender = spawnSender;
    }

    public static void ExecuteCommand(string input)
    {
        string[] parts = input.Split(' ');

        if (parts.Length == 0)
        {
            Console.Send("Неверная комманда.", ConsoleMessageType.warning);
            return;
        }

        string command = parts[0];
        string[] parameters = parts.Length > 1 ? parts[1..] : Array.Empty<string>();

        if (command.Length > 1)
            command = char.ToUpper(command[1]) + command.Substring(2);
        else
            command = command.ToUpper();        

        MethodInfo method = typeof(ConsoleCommands).GetMethod(command.TrimStart('/'), BindingFlags.Public | BindingFlags.Static);
        if (method == null)
        {
            Console.Send($"Команда {command} не найдена.", ConsoleMessageType.warning);
            return;
        }

        ParameterInfo[] paramInfos = method.GetParameters();
        object[] convertedParams = new object[paramInfos.Length];

        for (int i = 0; i < paramInfos.Length; i++)
            convertedParams[i] = Convert.ChangeType(parameters[i], paramInfos[i].ParameterType);

        method.Invoke(null, convertedParams);
    }

    public static List<string> GetAvailableCommands()
    {
        List<string> commands = new List<string>();
        MethodInfo[] methods = typeof(ConsoleCommands).GetMethods(BindingFlags.Public | BindingFlags.Static);

        foreach (MethodInfo method in methods)
        {
            string commandName = method.Name;
            commands.Add("/" + commandName);
        }

        return commands;
    }

    #region Commands

    public static void SetRoomPlayerCount(int value)
    {
        if (LobbySceneController.Instance != null)
        {
            LobbySceneController.Instance.SetPlayerCount(value);
            Console.Send($"Размер комант изменён на: {value}", ConsoleMessageType.standart);
        }
        else
            Console.Send("Размер комнат можно меня только в лобби", ConsoleMessageType.warning);
    }

    public static void SetQuality(int value)
    {
        PostProcessingBehaviour postProcessing = Camera.main.GetComponent<PostProcessingBehaviour>();
        if (postProcessing != null)
        {
            if (value == 0)
                postProcessing.profile = Resources.Load<PostProcessingProfile>("Console/Quality/Shooting_0");
            if (value == 1)
                postProcessing.profile = Resources.Load<PostProcessingProfile>("Console/Quality/Shooting_1");
            if (value == 2)
                postProcessing.profile = Resources.Load<PostProcessingProfile>("Console/Quality/Shooting_2");

            Console.Send("Качество графики измененно на значение: " + value, ConsoleMessageType.standart);
        }
        else
            Console.Send("Качество графики можно изменять только находясь в игре: " + value, ConsoleMessageType.warning);
    }

    public static void Respawn()
    {
        FPSController[] controller = MonoBehaviour.FindObjectsOfType<FPSController>();
        FPSController player;

        for (int i = 0; i < controller.Length; i++)
        {
            if (controller[i].isPlayer)
            {
                player = controller[i];
                if (player.isDead)
                {
                    _spawnSender.ResurrectPlayer();
                    Console.Send("Восскресился", ConsoleMessageType.standart);
                }
                else
                    Console.Send("Вы ещё живы", ConsoleMessageType.warning);
            }
        }
    }

    public static void SetSensivity(float value)
    {
        FPSController[] controller = MonoBehaviour.FindObjectsOfType<FPSController>();
        FPSController player;

        for (int i = 0; i < controller.Length; i++)
        {
            if (controller[i].isPlayer)
            {
                player = controller[i];
                player.SetSensitivity(value);
                Console.Send("Вы изменили уровень чуствительности на " + value, ConsoleMessageType.standart);
            }
        }
    }

    public static void Help()
    {
        Console.Send("SetRoomPlayerCount - игроков в комнате [значение]", ConsoleMessageType.warning);
        Console.Send("SetQuality - установить значение графики [0/2]", ConsoleMessageType.warning);
        Console.Send("SetSensivity - установить значение чуствительности [value]", ConsoleMessageType.warning);
    }

    #endregion
}