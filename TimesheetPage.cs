using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using static TimesheetTimothy.SeleniumHelpers;
using static TimesheetTimothy.ExitMessages;
using static TimesheetTimothy.UI;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace TimesheetTimothy
{
    internal static class TimesheetPage
    {
        internal static int CommitTimesheet(string username, string jobsFileName)
        {
            SecureString password = GetPassword(username);
            Stopwatch stopwatch = Stopwatch.StartNew();

            SetUpChromeDriver();
            OpenTimesheetPage(username, password);
            SetTimesheetEntries(jobsFileName);

#if RELEASE
        CommitTimesheet();
#endif // RELEASE

            stopwatch.Stop();
            Console.WriteLine($"{Result(ExitCode.TimesheetCommitted, stopwatch.ElapsedMilliseconds.ToString())}");
            return 0;
        }

        private static void SetTimesheetEntries(string jobsFileName)
        {
            int totalHours = 0;
            var week = JsonSerializer.Deserialize<Week>(File.ReadAllText(jobsFileName));
            foreach (var dayProp in typeof(Week).GetProperties())
            {
                // Not all days need to be defined by the user.
                if (dayProp.GetValue(week) is not Day day)
                    continue;

                // However, if a day _is_ defined, then the program expects some entries in that day.
                if (day.Entries is null)
                    throw new Exception($"{Result(ExitCode.DayMissingEntries, dayProp.Name)}");

                foreach (var entry in day.Entries)
                {
                    SetJobEntry(dayProp, entry);
                    totalHours += int.Parse(entry.Hours);
                }
            }

            Trace.Assert(totalHours == GetEnteredHours());
        }

        private static void SetJobEntry(System.Reflection.PropertyInfo dayProp, Entry entry)
        {
            if (string.IsNullOrWhiteSpace(entry.JobCode))
                throw new Exception($"{Result(ExitCode.EntryMissingJobCode, dayProp.Name)}");

            if (string.IsNullOrWhiteSpace(entry.Hours))
                throw new Exception($"{Result(ExitCode.EntryMissingHours, dayProp.Name)}");

            SetJobCode(entry.JobCode);
            SetDay(dayProp.Name);
            SetHours(entry.Hours);
            SetWorkType(entry.WorkType);
            SetComments(entry.Comments);
            SaveEntry();
        }

        private static void OpenTimesheetPage(string username, SecureString password)
        {
            var securePasswordPtr = IntPtr.Zero;
            try
            {
                securePasswordPtr = Marshal.SecureStringToGlobalAllocUnicode(password);
                if (!OpenTimesheet(username, Marshal.PtrToStringUni(securePasswordPtr) ?? string.Empty))
                    throw new Exception($"{Result(ExitCode.LoginDetailsIncorrect)}");
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(securePasswordPtr);
                password.Dispose();
            }
        }

        private static SecureString GetPassword(string username)
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
