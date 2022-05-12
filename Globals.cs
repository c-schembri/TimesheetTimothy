namespace TimesheetTimothy;

internal static class GlobalConstants
{
    internal const string EmptyString = "";
}

internal static class GlobalGuards
{
    internal static void ThrowArgumentExceptionIfLengthZero(Array array, string paramName)
    {
        if (array.Length == 0)
            throw new ArgumentException("Length cannot be 0", paramName);
    }
}
