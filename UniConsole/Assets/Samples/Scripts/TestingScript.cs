public static class TestingScript
{
    [Command]
    public static string TestCommand()
    {
        return "hehehehaaaa";
    }

    [Command]
    public static string TestCommand2()
    {
        return "Hello, World!";
    }

    [Command]
    public static string ComplexCommand(int num)
    {
        return "Square of " + num + " is " + num * num;
    }

    [Command]
    public static string Add(int a, int b)
    {
        return (a + b).ToString();
    }

    [Command]
    public static string TestCommand(string name)
    {
        return $"This is a method overload to say the name {name}";
    }
}