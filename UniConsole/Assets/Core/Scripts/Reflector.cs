using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public static class Reflector
{
    private static BindingFlags Flags = BindingFlags.Static | BindingFlags.Public;
    private static TerminalCommand[] _commands = { };
    private static Type[] _classes = { };
    
    private static bool initialized = false;
    
    public static TerminalCommand[] Commands
    {
        get
        {
            if (!initialized)
            {
                throw new InvalidOperationException("The command cache has not been initialized yet. Call UpdateCommandCache() first.");
            }
            return _commands;
        }
    }

    public static Type[] Classes
    {
        get
        {
            if (!initialized)
            {
                throw new InvalidOperationException("The command cache has not been initialized yet. Call UpdateCommandCache() first.");
            }
            return _classes;
        }
    }

    
    /// <summary>
    /// Finds all static methods with the command attribute in the assembly. Available through the Commands property
    /// </summary>
    public static void UpdateCommandCache(bool allowPrivateCommands = false)
    {
        if (allowPrivateCommands)
        {
            Flags |= BindingFlags.NonPublic;
        }
        
        // Get all classes in the assembly
        var types = Assembly.GetExecutingAssembly().GetTypes();
        var methods = new List<TerminalCommand>();
        var classes = new List<Type>();
        foreach (var type in types)
        {
            // Get all methods in the class
            var typeMethods = type.GetMethods(Flags);
            foreach (var method in typeMethods)
            {
                // Check if the method has the Command attribute
                var commandAttribute = method.GetCustomAttribute<CommandAttribute>();
                if (commandAttribute == null) continue;
                methods.Add(new TerminalCommand(type, method));
                if (!classes.Contains(type)) classes.Add(type);
            }
        }
        
        _commands = methods.ToArray();
        _classes = types.ToArray();
        
        initialized = true;
    }
}