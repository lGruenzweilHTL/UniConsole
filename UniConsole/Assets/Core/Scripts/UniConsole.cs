using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UniConsole : MonoBehaviour
{
    [SerializeField] private TMP_Text logText;
    [SerializeField] private TMP_InputField inputField;

    private readonly List<string> commandHistory = new();
    private int commandHistoryIndex = 0;
    
    public UnityEvent<string> OnCommandSubmitAttempted;
    public UnityEvent<MethodInfo> OnCommandSubmitted;
    public UnityEvent OnTerminalCleared;
    public UnityEvent OnTerminalAwake;

    private void Awake()
    {
        inputField.onSubmit.AddListener(OnInputFieldSubmit);
        OnTerminalCleared.AddListener(LogHelpText);
        OnTerminalAwake.AddListener(LogHelpText);
        
        OnTerminalAwake.Invoke();
    }

    private void LogHelpText()
    {
        Log("Type 'help' for a list of commands");
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

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            string command = inputField.text;
            var autocompletes = GetAutocompleteOptions(command);

            if (autocompletes.Length == 1)
            {
                // Only one option, complete the command
                inputField.text = autocompletes[0].Name;
                
                // Move the cursor to the end of the input field
                inputField.caretPosition = inputField.text.Length;
                
                return;
            }

            TerminalLog(command, string.Join(", ", autocompletes.Select(GetHelpString)));
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

    private void ExecuteCommand(string command)
    {
        OnCommandSubmitAttempted?.Invoke(command);
        
        var available = Reflector.Commands;
        string[] commandParts = command.Split(' ');
        string commandName = commandParts[0].Replace(" ", "");

        if (commandName.Equals("clear", StringComparison.OrdinalIgnoreCase))
        {
            logText.text = "";
            OnTerminalCleared?.Invoke();
            return;
        }

        foreach (var method in available)
        {
            if (method.Name.Equals(commandName, StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    object[] parameters = ParseParameters(commandParts[1..], method);
                    ParameterInfo[] expectedParameters = method.GetParameters();
                    object result = method.Invoke(null, parameters);
                    if (result != null)
                        TerminalLog(command, result);
                    
                    OnCommandSubmitted?.Invoke(method);
                }
                catch (Exception)
                {
                    TerminalLog(command, "Invalid number of arguments", LogType.Error);
                }

                return;
            }
        }

        Log(command);
    }

    private object[] ParseParameters(string[] parameters, MethodInfo method)
    {
        if (parameters.Length == 0)
            return null; // Invoke with no parameters

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

    private void TerminalLog(object command, object result, LogType logType = LogType.Message)
    {
        string log = $"> {command}\n{result}";

        if (logType == LogType.Message)
            Log(log);
        else if (logType == LogType.Warning)
            LogWarning(log);
        else if (logType == LogType.Error)
            LogError(log);
        else
            throw new ArgumentOutOfRangeException();
    }
    private void Log(object message)
    {
        logText.text += message + "\n";
    }
    private void LogWarning(object message)
    {
        Log("<color=\"yellow\">" + message + "</color>");
    }
    private void LogError(object message)
    {
        Log("<color=\"red\">" + message + "</color>");
    }

    private MethodInfo[] GetAutocompleteOptions(string command)
    {
        if (command == null) return null;

        var available = Reflector.Commands;

        if (available == null) return null;

        return available.Where(cmd => cmd.Name.StartsWith(command, StringComparison.OrdinalIgnoreCase)).ToArray();
    }

    [Command]
    public static void Clear()
    {
    }

    [Command]
    public static string Help()
        => "Available Commands:\n" + string.Join("\n", Reflector.Commands.Select(GetHelpString));

    private static string GetHelpString(MethodInfo method)
    {
        if (method == null) return null;
        return $"{method.Name} {string.Join(" ", method.GetParameters().Select(p => p.ParameterType.Name))}";
    }
}