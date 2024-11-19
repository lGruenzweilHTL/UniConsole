using System;

[AttributeUsage(AttributeTargets.Method)]
public class CommandAttribute : Attribute
{
    public string Description { get; }
    
    public CommandAttribute(string description = "No description provided.")
    {
        Description = description;
    }
}