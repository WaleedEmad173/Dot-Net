namespace BookStore.App.Menu;

internal static class ConsoleInput
{
    private static string? ReadLineChecked()
    {
        var line = Console.ReadLine();
        if (line is null)
        {
            Console.WriteLine();
            Print.Warn("Input stream closed - exiting.");
            Environment.Exit(0);
        }
        return line;
    }

    public static string ReadNonEmptyString(string prompt)
    {
        while (true)
        {
            Console.Write($"{prompt}: ");
            var input = ReadLineChecked()?.Trim();
            if (!string.IsNullOrWhiteSpace(input))
                return input;

            Print.Error("This field cannot be empty. Please try again.");
        }
    }

    public static string? ReadOptionalString(string prompt)
    {
        Console.Write($"{prompt} (optional, press Enter to skip): ");
        var input = ReadLineChecked()?.Trim();
        return string.IsNullOrWhiteSpace(input) ? null : input;
    }

    public static decimal ReadDecimal(string prompt)
    {
        while (true)
        {
            Console.Write($"{prompt}: ");
            var input = ReadLineChecked();
            if (decimal.TryParse(input, out var value) && value >= 0)
                return value;

            Print.Error("Please enter a valid non-negative number (e.g. 12.99).");
        }
    }

    public static int ReadInt(string prompt)
    {
        while (true)
        {
            Console.Write($"{prompt}: ");
            var input = ReadLineChecked();
            if (int.TryParse(input, out var value) && value >= 0)
                return value;

            Print.Error("Please enter a valid whole number (0 or greater).");
        }
    }

    public static double ReadDouble(string prompt)
    {
        while (true)
        {
            Console.Write($"{prompt}: ");
            var input = ReadLineChecked();
            if (double.TryParse(input, out var value) && value >= 0)
                return value;

            Print.Error("Please enter a valid non-negative number.");
        }
    }

    public static TimeSpan ReadDuration(string prompt)
    {
        while (true)
        {
            Console.Write($"{prompt} (format h:mm:ss, e.g. 5:30:00): ");
            var input = ReadLineChecked();
            if (TimeSpan.TryParse(input, out var value) && value >= TimeSpan.Zero)
                return value;

            Print.Error("Please enter a valid duration, e.g. 5:30:00 for 5 hours 30 minutes.");
        }
    }

    public static int ReadChoiceIndex(string prompt, IReadOnlyList<string> options)
    {
        while (true)
        {
            Console.WriteLine(prompt);
            for (int i = 0; i < options.Count; i++)
                Console.WriteLine($"  {i + 1}. {options[i]}");
            Console.Write("> ");

            var input = ReadLineChecked();
            if (int.TryParse(input, out var choice) && choice >= 1 && choice <= options.Count)
                return choice - 1;

            Print.Error($"Please enter a number between 1 and {options.Count}.");
        }
    }

    public static Guid ReadGuid(string prompt)
    {
        while (true)
        {
            Console.Write($"{prompt}: ");
            var input = ReadLineChecked();
            if (Guid.TryParse(input, out var value))
                return value;

            Print.Error("Please paste a valid Id as shown in the list (a GUID).");
        }
    }

    public static bool ReadYesNo(string prompt)
    {
        while (true)
        {
            Console.Write($"{prompt} (y/n): ");
            var input = ReadLineChecked()?.Trim().ToLowerInvariant();
            if (input == "y" || input == "yes") return true;
            if (input == "n" || input == "no") return false;

            Print.Error("Please answer y or n.");
        }
    }
}
