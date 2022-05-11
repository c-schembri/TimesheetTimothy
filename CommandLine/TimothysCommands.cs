using System.CommandLine;
using static TimesheetTimothy.PageInteraction.TimesheetPage;
using Command = System.CommandLine.Command;

namespace TimesheetTimothy.CommandLine;

internal static class TimothysCommands
{
    const string jobsFileName = "jobs.json";
    private static Command? commitCommand;

    public static Command Commit
    {
        get
        {
            if (commitCommand == null)
            {
                commitCommand = new("commit", "Commit the timesheet");
                commitCommand.AddArgument(CommitArguments.Username);
                commitCommand.SetHandler((string user) => DoYourTimesheet(user, jobsFileName), CommitArguments.Username);
            }

            return commitCommand;
        }
    }
}
