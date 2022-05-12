using System.Security;

namespace TimesheetTimothy.CommandLine;

internal static class UserPrompts
{
    internal static SecureString GetPassword(string username)
    {
        Console.WriteLine($"Please enter the password for {username}");

        SecureString password = new();
        ConsoleKeyInfo key;
        
        do
        {
            key = Console.ReadKey(true);
            password.AppendChar(key.KeyChar);
        } while (key.Key != ConsoleKey.Enter);

        Console.WriteLine();
        return password;
    }
}
