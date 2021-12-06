using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using static TimesheetTimothy.UI;

namespace TimesheetTimothy;

public static class Program
{
    public static IWebDriver Driver { get; } = new ChromeDriver(ChromeDriverService.CreateDefaultService());
    
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
                    Console.Write("*");
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
                return Result(ExitCode.LoginDetailsIncorrectError);
        } 
        finally 
        {
            Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            password.Dispose();
        }

        int day   = 0;
        int hours = 0;
        
        string[] jobs = File.ReadAllLines(jobsFileName);
        foreach (string job in jobs)
        {
            string[] jobParts = job.Split(' ');
            for (int i = 0; i < jobParts.Length; i += 2)
            {
                SetJobCode(jobParts[i]);
                SetDay(day);
                SetHours(jobParts[i + 1]);
                SaveEntry();
                
                hours += int.Parse(jobParts[i + 1]);
            }

            day++;
        }
        
        Trace.Assert(hours == GetEnteredHours());
        
#if RELEASE
        CommitTimesheet();
#endif

        stopwatch.Stop();
        return Result(ExitCode.TimesheetCommitted, stopwatch.ElapsedMilliseconds.ToString());
    }

    private static int Result(ExitCode code, string arg = "")
    {
        Driver.Quit();

        switch (code)
        {
            case ExitCode.TimesheetCommitted:
                Debug.Assert(!string.IsNullOrWhiteSpace(arg));
                Console.Write($"Timothy committed your timesheet in {arg} milliseconds.");
                break;
            
            case ExitCode.InvalidArgumentCount:
                Console.Write("Timothy requires two arguments, command and target user (e.g., 'commit' and 'user@email.com').");
                break;
            
            case ExitCode.InvalidArgumentSpecified:
                Debug.Assert(!string.IsNullOrWhiteSpace(arg));
                Console.Write($"'{arg}' is an unrecognised command.");
                break;
            
            case ExitCode.LoginDetailsIncorrectError:
                Console.Write("Entered username or password is incorrect.");
                break;
            
            case ExitCode.JobsFileNotFound:
                Console.Write("Could not find 'jobs.txt' file.");
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(code));
        }
        
        Console.WriteLine(".. Exiting.");
        return (int)code;
    }
}