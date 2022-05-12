namespace TimesheetTimothy.DataStructures;

internal record Entry(string? JobCode, string? Hours, string? WorkType, string? Comments);
internal record Day(Entry[]? Entries);
internal record Week(Day? Monday, Day? Tuesday, Day? Wednesday, Day? Thursday, Day? Friday);