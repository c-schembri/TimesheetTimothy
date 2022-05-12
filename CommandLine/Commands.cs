using System.CommandLine;
using static TimesheetTimothy.PageInteraction.TimesheetPage;

namespace TimesheetTimothy.CommandLine;

internal static class Commands
{
    internal static Command Commit { get; }

    static Commands()
    {
        Commit = new Command("commit", "Commit the timesheet");
        Commit.AddArgument(CommitArguments.Username);
        Commit.SetHandler((string user) => DoYourTimesheet(user, "jobs.json"), CommitArguments.Username);
    }
}
