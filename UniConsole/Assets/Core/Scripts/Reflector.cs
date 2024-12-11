using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

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
    /// Checks, if every command has a unique name
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    private static void ValidateCommands()
    {
        if (!CommandsAreValid()) 
            throw new InvalidOperationException("Commands are not valid. Make sure every command has a unique name.");
    }

    private static bool CommandsAreValid()
    {
        var commands = new List<TerminalCommand>();
        foreach (var command in Commands)
        {
            bool isValid = ValidateCommandName(command.Name, out var reason);
            if (commands.Contains(command))
            {
                isValid = false;
                reason = "Commands can not have the same name and the same parameters";
            }
            if (!isValid)
            {
                Debug.Log($"Invalid command: {command.Name} because {reason}");
                return false;
            }
            commands.Add(command);
        }

        return true;
    }

    private static bool ValidateCommandName(string name, out string reason)
    {
        if (name.Contains(' '))
        {
            reason = "Name can not contain white space";
            return false;
        }

        reason = null;
        return true;
    }
}