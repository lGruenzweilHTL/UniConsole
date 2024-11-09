using System.Collections.Generic;
using System.Reflection;
public static class Reflector
{
    private static MethodInfo[] _commands;
    public static MethodInfo[] Commands
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
    /// Finds all static methods with the command attribute in the assembly.
    /// </summary>
    /// <returns></returns>
    public static void UpdateCommandCache()
    {
        // Get all classes in the assembly
        var types = Assembly.GetExecutingAssembly().GetTypes();
        var methods = new List<MethodInfo>();
        foreach (var type in types)
        {
            // Get all methods in the class
            var typeMethods = type.GetMethods(BindingFlags.Static);
            foreach (var method in typeMethods)
            {
                // Check if the method has the Command attribute
                var commandAttribute = method.GetCustomAttribute<CommandAttribute>();
                if (commandAttribute != null)
                {
                    methods.Add(method);
                }
            }
        }
        
        _commands = methods.ToArray();
    }
}