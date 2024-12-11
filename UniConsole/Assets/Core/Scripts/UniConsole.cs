using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class UniConsole : MonoBehaviour
{
    private const string HELP_TEXT = "Type 'help' for a list of commands";

    [SerializeField] private UniConsoleConfigScriptableObject config;
    [Space] [SerializeField] private TMP_Text logText;
    [SerializeField] private TMP_InputField inputField;

    private readonly List<string> commandHistory = new();
    private int commandHistoryIndex = 0;

    public UnityEvent<string> OnCommandSubmitAttempted;
    public UnityEvent<TerminalCommand> OnCommandSubmitted;
    public UnityEvent OnTerminalCleared;
    public UnityEvent OnTerminalEnabled;

    private void Awake()
    {
        inputField.onSubmit.AddListener(OnInputFieldSubmit);
        OnTerminalCleared.AddListener(() =>
        {
            if (config.PrintHelpTextOnClear) Log(HELP_TEXT);
        });
        OnTerminalEnabled.AddListener(() =>
        {
            if (config.ClearOnEnable)
            {
                ClearTerminal();
            }
            else
            {
                if (config.PrintHelpTextOnEnable) Log(HELP_TEXT);
            }
        });
        
        Application.logMessageReceived += HandleDebugLog;
        Reflector.UpdateCommandCache(config.AllowPrivateCommands);
    }

    private void OnEnable()
    {
        OnTerminalEnabled?.Invoke();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && commandHistoryIndex > 0)
        {
            commandHistoryIndex--;
            inputField.text = commandHistory[commandHistoryIndex];
            inputField.caretPosition = inputField.text.Length;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) && commandHistoryIndex < commandHistory.Count)
        {
            commandHistoryIndex++;
            inputField.text = commandHistoryIndex == commandHistory.Count ? "" : commandHistory[commandHistoryIndex];
            inputField.caretPosition = inputField.text.Length;
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            HandleAutocomplete(inputField.text);
        }
    }

    private void HandleAutocomplete(string command)
    {
        string[] parts = command.Split(' ');
        command = parts[^1];

        var autocompletes = TerminalCommand.GetAutocompleteOptions(command);
        var options = autocompletes
            .Select(c => c.Name)
            .ToArray();
        var formatted = autocompletes
            .Select(c => c.ToString());

        if (options.Length == 0)
            return;

        int diffIdx = GetEarliestDifferenceIndex(options);
        if (diffIdx != command.Length)
        {
            // Calculate final autocomplete
            parts[^1] = options[0][..diffIdx];
            string complete = string.Join(' ', parts);

            // Only one option, complete the command
            inputField.text = complete;

            // Move the cursor to the end of the input field
            inputField.caretPosition = complete.Length;
        }
        else
            TerminalLog(command, string.Join(", ", formatted));
    }

    private static int GetEarliestDifferenceIndex(string[] strings)
    {
        if (strings.Length == 1)
            return strings[0].Length;

        int idx = int.MaxValue;
        for (int i = 0; i < strings.Length - 1; i++)
        {
            int limit = Mathf.Min(strings[i].Length, strings[i + 1].Length);
            for (int j = 0; j < limit; j++)
            {
                if (strings[i][j] != strings[i + 1][j])
                {
                    idx = Mathf.Min(j, idx);
                    break;
                }
            }

            idx = Mathf.Min(limit, idx);
        }

        return idx;
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

    private void ExecuteCommand(string commandToExecute)
    {
        OnCommandSubmitAttempted?.Invoke(commandToExecute);

        var available = Reflector.Commands;
        string[] commandParts = commandToExecute.Split(' ');
        string commandName = commandParts[0];

        if (commandName.Equals("clear", StringComparison.OrdinalIgnoreCase))
        {
            ClearTerminal();
            return;
        }

        foreach (var command in available)
        {
            if (!command.Name.Equals(commandName, StringComparison.OrdinalIgnoreCase))
                continue;

            // Not the correct parameter count
            var expectedParameters = command.Method.GetParameters();
            if (commandParts.Length - 1 != expectedParameters.Length)
                continue;

            // Parse parameters
            object[] parameters = { };
            try
            {
                parameters = ParseParameters(commandParts[1..], command.Method);
            }
            catch (Exception)
            {
                TerminalLog(commandToExecute, "Could not Parse parameters", UniConsoleLogType.Error);
                return;
            }

            // Everything is correct, actually execute the command

            if (expectedParameters.Length == 0) // No parameters, needs null to work correctly
                expectedParameters = null;

            // Check if parameters are the same
            if (parameters == null ^ expectedParameters == null)
                continue;
            if (parameters != null && expectedParameters != null && parameters.Length != expectedParameters.Length)
                continue;

            object result = command.Method.Invoke(null, parameters);
            TerminalLog(commandToExecute, GetParsedResult(result));

            OnCommandSubmitted?.Invoke(command);

            // Command executed successfully
            return;
        }

        Log(commandToExecute);
    }

    private string GetParsedResult(object result)
    {
        if (result == null)
            return config.VoidCommandFeedback;
        
        if (result is string str)
            return str;

        if (result.GetType().IsArray && result.GetType().GetArrayRank() > 1)
            throw new NotSupportedException("Multi-dimensional arrays are not supported");
        
        if (result is IEnumerable collection)
            return string.Join(config.CollectionSeparatorOutput, collection.Cast<object>());

        return result.ToString();
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
                : targetTypes[i].IsArray
                    ? ParseArray(parameters[i], targetTypes[i].GetElementType())
                : Convert.ChangeType(parameters[i], targetTypes[i]);
        }

        return parsedParameters;
    }

    private object ParseArray(string param, Type elementType)
    {
        var inputArray = param.Split(config.CollectionSeparatorInput);
        Array result = Array.CreateInstance(elementType, inputArray.Length);
        
        for (int i = 0; i < inputArray.Length; i++)
        {
            result.SetValue(Convert.ChangeType(inputArray[i], elementType), i);
        }
        
        return result;
    }
    
    private void HandleDebugLog(string log, string stackTrace, LogType type)
    {
        bool enabled = gameObject.activeInHierarchy && this.enabled;
        if (!config.InterceptWhenDisabled && !enabled)
            return;

        if (type == LogType.Log && config.InterceptMessages)
        {
            Log(log);
            if (config.IncludeStackTraceOnMessage) Log(stackTrace);
            return;
        }

        if (type == LogType.Warning && config.InterceptWarnings)
        {
            LogWarning(log);
            if (config.IncludeStackTraceOnErrors) LogWarning(stackTrace);
            return;
        }

        if (type is LogType.Error or LogType.Exception && config.InterceptErrors)
        {
            LogError(log);
            if (config.IncludeStackTraceOnErrors) LogError(stackTrace);
        }

        
    }

    private void TerminalLog(object command, object result, UniConsoleLogType uniConsoleLogType = UniConsoleLogType.Message)
    {
        string log = $"> {command}\n{result}";

        switch (uniConsoleLogType)
        {
            case UniConsoleLogType.Message:
                Log(log);
                break;
            case UniConsoleLogType.Warning:
                LogWarning(log);
                break;
            case UniConsoleLogType.Error:
                LogError(log);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void Log(object message)
    {
        logText.text += $"<color={config.MessageColor.ToTMPColorCode()}>{message}</color>\n";
    }

    private void LogWarning(object message)
    {
        Log($"<color={config.WarningColor.ToTMPColorCode()}>{message}</color>");
    }

    private void LogError(object message)
    {
        Log($"<color={config.ErrorColor.ToTMPColorCode()}>{message}</color>");
    }

    [Command("clear", "clears the console")]
    public static void Clear()
    {
        // handled in ExecuteCommand because it needs the reference to the logText
    }

    private void ClearTerminal()
    {
        ClearTerminalNoInvoke();
        OnTerminalCleared?.Invoke();
    }
    private void ClearTerminalNoInvoke()
    {
        logText.text = "";
    }

    [Command("help", "displays the description of a command")]
    public static string Help(string commandName)
        => string.Join('\n', Reflector.Commands.Where(
                c => c.Name.Equals(commandName, StringComparison.OrdinalIgnoreCase))
            .Select(cmd =>
            {
                string description = cmd.Method.GetCustomAttribute<CommandAttribute>().Description;
                string name = cmd.ToString();

                return $"{name}\n\t{description}";
            }));


    [Command("help", "lists all commands")]
    public static string Help()
        => "Available Commands:\n" + string.Join("\n",
            Reflector.Commands
                .Select(c => c.ToString()));

    [Command("hierarchy", "prints the unity project hierarchy")]
    public static string PrintHierarchy()
    {
        string result = "";
        foreach (var obj in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            result += GetHierarchy(obj.transform);
        }

        return result;

        // Recursively print the hierarchy
        string GetHierarchy(Transform transform, int depth = 0)
        {
            string indent = new string(' ', depth * 2);
            string result = indent + transform.name + "\n";

            for (int i = 0; i < transform.childCount; i++)
                result += GetHierarchy(transform.GetChild(i), depth + 1);

            return result;
        }
    }

    [Command("exit", "leaves the game using Application.Quit()")]
    public static void Exit()
    {
        Application.Quit();
    }
}