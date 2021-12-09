using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Text.Json;
using OpenQA.Selenium.Chrome;
using static TimesheetTimothy.UI;

namespace TimesheetTimothy;

public static class Program
{
    public static IWebDriver Driver { get; } = new ChromeDriver(ChromeDriverService.CreateDefaultService());
    
    private enum ExitCode
    {
        TimesheetCommitted,
        InvalidArgumentCount,
        InvalidArgumentSpecified,
        LoginDetailsIncorrect,
        JobsFileNotFound,
        DayMissingEntries,
        EntryMissingJobCode,
        EntryMissingHours
    }
    private record Week(Day? Monday, Day? Tuesday, Day? Wednesday, Day? Thursday, Day? Friday);
    private record Day(Entry[]? Entries);
    private record Entry(string? JobCode, string? Hours, string? WorkType);

    /// The main entry point for the program.
    public static int Main(string[] args)
    {
        if (args.Length != 2)
            return Result(ExitCode.InvalidArgumentCount);
        
        const string jobsFileName = "jobs.txt";
        if (!File.Exists(jobsFileName))
            return Result(ExitCode.JobsFileNotFound);

        string username;
        SecureString password = new();

        switch (args[0].ToLower())
        {
            case "commit":
                username = args[1];
                Console.WriteLine($"Please enter the password for {username}");
                
                ConsoleKeyInfo key;
                do 
                {
                    key = Console.ReadKey(true);
                    password.AppendChar(key.KeyChar);
                } while (key.Key != ConsoleKey.Enter);
                
                Console.WriteLine();
                break;
            
            default:
                return Result(ExitCode.InvalidArgumentSpecified, args[0]);
        }
        
        Stopwatch stopwatch = new();
        stopwatch.Start();
        
        Driver.Navigate().GoToUrl("https://timesheets.dialoggroup.biz/?company=accesstesting");

        var valuePtr = IntPtr.Zero;
        try 
        {
            valuePtr = Marshal.SecureStringToGlobalAllocUnicode(password);
            if (!OpenTimesheet(username, Marshal.PtrToStringUni(valuePtr) ?? string.Empty))
                return Result(ExitCode.LoginDetailsIncorrect);
        } 
        finally 
        {
            Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            password.Dispose();
        }

        int totalHours = 0;
        
        var week = JsonSerializer.Deserialize<Week>(File.ReadAllText(jobsFileName));
        foreach (var dayProp in typeof(Week).GetProperties())
        {
            // Not all days need to be defined by the user.
            if (dayProp.GetValue(week) is not Day day)
                continue;

            // However, if a day _is_ defined, then the program expects some entries in that day.
            if (day.Entries is null)
                return Result(ExitCode.DayMissingEntries, dayProp.Name);
            
            foreach ((string? jobCode, string? hours, string? workType) in day.Entries)
            {
                if (string.IsNullOrWhiteSpace(jobCode))
                    return Result(ExitCode.EntryMissingJobCode, dayProp.Name);
                
                if (string.IsNullOrWhiteSpace(hours))
                    return Result(ExitCode.EntryMissingHours, dayProp.Name);
                
                SetJobCode(jobCode);
                SetDay(dayProp.Name);
                SetHours(hours);
                SetWorkType(workType);
                SaveEntry();

                totalHours += int.Parse(hours!);
            }
        }    
        
        Trace.Assert(totalHours == GetEnteredHours());
        
#if RELEASE
        CommitTimesheet();
#endif // RELEASE

        stopwatch.Stop();
        return Result(ExitCode.TimesheetCommitted, stopwatch.ElapsedMilliseconds.ToString());
    }

    private static int Result(ExitCode code, string arg = "")
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
        
        Console.WriteLine($"{message}... Exiting.");
        return (int)code;
    }
}