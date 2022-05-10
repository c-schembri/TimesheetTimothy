using System.CommandLine;

namespace TimesheetTimothy
{
    public static class CommitArguments
    {
        private static Argument<string>? username;

        public static Argument<string> Username
        {
            get
            {
                username ??= new("Username", "Your login email address, e.g. user@email.com");
                return username;
            }
        }
    }
}
