public class CommandAttribute : System.Attribute
{
    public string Description { get; }
    
    public CommandAttribute(string description = "No description provided.")
    {
        Description = description;
    }
}