using System.CommandLine;
using static TimesheetTimothy.TimesheetPage;
using Command = System.CommandLine.Command;

namespace TimesheetTimothy
{
    internal static class TimothysCommands
    {
        const string jobsFileName = "jobs.json";

        public static Command Commit
        {
            get
            {
                Command commitCommand = new("commit", "Commit the timesheet");
                commitCommand.AddArgument(CommitArguments.Username);
                commitCommand.SetHandler((string e) => DoYourTimesheet(e, jobsFileName), CommitArguments.Username);

                return commitCommand;
            }
        }
    }
}
