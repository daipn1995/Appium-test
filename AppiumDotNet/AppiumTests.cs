using NUnit.Framework;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium;
using System.Diagnostics;
using OpenQA.Selenium.Appium.Enums;
using OpenQA.Selenium.Appium.Service;

namespace AppiumDotNet;

public class AppiumTests
{
    private AppiumLocalService _appiumLocalService;

    [SetUp]
    public void SetUp()
    {
        _appiumLocalService = new AppiumServiceBuilder().UsingAnyFreePort().Build();
        _appiumLocalService.Start();
    }

    [Test]
    public void Test1()
    {
        // start the emulator
        Process process = new Process();

        if (!IsEmulatorTurnedOn("emulator-5554"))
        {
            string emulatorPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "AppData\\Local\\Android\\Sdk\\emulator\\emulator.exe");
            string avdName = "Pixel_6_Pro_API_33"; // name of AVD to start

            process.StartInfo.FileName = emulatorPath;
            process.StartInfo.Arguments = "-avd " + avdName;
            process.Start();
            Thread.Sleep(15000);
        }
        Console.WriteLine("===== [STARTED] the emulator");

        // Connect to the emulator
        string appPath = $"{TestContext.CurrentContext.TestDirectory}\\App\\App.apk";

        AppiumOptions options = new AppiumOptions();
        options.AddAdditionalCapability(MobileCapabilityType.DeviceName, "sdk_gphone64_x86_64");
        options.AddAdditionalCapability(MobileCapabilityType.Udid, "emulator-5554");
        options.AddAdditionalCapability(MobileCapabilityType.PlatformName, "Android");
        options.AddAdditionalCapability(MobileCapabilityType.PlatformVersion, "13.0");
        options.AddAdditionalCapability(MobileCapabilityType.App, appPath);
        options.AddAdditionalCapability("appPackage", "everything.appium");
        options.AddAdditionalCapability("appActivity", "everything.appium.MainActivity");
        AndroidDriver<AndroidElement> driver = new AndroidDriver<AndroidElement>(_appiumLocalService, options);
        //driver.Context = "NATIVE_APP";
        //driver.Context = "WEBVIEW_chrome";

        // Click the button with id "myButton"
        AndroidElement button = driver.FindElementByXPath("//*[@text='Introduction to Appium']/following-sibling::android.widget.Button[@text='UNLOCK']");
        button.Click();
        Thread.Sleep(3000);

        // Close the driver
        driver.Quit();
        // Close process to start emulator
        try
        {
            process.Kill();
        } catch (Exception) { }
        
    }

    [Test]
    public void Test2()
    {
        // Connect to the emulator
        string appPath = $"{TestContext.CurrentContext.TestDirectory}\\App\\App.apk";

        AppiumOptions options = new AppiumOptions();
        options.AddAdditionalCapability(MobileCapabilityType.DeviceName, "sdk_gphone64_x86_64");
        options.AddAdditionalCapability(MobileCapabilityType.Udid, "emulator-5554");
        options.AddAdditionalCapability(MobileCapabilityType.PlatformName, "Android");
        options.AddAdditionalCapability(MobileCapabilityType.PlatformVersion, "13.0");
        options.AddAdditionalCapability(MobileCapabilityType.App, appPath);
        AndroidDriver<AndroidElement> driver = new AndroidDriver<AndroidElement>(new Uri("http://127.0.0.1:4723/wd/hub"), options);
        //driver.Context = "NATIVE_APP";
        //driver.Context = "WEBVIEW_chrome";

        // Click the button with id "myButton"
        AndroidElement button = driver.FindElementByXPath("//*[@text='Introduction to Appium']/following-sibling::android.widget.Button[@text='UNLOCK']");
        button.Click();
        Thread.Sleep(3000);

        // Close the driver
        driver.Quit();

    }


    [TearDown]
    public void TearDown()
    {
        //_appiumLocalService.Dispose();
    }

    public bool IsEmulatorTurnedOn(string emulatorName)
    {
        Process process = new Process();
        string adbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "AppData\\Local\\Android\\Sdk\\platform-tools\\adb.exe");
        process.StartInfo.FileName = adbPath;
        process.StartInfo.Arguments = "devices";
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.Start();

        string output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        process.Kill();

        if (output.Contains(emulatorName))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}