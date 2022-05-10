using System.CommandLine;
using static TimesheetTimothy.TimesheetPage;

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
                commitCommand.SetHandler((string e) => CommitTimesheet(e, jobsFileName), CommitArguments.Username);
                return commitCommand;
            }
        }
    }
}
