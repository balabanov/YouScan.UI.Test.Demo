using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.PageObjects;
using NUnit.Framework;
using System;

namespace YouScan.UI.Test.Demo
{
    public class Page
    {
        public IWebDriver _driver;

        public Page(IWebDriver driver)
        {
            this._driver = driver;
            PageFactory.InitElements(_driver, this);
        }
    }

    public class LoginPage : Page
    {
        [FindsBy(How = How.Name)]
        private IWebElement username;

        [FindsBy(How = How.Name)]
        private IWebElement password;

        [FindsBy(How = How.LinkText, Using = "Войти")]
        private IWebElement login;

        public LoginPage(IWebDriver driver)
            : base(driver)
        {
            Console.WriteLine(_driver.Title);
            Console.WriteLine(_driver.Url);
            if (!_driver.Title.Contains("Авторизация") || !_driver.Url.Equals(Properties.Settings.Default.Environment + "Login/LogOn"))
                throw new NoSuchWindowException("This is not the YouScan login page");
        }

        public DashboardPage DoLogin(string UserName, string Password)
        {
            username.SendKeys(UserName);
            password.SendKeys(Password);
            login.Click();
            PageFactory.InitElements(_driver, (new DashboardPage(_driver)));
            return new DashboardPage(_driver);
        }
    }

    public class DashboardPage : Page
    {
        [FindsBy(How = How.XPath, Using = "//span[@class='login']/span[@class='white']")]
        private IWebElement ActualUsername;

        public DashboardPage(IWebDriver driver)
            : base(driver)
        {
            if (!_driver.Title.Contains("Список Тем: Мониторинг Социальных Медиа|YouScan") || !_driver.Url.Equals(Properties.Settings.Default.Environment + "Theme"))
                throw new NoSuchWindowException("This is not the YouScan dashboard page");
        }

        public bool IsCurrentUser(string username)
        {
            return ActualUsername.Text.Equals(username);
        }
    }

    [TestFixture]
    public class LoginPageTest
    {
        private IWebDriver driver;
        private LoginPage Login;

        [SetUp]
        [Description("Sets up the test fixture page objects and navigates to the login page.")]
        public void SetUp()
        {
            driver = new FirefoxDriver();
            driver.Navigate().GoToUrl(Properties.Settings.Default.Environment+"Login/LogOn");
            Login = new LoginPage(driver);
            PageFactory.InitElements(driver, Login);
        }

        [Test]
        [Description("Enters invalid credentials and asserts that a correct error message is displayed.")]
        public void SubmitFormInvalidCredentials()
        {
            DashboardPage Dashboard = Login.DoLogin(Properties.Settings.Default.Login, Properties.Settings.Default.Password);
            Assert.True(Dashboard.IsCurrentUser(Properties.Settings.Default.Login), "Invalid user name or password");
        }

        [TearDown]
        [Description("Cleans up, shuts down.")]
        public void Shutdown()
        {
            driver.Quit();
        }
    }
}
