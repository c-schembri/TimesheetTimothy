namespace TimesheetTimothy;

public static class GlobalConstants
{
    public const string EmptyString = "";
}

public static class GlobalGuards
{
    public static void EnsureLengthNotZero(Array array, string paramName)
    {
        if (array.Length == 0)
        {
            throw new ArgumentException("Length cannot be 0", paramName);
        }
    }
}