using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class TerminalCommand : IEquatable<TerminalCommand>
{
    public readonly Type Class;
    public readonly MethodInfo Method;
    public  bool IsAmbiguous => Reflector.Commands.Count(Equals) > 1;

    public TerminalCommand(Type @class, MethodInfo method)
    {
        Class = @class;
        Method = method;
    }

    /// <summary>
    /// Returns only the name of the method itself.
    /// </summary>
    public string Name => Method.Name;

    /// <summary>
    /// Returns the full name of the method (including class and namespace).
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

    public bool Equals(TerminalCommand other)
    {
        bool nameEqual = Name.Equals(other.Name);
        var paramTypes = Method.GetParameters().Select(p => p.ParameterType).ToArray();
        var otherParamTypes = other.Method.GetParameters().Select(p => p.ParameterType).ToArray();

        bool paramsEqual = paramTypes.Equals(otherParamTypes);
        return nameEqual && paramsEqual;
    }
}