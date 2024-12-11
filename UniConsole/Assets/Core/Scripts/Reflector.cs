using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public static class Reflector
{
    private static BindingFlags Flags = BindingFlags.Static | BindingFlags.Public;
    private static TerminalCommand[] _commands = { };
    
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
        foreach (var type in types)
        {
            // Get all methods in the class
            var typeMethods = type.GetMethods(Flags);
            foreach (var method in typeMethods)
            {
                // Check if the method has the Command attribute
                var commandAttribute = method.GetCustomAttribute<CommandAttribute>();
                if (commandAttribute == null) continue;
                methods.Add(new TerminalCommand(commandAttribute.Name, type, method));
            }
        }
        
        _commands = methods.ToArray();
        
        initialized = true;
        ValidateCommands();
    }

    /// <summary>
    /// Checks, if every command has a unique name and validates the individual command names.
    /// Command names must be alphanumeric
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    private static void ValidateCommands()
    {
        if (!CommandsAreValid(out var cmd)) 
            throw new InvalidOperationException($"The command {cmd} is invalid. Command names must be alphanumeric and unique.");
    }

    private static bool CommandsAreValid(out string cmd)
    {
        var commands = new List<TerminalCommand>();
        foreach (var command in Commands)
        {
            if (!ValidateCommandName(command.Name) || commands.Contains(command))
            {
                cmd = UnescapeString(command.Name);
                return false;
            }
            commands.Add(command);
        }

        cmd = null;
        return true;
    }

    private static string UnescapeString(string name)
    {
        // Unescape every character that is not alphanumeric
        return string.Join("", name.Where(char.IsLetterOrDigit));
    }

    private static bool ValidateCommandName(string name)
    {
        return name.All(char.IsLetterOrDigit);
    }
}