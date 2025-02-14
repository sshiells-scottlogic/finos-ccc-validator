namespace Finos.CCC.Validator;

public static class ConsoleWriter
{
    private const string ColorReset = "\x1b[0m";
    private const string Green = "\x1b[32m";
    private const string Red = "\x1b[31m";

    public static void WriteError(string message)
    {
        Console.WriteLine($"{Red}{message}{ColorReset}");
    }
    public static void WriteSuccess(string message)
    {
        Console.WriteLine($"{Green}{message}{ColorReset}");
    }
}
