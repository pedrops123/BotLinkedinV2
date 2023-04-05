using System;
using System.Collections.Generic;
using System.Threading;
using BotLinkedn.Models;
using OpenQA.Selenium;

namespace BotLinkedn.Commands
{
    public sealed class CommandsBot
    {
        private readonly IWebDriver _driver;
        private readonly LoginModel _loginModel;
        private readonly int _positionButtonTypePerson = 1;
        public CommandsBot(
            IWebDriver driver,
            LoginModel loginModel)
        {
            _driver = driver;
            _loginModel = loginModel;
        }

        public void NavigateAndLogin()
        {
            _driver.Navigate().GoToUrl("https://www.linkedin.com/");

            Thread.Sleep(new TimeSpan(0,0,10));

            var inputLogin = _driver.FindElement(By.XPath("//input[@id='session_key']"));
            var inputPassword = _driver.FindElement(By.XPath("//input[@id='session_password']"));
            var submitBtn = _driver.FindElement(By.XPath("//button[contains(@data-id, 'sign-in-form__submit-btn')]"));

            inputLogin.Click();
            inputLogin.SendKeys(_loginModel.Userame);
            Thread.Sleep(new TimeSpan(0,0,3));

            inputPassword.Click();
            inputPassword.SendKeys(_loginModel.Password);
            Thread.Sleep(new TimeSpan(0,0,3));

            submitBtn.Click();
        }

        public void SearchKeyWord()
        {
            var inputSearch = _driver.FindElement(By.XPath("//input[contains(@class, 'search-global-typeahead__input')]"));
            inputSearch.Click();
            inputSearch.SendKeys(_loginModel.KeyWord);
            inputSearch.SendKeys(Keys.Enter);

            Thread.Sleep(new TimeSpan(0,0,3));

            var btnPersons = _driver.FindElement(By.XPath($"(//button[contains(@class,'artdeco-pill')])[{_positionButtonTypePerson}]"));

            btnPersons.Click();

        }
    }
}