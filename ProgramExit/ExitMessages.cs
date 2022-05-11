namespace TimesheetTimothy.ProgramExit;

internal static class ExitMessages
{
    internal static string GetResultMsg(ExitCode code, string arg = EmptyString)
    {
        return Messages[code](arg) + "... Exiting";
    }

    private static readonly Dictionary<ExitCode, Func<string, string>> Messages = new()
    {
#if RELEASE
        { ExitCode.TimesheetCommitted, (arg) => $"Timothy committed your timesheet in {arg} milliseconds" },
#else
        { ExitCode.TimesheetCommitted, (arg) => $"Timothy would have committed your timesheet in {arg} milliseconds (debug build)" },
#endif // RELEASE
        { ExitCode.InvalidArgumentCount, (arg) => "Timothy requires two arguments, command and target user (e.g., 'commit' and 'user@email.com')" },
        { ExitCode.InvalidArgumentSpecified, (arg) => $"'{arg}' is an unrecognised command" },
        { ExitCode.LoginDetailsIncorrect, (arg) => "Entered username or password is incorrect" },
        { ExitCode.JobsFileNotFound, (arg) => "Could not find 'jobs.json' file" },
        { ExitCode.DayMissingEntries, (arg) => $"Day '{arg}' was defined, but no entries were defined for this day" },
        { ExitCode.EntryMissingJobCode, (arg) => $"Day '{arg}' has an entry missing a job code" },
        { ExitCode.EntryMissingHours, (arg) => $"Day '{arg}' has an entry missing hours" }
    };
}