using System.CommandLine;

namespace TimesheetTimothy.CommandLine;

internal static class CommitArguments
{
    internal static Argument<string> Username { get; } = new("Username", "Your login email address, e.g. user@email.com");
}
