using System.Text.RegularExpressions;
using static TimesheetTimothy.PageInteraction.SeleniumHelpers;
using static SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace TimesheetTimothy.PageInteraction;

public static class UI
{
    public static bool OpenTimesheet(string username, string password)
    {
        SendKeys(By.Id("userNameInput"), username);
        SendKeys(By.Id("passwordInput"), password);
        Click(By.Id("submitButton"));

        if (ElementVisible(By.Id("errorText")))
            return false;

        WaitUntil(ElementIsVisible(By.ClassName("mainContent")));
        Click(By.Name("btnLoadTS"));
        return true;
    }

    public static void SetJobCode(string jobCode)
    {
        SendKeys(By.Name("txtJobNum"), jobCode);
    }

    public static void SetDay(string day)
    {
        int dayNumber = day switch
        {
            "Monday" => 0,
            "Tuesday" => 1,
            "Wednesday" => 2,
            "Thursday" => 3,
            "Friday" => 4,
            _ => throw new ArgumentOutOfRangeException(nameof(day), "Unrecognised day")
        };

        Click(By.Name("cmbDay"));

        var selector = By.TagName("option");
        var options = WaitUntil(ElementIsVisible(selector), selector);
        Click(options.ElementAt(dayNumber));
    }

    public static void SetHours(string hours)
    {
        SendKeys(By.Name("txtHours"), hours);
    }

    public static void SetWorkType(string? workType)
    {
        SendKeys(By.Name("txtWorkType"), workType ?? string.Empty);
    }

    internal static void SetComments(string? comments)
    {
        SendKeys(By.Name("txaComments"), comments ?? string.Empty);
    }

    public static void SaveEntry()
    {
        Click(By.Name("Submit"));

        // Sometimes a warning page appears after saving the entry, e.g., when using a new job code.
        var ignoreWarningButtonSelector = By.Name("btnContinue");
        if (ElementVisible(ignoreWarningButtonSelector))
        {
            Console.WriteLine("Ignoring warning page");
            Click(ignoreWarningButtonSelector);
        }
    }

    public static int GetEnteredHours()
    {
        string enteredHours = FindElement(
            By.Name("ListEntries"),
            By.CssSelector("[bgcolor='#CCCCCC'] [align='right']")).Text;

        return int.Parse(Regex.Replace(enteredHours, "[^0-9]", string.Empty));
    }

#if RELEASE
    public static void CommitTimesheet()
    {
        Click(By.CssSelector("[href='TSCommit.asp']"));
        Click(WaitUntil(ElementIsVisible(By.Name("btnContinue"))));
    }
#endif // RELEASE
}