namespace TimesheetTimothy.ProgramExit;

internal enum ExitCode
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