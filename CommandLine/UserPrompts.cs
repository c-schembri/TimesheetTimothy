using System.Security;

namespace TimesheetTimothy
{
    internal static class UserPrompts
    {
        public static SecureString GetPassword(string username)
        {
            SecureString password = new();
            Console.WriteLine($"Please enter the password for {username}");

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
}
