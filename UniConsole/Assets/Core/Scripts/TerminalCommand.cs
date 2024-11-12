using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class TerminalCommand
{
    public Type Class;
    public MethodInfo Method;

    public TerminalCommand(Type @class, MethodInfo method)
    {
        Class = @class;
        Method = method;
    }

    public bool IsAmbiguous
        => Reflector.Commands.Count(cmd => cmd.Name == Name) > 1;


    /// <summary>
    /// Returns only the name of the method itself.
    /// </summary>
    public string Name => Method.Name;

    /// <summary>
    /// Returns the full name of the method (including namespace).
    /// </summary>
    public string FullName => $"{Class.FullName}.{Name}";

    public string[] GetAllPossibleNames()
    {
        List<string> names = new()
        {
            Name,
            FullName
        };

        if (!string.IsNullOrWhiteSpace(Class.Namespace)) names.Add($"{Class.Namespace}.{FullName}");
        return names.ToArray();
    }
}