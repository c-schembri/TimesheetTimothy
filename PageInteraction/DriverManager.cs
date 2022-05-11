using WebDriverManager;
using WebDriverManager.DriverConfigs;
using WebDriverManager.DriverConfigs.Impl;
using static TimesheetTimothy.Program;

namespace TimesheetTimothy.PageInteraction;

static internal class DriverManager
{
    public static void SetUpDriver(IDriverConfig driverConfig)
    {
        new WebDriverManager.DriverManager().SetUpDriver(driverConfig);
        Driver.Navigate().GoToUrl("https://timesheets.dialoggroup.biz/?company=accesstesting");
    }

    public static void TearDownDriver()
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
