using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;

public class UniConsole : MonoBehaviour
{
    [SerializeField] private TMP_Text logText;
    [SerializeField] private TMP_InputField inputField;
    
    private readonly List<string> commandHistory = new();
    private int commandHistoryIndex = 0;

    private void Awake()
    {
        inputField.onSubmit.AddListener(OnInputFieldSubmit);
        Log("> ");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && commandHistoryIndex > 0)
        {
            commandHistoryIndex--;
            inputField.text = commandHistory[commandHistoryIndex];
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) && commandHistoryIndex < commandHistory.Count)
        {
            commandHistoryIndex++;
            inputField.text = commandHistoryIndex == commandHistory.Count ? "" : commandHistory[commandHistoryIndex];
        }
    }

    private void OnInputFieldSubmit(string command)
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ExecuteCommand(command);
            
            commandHistory.Add(command);
            commandHistoryIndex = commandHistory.Count;
            
            inputField.text = "";
            inputField.ActivateInputField();
        }
    }

    public void ExecuteCommand(string command)
    {
        var available = Reflector.Commands;
        string[] commandParts = command.Split(' ');
        string commandName = commandParts[0].Replace(" ", "");

        if (commandName.Equals("clear", StringComparison.OrdinalIgnoreCase))
        {
            logText.text = "";
            return;
        }

        foreach (var method in available)
        {
            if (method.Name.Equals(commandName, StringComparison.OrdinalIgnoreCase))
            {
                var parameters = ParseParameters(commandParts[1..], method);
                var result = method.Invoke(null, parameters);
                if (result != null) TerminalLog(command, result);
                return;
            }
        }

        TerminalLog(command, command);
    }

    private object[] ParseParameters(string[] parameters, MethodInfo method)
    {
        if (parameters.Length == 0) return null; // Invoke with no parameters

        object[] parsedParameters = new object[parameters.Length];
        Type[] targetTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();

        for (var i = 0; i < parsedParameters.Length; i++)
        {
            parsedParameters[i] = targetTypes[i].IsEnum
                ? Enum.Parse(targetTypes[i], parameters[i], true)
                : Convert.ChangeType(parameters[i], targetTypes[i]);
        }

        return parsedParameters;
    }

    private void TerminalLog(object command, object result)
    {
        Log($"{command}\n{result}\n> ");
    }
    private void Log(object message)
    {
        logText.text += message;
    }

    [Command]
    public static void Clear()
    {
    }

    [Command]
    public static string Help()
        => "Available Commands:\n" + string.Join("\n", Reflector.Commands.Select(cmd =>
            $"{cmd.Name} {string.Join(" ", cmd.GetParameters().Select(p => p.ParameterType.Name))}"));
}