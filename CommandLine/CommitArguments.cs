using System.CommandLine;

namespace TimesheetTimothy.CommandLine;

public static class CommitArguments
{
    public static Argument<string> Username { get; } = new("Username", "Your login email address, e.g. user@email.com");
}
