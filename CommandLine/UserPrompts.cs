using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

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
