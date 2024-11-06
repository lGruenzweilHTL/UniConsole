using System.Collections.Generic;
using System.Reflection;
public static class Reflector
{
    
    /// <summary>
    /// Finds all public static methods with no parameters and the command attribute in the assembly.
    /// TODO cache the result
    /// </summary>
    /// <returns></returns>
    public static MethodInfo[] GetCommands()
    {
        // Get all classes in the assembly
        var types = Assembly.GetExecutingAssembly().GetTypes();
        var methods = new List<MethodInfo>();
        foreach (var type in types)
        {
            // Get all methods in the class
            var typeMethods = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
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
        
        return methods.ToArray();
    }
}