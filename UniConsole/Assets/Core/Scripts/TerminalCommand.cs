using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class TerminalCommand : IEquatable<TerminalCommand>
{
    public readonly string Name;
    public readonly Type Class;
    public readonly MethodInfo Method;

    public TerminalCommand(string name, Type @class, MethodInfo method)
    {
        Name = name;
        Class = @class;
        Method = method;
    }
    
    public static TerminalCommand[] GetAutocompleteOptions(string command)
    {
        if (command == null)
            return null;

        var available = Reflector.Commands;

        return available?.Where(cmd => cmd.Name.StartsWith(command, StringComparison.OrdinalIgnoreCase)).ToArray();
    }

    public bool Equals(TerminalCommand other)
    {
        if (other == null) return false;
        return Name.Equals(other.Name) && Method.Equals(other.Method);
    }

    public string GetJoinedParameters()
    {
        return string.Join(" ", Method.GetParameters().Select(p => p.ParameterType.Name));
    }

    public override string ToString()
    {
        return Name + " " + GetJoinedParameters();
    }
}