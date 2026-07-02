namespace BookStore.App.Menu;


internal static class Print
{
    public static void Success(string message) => WriteColored(message, ConsoleColor.Green);

    public static void Error(string message) => WriteColored($"✗ {message}", ConsoleColor.Red);

    public static void Info(string message) => WriteColored(message, ConsoleColor.Cyan);

    public static void Header(string title)
    {
        Console.WriteLine();
        WriteColored($"── {title} ──", ConsoleColor.Yellow);
    }

    public static void Warn(string message) => WriteColored($"⚠ {message}", ConsoleColor.DarkYellow);

    private static void WriteColored(string message, ConsoleColor color)
    {
        var original = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ForegroundColor = original;
    }
}
