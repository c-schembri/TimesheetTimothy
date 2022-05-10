using System.CommandLine;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Text.Json;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using static TimesheetTimothy.UI;
using static TimesheetTimothy.Program;

namespace TimesheetTimothy
{
    internal static class TimothysCommands
    {
        private record Week(Day? Monday, Day? Tuesday, Day? Wednesday, Day? Thursday, Day? Friday);
        private record Day(Entry[]? Entries);
        private record Entry(string? JobCode, string? Hours, string? WorkType, string? Comments);
        const string jobsFileName = "jobs.json";

        public static System.CommandLine.Command Commit
        {
            get
            {
                System.CommandLine.Command commitCommand = new("commit", "Commit the timesheet");
                commitCommand.AddArgument(CommitArguments.Username);
                commitCommand.SetHandler((string e) => CommitTimesheet(e), CommitArguments.Username);
                return commitCommand;
            }
        }

        private static int CommitTimesheet(string username)
        {
            SecureString password = GetPassword(username);
            Stopwatch stopwatch = Stopwatch.StartNew();
            SetUpChromeDriver();

            var securePasswordPtr = GetSecurePwd(username, password);

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

#if RELEASE
        CommitTimesheet();
#endif // RELEASE

            stopwatch.Stop();
            Console.WriteLine($"{Result(ExitCode.TimesheetCommitted, stopwatch.ElapsedMilliseconds.ToString())}");
            return 0;
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

        private static IntPtr GetSecurePwd(string username, SecureString password)
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

            return securePasswordPtr;
        }

        private static void SetUpChromeDriver()
        {
            new DriverManager().SetUpDriver(new ChromeConfig());

            Driver.Navigate().GoToUrl("https://timesheets.dialoggroup.biz/?company=accesstesting");
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

        private static string Result(ExitCode code, string arg = EmptyString)
        {
            Driver.Quit();
            string message;

            switch (code)
            {
                case ExitCode.TimesheetCommitted:
                    Debug.Assert(!string.IsNullOrWhiteSpace(arg));
#if RELEASE
                message = $"Timothy committed your timesheet in {arg} milliseconds";
#else
                    message = $"Timothy would have committed your timesheet in {arg} milliseconds (debug build)";
#endif // RELEASE
                    break;

                case ExitCode.InvalidArgumentCount:
                    message = "Timothy requires two arguments, command and target user (e.g., 'commit' and 'user@email.com')";
                    break;

                case ExitCode.InvalidArgumentSpecified:
                    Debug.Assert(!string.IsNullOrWhiteSpace(arg));
                    message = $"'{arg}' is an unrecognised command";
                    break;

                case ExitCode.LoginDetailsIncorrect:
                    message = "Entered username or password is incorrect";
                    break;

                case ExitCode.JobsFileNotFound:
                    message = "Could not find 'jobs.txt' file";
                    break;

                case ExitCode.DayMissingEntries:
                    message = $"Day '{arg}' was defined, but no entries were defined for this day";
                    break;

                case ExitCode.EntryMissingJobCode:
                    message = $"Day '{arg}' has an entry missing a job code";
                    break;

                case ExitCode.EntryMissingHours:
                    message = $"Day '{arg}' has an entry missing hours";
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(code));
            }

            return $"{message}... Exiting.";
        }
    }
}
