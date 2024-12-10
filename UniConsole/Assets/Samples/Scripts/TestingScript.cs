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

    [Command("Returns an array of random numbers")]
    public static int[] GetRandomArray(int length)
    {
        var random = new System.Random();
        var array = new int[length];
        for (int i = 0; i < length; i++)
        {
            array[i] = random.Next(0, 100);
        }

        return array;
    }
    
    [Command("Returns a higher dimensional array of random numbers")]
    public static int[,] GetRandomArray2D(int length, int width)
    {
        var random = new System.Random();
        var array = new int[length, width];
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < width; j++)
            {
                array[i,j] = random.Next(0, 100);
            }
        }

        return array;
    }

    [Command]
    public static System.Tuple<int, int> Swap(int a, int b) => new(b, a);
}


public static class TestClass
{
    [Command("Does testing things from another class")]
    public static string TestCommand() => "TestCommand";
}