using OpenQA.Selenium;

namespace FrontWally.WebApp.TestUI
{
    internal class SelectElement
    {
        private IWebElement webElement;

        public SelectElement(IWebElement webElement)
        {
            this.webElement = webElement;
        }
    }
}