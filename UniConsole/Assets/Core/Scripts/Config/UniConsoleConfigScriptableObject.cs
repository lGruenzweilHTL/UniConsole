using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "UniConsole/Config", fileName = "New UniConsoleConfig")]
public class UniConsoleConfigScriptableObject : ScriptableObject
{
    [Header("Theme")]
    public Color MessageColor = Color.white;
    public Color WarningColor = Color.yellow;
    public Color ErrorColor = Color.red;
    
    [Header("Help")]
    [Tooltip("Should the Console print a help text when enabled?")] 
    public bool PrintHelpTextOnEnable = true;
    [Tooltip("Should the Console print a help text when cleared?")]
    public bool PrintHelpTextOnClear = true;

    [Tooltip("Should the console be cleared when enabled?")]
    public bool ClearOnEnable = true;
    
    [Header("Reflection")]
    [Tooltip("Should private methods with the command attribute be recognized as such?")]
    public bool AllowPrivateCommands = false;

    [Header("Debugging")] 
    [Tooltip("Should the Console listen for debug messages when disabled?")]
    public bool InterceptWhenDisabled = false;
    [Tooltip("Should the Console log messages in the debug console?")]
    public bool InterceptMessages = false;
    [Tooltip("Should the Console log warnings in the debug console?")]
    public bool InterceptWarnings = true;
    [Tooltip("Should the Console log errors in the debug console?")]
    public bool InterceptErrors = true;
    [Tooltip("Should the Console include the stack trace when logging messages?")]
    public bool IncludeStackTraceOnMessage = false;
    [Tooltip("Should the Console include the stack trace when logging warnings or errors?")]
    public bool IncludeStackTraceOnErrors = true;
    
    [Header("Feedback")]
    [Tooltip("The feedback message when a command with no return value is executed successfully")]
    public string VoidCommandFeedback = "Command executed successfully";
    
    [Header("Input")]
    [Tooltip("Separator for rendering collections")]
    public string CollectionSeparatorOutput = ", ";
    public char CollectionSeparatorInput = ',';
}