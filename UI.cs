using System.Text.RegularExpressions;
using OpenQA.Selenium;
using static TimesheetTimothy.SeleniumHelpers;
using static SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace TimesheetTimothy;

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

    public static void SetDay(int day)
    {
        Click(By.Name("cmbDay"));

        var selector = By.TagName("option");
        var options = WaitUntil(ElementIsVisible(selector), selector);
        Click(options.ElementAt(day));                
    }

    public static void SetHours(string hours)
    {
        SendKeys(By.Name("txtHours"), hours);
    }

    public static void SaveEntry()
    {
        Click(By.Name("Submit"));
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