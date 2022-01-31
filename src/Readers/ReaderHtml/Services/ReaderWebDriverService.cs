using System;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace ReaderHtml.Services
{
    public class ReaderWebDriverService : IReaderHtmlService
    {
        private readonly IWebDriver _webDriver;

        public ReaderWebDriverService(IWebDriver webDriver)
        {
            _webDriver = webDriver ?? throw new ArgumentNullException(nameof(webDriver));
        }

        public Task<string> GetAsync(string address)
        {
            _webDriver.Navigate().GoToUrl(address);
            return Task.FromResult(_webDriver.PageSource);
        }
    }
}