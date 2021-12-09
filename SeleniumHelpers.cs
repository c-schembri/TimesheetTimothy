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
        return WaitUntilInternal(cond);
    }

    public static IEnumerable<IWebElement> WaitUntil(Func<IWebDriver, IWebElement> cond, By selector)
    {
        WaitUntilInternal(cond);
        return Driver.FindElements(selector);
    }

    public static IWebElement FindElement(params By[] selectors)
    {
        if (selectors.Length == 0)
            throw new ArgumentException("Length cannot be 0", nameof(selectors));
        
        IWebElement latestFind = Driver.FindElement(selectors[0]);
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
            WaitUntilInternal(ElementIsVisible(selector));
            return true;
        }
        catch (WebDriverTimeoutException)
        {
            return false;
        }
    }

    private static IWebElement WaitUntilInternal(Func<IWebDriver, IWebElement> cond)
    {
        return new WebDriverWait(Driver, TimeSpan.FromSeconds(1)).Until(cond);
    }
}