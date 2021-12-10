using OpenQA.Selenium.Support.UI;
using static TimesheetTimothy.Program;
using static SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace TimesheetTimothy;

public static class SeleniumHelpers
{
    public static void SendKeys(By selector, string input)
    {
        Driver.FindElement(selector).SendKeys(input);
    }

    public static void Click(By selector)
    {
        Driver.FindElement(selector).Click();
    }

    public static void Click(IWebElement element)
    {
        element.Click();
    }

    public static IWebElement WaitUntil(Func<IWebDriver, IWebElement> cond)
    {
        return InternalWaitUntil(cond);
    }

    public static IEnumerable<IWebElement> WaitUntil(Func<IWebDriver, IWebElement> cond, By selector)
    {
        InternalWaitUntil(cond);
        return Driver.FindElements(selector);
    }

    public static IWebElement FindElement(params By[] selectors)
    {
        EnsureLengthNotZero(selectors, nameof(selectors));

        var latestFind = Driver.FindElement(selectors[0]);
        for (int i = 1; i < selectors.Length; i++)
        {
            latestFind = latestFind.FindElement(selectors[i]);
        }

        return latestFind;
    }
    
    public static bool ElementVisible(By selector)
    {
        try
        {
            InternalWaitUntil(ElementIsVisible(selector));
            return true;
        }
        catch (WebDriverTimeoutException)
        {
            return false;
        }
    }

    private static IWebElement InternalWaitUntil(Func<IWebDriver, IWebElement> cond)
    {
        return new WebDriverWait(Driver, TimeSpan.FromSeconds(1)).Until(cond);
    }
}