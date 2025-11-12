using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontWally.WebApp.TestUI
{
    [TestClass]
    public class AuthControllerTest
    {
        private IWebDriver _driver;
        private readonly string _urlBase = "https://localhost:44327";


        [TestInitialize]
        public void Setup()
        {
            _driver = new ChromeDriver();
            _driver.Manage().Window.Maximize();
        }

        [TestMethod]
        public void Login_CredencialesCorrecta_RedireccionaHome()
        {
            _driver.Navigate().GoToUrl($"{_urlBase}/Auth/Login");

            _driver.FindElement(By.Name("Email")).SendKeys("judithcaceres1@gmail.com");
            _driver.FindElement(By.Name("Password")).SendKeys("12345678");

            _driver.FindElement(By.CssSelector("button[type = 'submit']")).Click();

            Assert.IsTrue(_driver.Url.Contains("https://localhost:44327/"));

        }
    }
}
