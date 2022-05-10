﻿using OpenQA.Selenium.Chrome;
using System.CommandLine;

namespace TimesheetTimothy;

public static class Program
{
    public static IWebDriver Driver { get; } = new ChromeDriver();

    /// The main entry point for the program.
    public static int Main(string[] args)
    {
        var cmd = new RootCommand
        {
            TimothysCommands.Commit       
        };

        var result = cmd.Invoke(args);
        Driver.Quit();
        return result;
    }
}