using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Text.Json;
using static TimesheetTimothy.ExitMessages;
using static TimesheetTimothy.SeleniumHelpers;
using static TimesheetTimothy.UI;
using static TimesheetTimothy.UserPrompts;

namespace TimesheetTimothy
{
    internal static class TimesheetPage
    {
        internal static int DoYourTimesheet(string username, string jobsFileName)
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
            Console.WriteLine($"{GetResultMsg(ExitCode.TimesheetCommitted, stopwatch.ElapsedMilliseconds.ToString())}");
            return 0;
        }

        private static void OpenTimesheetPage(string username, SecureString password)
        {
            var securePasswordPtr = IntPtr.Zero;
            try
            {
                securePasswordPtr = Marshal.SecureStringToGlobalAllocUnicode(password);
                if (!OpenTimesheet(username, Marshal.PtrToStringUni(securePasswordPtr) ?? string.Empty))
                    throw new Exception($"{GetResultMsg(ExitCode.LoginDetailsIncorrect)}");
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(securePasswordPtr);
                password.Dispose();
            }
        }

        private static void SetTimesheetEntries(string jobsFileName)
        {
            int totalHours = 0;
            var week = JsonSerializer.Deserialize<Week>(File.ReadAllText(jobsFileName)) ?? throw new NullReferenceException();
            ProcessWeek(week, ref totalHours);
            Trace.Assert(totalHours == GetEnteredHours());
        }

        private static void ProcessWeek(Week week, ref int totalHours)
        {
            foreach (var dayProp in typeof(Week).GetProperties())
            {
                // Not all days need to be defined by the user.
                if (dayProp.GetValue(week) is not Day day)
                    continue;

                // However, if a day _is_ defined, then the program expects some entries in that day.
                if (day.Entries is null)
                    throw new Exception($"{GetResultMsg(ExitCode.DayMissingEntries, dayProp.Name)}");

                foreach (var entry in day.Entries)
                {
                    SetJobEntry(dayProp, entry);
                    totalHours += int.Parse(entry.Hours!);
                }
            }
        }

        private static void SetJobEntry(System.Reflection.PropertyInfo dayProp, Entry entry)
        {
            ArgumentNullException.ThrowIfNull(entry.JobCode, "JobCode");
            ArgumentNullException.ThrowIfNull(entry.Hours, "Hours");

            SetJobCode(entry.JobCode);
            SetDay(dayProp.Name);
            SetHours(entry.Hours);
            SetWorkType(entry.WorkType);
            SetComments(entry.Comments);
            SaveEntry();
        }

    }
}
