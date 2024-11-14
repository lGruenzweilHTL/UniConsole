public static class TestingScript
{
    [Command("Prints test things")]
    public static string TestCommand()
    {
        return "hehehehaaaa";
    }

    [Command("Prints test things")]
    public static string TestCommand2()
    {
        return "Hello, World!";
    }

    [Command("Does something complex")]
    public static string ComplexCommand(int num)
    {
        return "Square of " + num + " is " + num * num;
    }

    [Command("Adds two numbers")]
    public static string Add(int a, int b)
    {
        return (a + b).ToString();
    }
    [Command("Adds three numbers")]
    public static int Add(int a, int b, int c)
    {
        return a + b + c;
    }
}

public static class TestClass
{
    [Command("Does testing things from another class")]
    public static string TestCommand() => "TestCommand";
}