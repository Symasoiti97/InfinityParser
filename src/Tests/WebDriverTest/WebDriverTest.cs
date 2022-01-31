using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace WebDriverTest
{
    public class WebDriverTest
    {
        [Test]
        public void GetBodyBySelenium_Test()
        {
            var outPutDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            using IWebDriver driver = new ChromeDriver(outPutDirectory);
            driver.Navigate().GoToUrl("https://makler.md/ru/transnistria/real-estate/real-estate-for-sale/apartments-for-sale?list&city[]=1112&currency_id=5&list=detail");

            Assert.IsNotEmpty(driver.PageSource);
            Assert.Greater(driver.PageSource.Length, 0);
        }
    }
}