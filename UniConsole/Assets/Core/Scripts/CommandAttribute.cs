using System;

[AttributeUsage(AttributeTargets.Method)]
public class CommandAttribute : Attribute
{
    public string Name { get; }
    public string Description { get; }
    
    public CommandAttribute(string name, string description = "No description provided.")
    {
        Name = name;
        Description = description;
    }
}