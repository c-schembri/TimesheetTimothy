using System.CommandLine;
using TimesheetTimothy.CommandLine;
using TimesheetTimothy.PageInteraction;

namespace TimesheetTimothy;

public static class Program
{
    /// The main entry point for the program.
    public static int Main(string[] args)
    {
        try
        {
            RootCommand cmd = new()
            {
                Commands.Commit
            };

            return cmd.Invoke(args);
        }
        finally
        {
            DriverManager.TearDownDriver();
        }
    }
}