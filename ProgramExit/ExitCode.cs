namespace TimesheetTimothy;

public enum ExitCode
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