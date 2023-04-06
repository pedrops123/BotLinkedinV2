using System;
using System.Threading;
using BotLinkedn.Interfaces;
using BotLinkedn.Models;
using Microsoft.Extensions.DependencyInjection;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace BotLinkedn.Commands
{
    public sealed class CommandsBot
    {
        private readonly IWebDriver _driver;
        private readonly LoginModel _loginModel;
        private readonly int _positionButtonTypePerson = 1;
        private readonly ServiceProvider _provider;
        private readonly IReportService _reportService;
        
        public CommandsBot(
            IWebDriver driver,
            LoginModel loginModel,
            ServiceProvider provider)
        {
            _driver = driver;
            _loginModel = loginModel;
            _provider = provider;
            _reportService = _provider.GetService<IReportService>();
            _reportService.CreateFile(_loginModel.FolderName);
        }

        public void NavigateAndLogin()
        {
            _driver.Navigate().GoToUrl("https://www.linkedin.com/");

             WaitUntilElementIsVisible(By.XPath("//input[@id='session_key']"));

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
            var xpathChevronDown = "//li-icon[contains(@type,'chevron-down')]";

            WaitUntilElementIsVisible(By.XPath(xpathChevronDown),30);
            var CollapseIcons = _driver.FindElements(By.XPath(xpathChevronDown));
            CollapseIcons[1].Click();

            var inputSearch = _driver.FindElement(By.XPath("//input[contains(@class, 'search-global-typeahead__input')]"));
            inputSearch.Click();
            inputSearch.SendKeys(_loginModel.KeyWord);
            inputSearch.SendKeys(Keys.Enter);

            var xpathBtnPersons = $"(//button[contains(@class,'artdeco-pill')])[{_positionButtonTypePerson}]";
            
            WaitUntilElementIsVisible(By.XPath(xpathBtnPersons));

            var btnPersons = _driver.FindElement(By.XPath(xpathBtnPersons));

            btnPersons.Click();
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
                        var name = elementDiv.Text.Split("\r")[0];

                        var existsConnect = elementDiv.FindElements(By.XPath("//button[contains(span,'Conectar')]")).Count > 0;

                        if(existsConnect)
                        {
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

                            _reportService.WriteFile($"{name};link;Não", _loginModel.FolderName);

                            Thread.Sleep(new TimeSpan(0,0,6));
                        }
                        else
                        {
                            _reportService.WriteFile($"{name};link;Sim", _loginModel.FolderName);
                        }                        
                    }
                    catch(Exception e)
                    {
                        System.Console.WriteLine("error");
                    }
                }

                Thread.Sleep(new TimeSpan(0,0,10));

                ArrowDownPage(30);

                var xpathBtnNextPagination = "//button[contains(@aria-label,'Avançar')]";

                var test = WaitUntilElementIsVisible(By.XPath(xpathBtnNextPagination), 30);

                var btnNextPagination = _driver.FindElement(By.XPath(xpathBtnNextPagination));

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

        public IWebElement WaitUntilElementIsVisible(By element, int seconds = 10)
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(seconds));
                return wait.Until(driver => driver.FindElement(element));
            }
            catch(NoSuchElementException)
            {
                Console.WriteLine($"Elemento { element } não foi encontrado no contexto da pagina !");

                throw;
            }           
        }
    }
}