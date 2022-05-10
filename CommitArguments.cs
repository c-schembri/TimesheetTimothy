using System.CommandLine;

namespace TimesheetTimothy
{
    public static class CommitArguments
    {
        private static Argument<string>? email;

        public static Argument<string> Email
        {
            get
            {
                email ??= new("Email", "Your login email address");
                return email;
            }
            private set { }
        }
    }
}
