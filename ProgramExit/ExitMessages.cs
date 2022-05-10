using System.Diagnostics;

namespace TimesheetTimothy
{
    internal static class ExitMessages
    {
        internal static string Result(ExitCode code, string arg = EmptyString)
        {
            string message;

            switch (code)
            {
                case ExitCode.TimesheetCommitted:
                    Debug.Assert(!string.IsNullOrWhiteSpace(arg));
#if RELEASE
                message = $"Timothy committed your timesheet in {arg} milliseconds";
#else
                    message = $"Timothy would have committed your timesheet in {arg} milliseconds (debug build)";
#endif // RELEASE
                    break;

                case ExitCode.InvalidArgumentCount:
                    message = "Timothy requires two arguments, command and target user (e.g., 'commit' and 'user@email.com')";
                    break;

                case ExitCode.InvalidArgumentSpecified:
                    Debug.Assert(!string.IsNullOrWhiteSpace(arg));
                    message = $"'{arg}' is an unrecognised command";
                    break;

                case ExitCode.LoginDetailsIncorrect:
                    message = "Entered username or password is incorrect";
                    break;

                case ExitCode.JobsFileNotFound:
                    message = "Could not find 'jobs.txt' file";
                    break;

                case ExitCode.DayMissingEntries:
                    message = $"Day '{arg}' was defined, but no entries were defined for this day";
                    break;

                case ExitCode.EntryMissingJobCode:
                    message = $"Day '{arg}' has an entry missing a job code";
                    break;

                case ExitCode.EntryMissingHours:
                    message = $"Day '{arg}' has an entry missing hours";
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(code));
            }

            return $"{message}... Exiting.";
        }
    }
}
