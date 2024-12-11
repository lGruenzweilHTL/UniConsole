using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "UniConsole/Config", fileName = "New UniConsoleConfig")]
public class UniConsoleConfigScriptableObject : ScriptableObject
{
    public Color MessageColor = Color.white;
    public Color WarningColor = Color.yellow;
    public Color ErrorColor = Color.red;
    
    [Space]
    
    [Tooltip("Should the Console print a help text when enabled?")] 
    public bool PrintHelpTextOnEnable = true;
    [Tooltip("Should the Console print a help text when cleared?")]
    public bool PrintHelpTextOnClear = true;
    [Tooltip("Should private methods with the command attribute be recognized as such?")]
    public bool AllowPrivateCommands = false;
    
    [Space]
    
    [Tooltip("The feedback message when a command with no return value is executed successfully")]
    public string VoidCommandFeedback = "Command executed successfully";
    
    [FormerlySerializedAs("CollectionSeparator")] [Tooltip("Separator for rendering collections")]
    public string CollectionSeparatorOutput = ", ";
    public char CollectionSeparatorInput = ',';
}