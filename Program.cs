using System.CommandLine;
using TimesheetTimothy.CommandLine;
using TimesheetTimothy.PageInteraction;

namespace TimesheetTimothy;

public static class Program
{
    /// The main entry point for the program.
    public static int Main(string[] args)
    {
        RootCommand cmd = new()
        {
            Commands.Commit
        };

        int result = cmd.Invoke(args);
        
        // Only attempt driver tear-down if a valid command was passed to the program.
        if (result == 0)
            DriverManager.TearDownDriver();
        
        return result;
    }
}