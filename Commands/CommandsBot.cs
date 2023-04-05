using System;
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
            var CollapseIcons = _driver.FindElements(By.XPath("//li-icon[contains(@type,'chevron-down')]"));
            CollapseIcons[1].Click();

            var inputSearch = _driver.FindElement(By.XPath("//input[contains(@class, 'search-global-typeahead__input')]"));
            inputSearch.Click();
            inputSearch.SendKeys(_loginModel.KeyWord);
            inputSearch.SendKeys(Keys.Enter);

            Thread.Sleep(new TimeSpan(0,0,3));

            var btnPersons = _driver.FindElement(By.XPath($"(//button[contains(@class,'artdeco-pill')])[{_positionButtonTypePerson}]"));

            btnPersons.Click();

            Thread.Sleep(new TimeSpan(0,0,10));

        }

        public void SendMessageToAll()
        {
            bool isLocked = false;

            while(!isLocked)
            {
                var results = _driver.FindElements(By.XPath("//div[@class='entity-result__item']"));
                
                foreach(IWebElement elementDiv in results)
                {
                    try
                    {
                        var name = elementDiv.FindElement(By.XPath("//span[@dir='ltr']")).Text.Split("\r")[0];
                        var btnConnect = elementDiv.FindElement(By.XPath("//button[contains(span,'Conectar')]"));
                        btnConnect.Click();
                        Thread.Sleep(new TimeSpan(0,0,6));   

                        bool existsDialogRequest = _driver.FindElements(By.XPath("//button[contains(@aria-label,'Other')]")).Count > 0;

                        if(existsDialogRequest)
                         {
                             var dialogRequest = _driver.FindElement(By.XPath("//div[contains(@role,'dialog')]"));
                             var buttonOther = dialogRequest.FindElement(By.XPath("//button[contains(@aria-label,'Other')]"));
                             var buttonConnect = dialogRequest.FindElement(By.XPath("//button[contains(@aria-label, 'Conecte')]"));
                             buttonOther.Click();
                             buttonConnect.Click();
                             Thread.Sleep(new TimeSpan(0,0,6));                                   
                         }

                         bool existsInputEmail = _driver.FindElements(By.XPath("//input[contains(@type,'email')]")).Count > 0;
                         if(existsInputEmail)
                         {
                            var emailInput = _driver.FindElement(By.XPath("//input[contains(@type,'email')]"));
                            emailInput.SendKeys(_loginModel.Userame);
                         }

                        var message = _loginModel.Message.Replace("{{name}}", name);
                        var btnAddNote = _driver.FindElement(By.XPath("//button[contains(@aria-label, 'Adicionar nota')]"));

                        btnAddNote.Click();
                   
                        var textArea = _driver.FindElement(By.XPath("//textarea[contains(@id, 'custom-message')]"));

                         textArea.SendKeys(message);

                         var btnSendMessage = _driver.FindElement(By.XPath("//button[contains(@aria-label,'Enviar agora')]"));

                         btnSendMessage.Click();

                     Thread.Sleep(new TimeSpan(0,0,6));
                    }
                    catch(Exception e)
                    {
                        System.Console.WriteLine("error");
                    }
                }

                ArrowDownPage(30);

                Thread.Sleep(new TimeSpan(0,0,10));

                var btnNextPagination = _driver.FindElement(By.XPath("//button[contains(@aria-label,'Avançar')]"));

                if(!btnNextPagination.Enabled)
                {
                    isLocked = true;
                }
                else
                {
                    btnNextPagination.Click();
                }

                 Thread.Sleep(new TimeSpan(0,0,5));
            } 
        }

        public void ArrowDownPage(int times)
        {
            IWebElement bodyPage = _driver.FindElement(By.TagName("body"));

            for(int i = 0; i <= times; i++){
                bodyPage.SendKeys(Keys.ArrowDown);
            }
        }
    }
}