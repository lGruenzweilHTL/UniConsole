using System.Linq;

public static class ProbablyABug
{
    [Command]
    public static string TestCommand()
    {
        return "Shadowed bug";
    }
}