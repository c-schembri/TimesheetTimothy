using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WebDriverManager.DriverConfigs.Impl;

namespace TimesheetTimothy.PageInteraction;

internal static class DriverManager
{
    internal static IWebDriver Driver { get; }

    static DriverManager()
    {
        new WebDriverManager.DriverManager().SetUpDriver(new ChromeConfig());
        Driver = new ChromeDriver();
        Driver.Navigate().GoToUrl("https://timesheets.dialoggroup.biz/?company=accesstesting");
    }
    
    internal static void TearDownDriver()
    {
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
