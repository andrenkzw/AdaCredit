using System.Globalization;

namespace AdaCredit;
public static class Utils
{
    // source: http://rajeshbailwal.blogspot.com/2012/03/password-in-c-console-application.html
    public static string ReadPassword()
    {
        string password = "";
        ConsoleKeyInfo info = Console.ReadKey(true);
        while (info.Key != ConsoleKey.Enter)
        {
            if (info.Key != ConsoleKey.Backspace)
            {
                Console.Write("*");
                password += info.KeyChar;
            }
            else if (info.Key == ConsoleKey.Backspace)
            {
                if (!string.IsNullOrEmpty(password))
                {
                    // remove one character from the list of password characters
                    password = password.Substring(0, password.Length - 1);
                    // get the location of the cursor
                    int pos = Console.CursorLeft;
                    // move the cursor to the left by one character
                    Console.SetCursorPosition(pos - 1, Console.CursorTop);
                    // replace it with space
                    Console.Write(" ");
                    // move the cursor to the left by one character again
                    Console.SetCursorPosition(pos - 1, Console.CursorTop);
                }
            }
            info = Console.ReadKey(true);
        }
        // add a new line because user pressed enter at the end of their password
        Console.WriteLine();
        return password;
    }

    public static IEnumerable<string> GetTransactionFiles(DirectoryInfo directoryInfo)
        => directoryInfo.GetFiles().Where(x => x.Extension == ".csv").Select(x => x.Name)
            .Select(x => x.Substring(0, x.Length-4)).Where(
                x => DateTime.TryParseExact(
                    x.Substring(x.LastIndexOf('-') + 1),
                    "yyyyMMdd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime y
                )
            );
    
    public static DateTime DateFromTransactionFile(string filename)
        => DateTime.ParseExact(
                filename.Substring(filename.LastIndexOf('-') + 1),
                "yyyyMMdd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None
            );
}
