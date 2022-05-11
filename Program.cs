using OpenQA.Selenium.Chrome;
using System.CommandLine;
using TimesheetTimothy.CommandLine;

namespace TimesheetTimothy;

public static class Program
{
    public static IWebDriver Driver { get; } = new ChromeDriver();

    /// The main entry point for the program.
    public static int Main(string[] args)
    {
        var cmd = new RootCommand
        {
            TimothysCommands.Commit       
        };

        try
        {
            var result = cmd.Invoke(args);
            return result;
        }
        finally
        {
            PageInteraction.DriverManager.TearDownDriver();
        }
    }
}