using System.Collections.Generic;
using System.Reflection;
public static class Reflector
{
    private static TerminalCommand[] _commands;
    public static TerminalCommand[] Commands
    {
        get
        {
            if (_commands == null || _commands.Length == 0)
            {
                UpdateCommandCache();
            }
            return _commands;
        }
    }

    
    /// <summary>
    /// Finds all static methods with the command attribute in the assembly. Available through the Commands property
    /// </summary>
    public static void UpdateCommandCache()
    {
        // Get all classes in the assembly
        var types = Assembly.GetExecutingAssembly().GetTypes();
        var methods = new List<TerminalCommand>();
        foreach (var type in types)
        {
            // Get all methods in the class
            var typeMethods = type.GetMethods(BindingFlags.Static | BindingFlags.Public);
            foreach (var method in typeMethods)
            {
                // Check if the method has the Command attribute
                var commandAttribute = method.GetCustomAttribute<CommandAttribute>();
                if (commandAttribute != null)
                {
                    methods.Add(new TerminalCommand(type, method));
                }
            }
        }
        
        _commands = methods.ToArray();
    }
}