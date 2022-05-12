using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WebDriverManager.DriverConfigs.Impl;

namespace TimesheetTimothy.PageInteraction;

internal static class DriverManager
{
    internal static IWebDriver Driver
    {
        get
        {
            if (_driver is null)
            {
                new WebDriverManager.DriverManager().SetUpDriver(new ChromeConfig());
                _driver = new ChromeDriver();
                _driver.Navigate().GoToUrl("https://timesheets.dialoggroup.biz/?company=accesstesting");
            }

            return _driver;
        }
    }
    private static IWebDriver? _driver;

    internal static void TearDownDriver()
    {
        if (_driver is null)
            return;
        
        try
        { 
            Driver.Quit();
        }
        catch (DriverServiceNotFoundException)
        {
            // we don't care if the chromedriver.exe has not been set up yet before the driver needs to quit
        }
    }
}
