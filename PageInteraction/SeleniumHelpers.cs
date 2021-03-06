using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using static TimesheetTimothy.PageInteraction.DriverManager;
using static SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace TimesheetTimothy.PageInteraction;

internal static class SeleniumHelpers
{
    internal static void SendKeys(By selector, string input)
    {
        Driver.FindElement(selector).SendKeys(input);
    }

    internal static void Click(By selector)
    {
        Driver.FindElement(selector).Click();
    }

    internal static void Click(IWebElement element)
    {
        element.Click();
    }

    internal static IWebElement WaitUntil(Func<IWebDriver, IWebElement> cond)
    {
        return InternalWaitUntil(cond);
    }

    internal static IEnumerable<IWebElement> WaitUntil(Func<IWebDriver, IWebElement> cond, By selector)
    {
        InternalWaitUntil(cond);
        return Driver.FindElements(selector);
    }

    internal static IWebElement FindElement(params By[] selectors)
    {
        ThrowArgumentExceptionIfLengthZero(selectors, nameof(selectors));

        var latestFind = Driver.FindElement(selectors[0]);
        for (int i = 1; i < selectors.Length; i++)
        {
            latestFind = latestFind.FindElement(selectors[i]);
        }

        return latestFind;
    }

    internal static bool ElementVisible(By selector)
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