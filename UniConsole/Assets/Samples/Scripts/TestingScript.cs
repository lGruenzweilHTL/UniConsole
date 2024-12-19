public static class TestingScript
{
    [Command("test-cmd")]
    public static string[] LongTestingCommandWithSpace(string[] args)
    {
        return args;
    }
}