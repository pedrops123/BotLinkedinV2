using System;
using System.Linq;
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

        public void SearchKeyWord(string keyWord = "Pessoas")
        {
            var xpathChevronDown = "//li-icon[contains(@type,'chevron-down')]";

            WaitUntilElementIsVisible(By.XPath(xpathChevronDown),30);
            var CollapseIcons = _driver.FindElements(By.XPath(xpathChevronDown));
            try
            {
                CollapseIcons[1].Click();
            }
            catch(Exception)
            {
                CollapseIcons[0].Click();
            }
            
            var inputSearch = _driver.FindElement(By.XPath("//input[contains(@class, 'search-global-typeahead__input')]"));
            inputSearch.Clear();
            inputSearch.Click();
            inputSearch.SendKeys(keyWord.ToLower() == "pessoas" ? _loginModel.KeyWordAddConnections : _loginModel.KeyWordSearchJobs);
            inputSearch.SendKeys(Keys.Enter);

            ValidateSearchNotFound();

            var xpathBtnPersons = $"//button[contains(@class,'artdeco-pill') and text()='{ keyWord }']";
            
            WaitUntilElementIsVisible(By.XPath(xpathBtnPersons));

            var btnPersons = _driver.FindElement(By.XPath(xpathBtnPersons));

            btnPersons.Click();
        }

        public void SendMessageToAll()
        {
            string xpathResultGrid = "//div[@class='entity-result__item']";

            WaitUntilElementIsVisible(By.XPath(xpathResultGrid));

            bool isLocked = false;

            while(!isLocked)
            {
                var results = _driver.FindElements(By.XPath(xpathResultGrid));
                
                foreach(IWebElement elementDiv in results)
                {
                    try
                    {
                        var name = elementDiv.Text.Split("\r")[0];

                        var url = elementDiv.FindElement(By.XPath("div[contains(@class,'entity-result__universal-image')]/div/a")).GetAttribute("href");

                        var existsConnect = elementDiv.FindElements(By.TagName("button")).Where(r => r.Text == "Conectar" ).Count() > 0;

                        if(existsConnect)
                        {
                            var btnConnect = elementDiv.FindElements(By.TagName("button")).Where(r => r.Text == "Conectar").FirstOrDefault();
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

                            var xPathLimitLockAddContact = "//div[contains(@class, 'ip-fuse-limit-alert__warning')]";
                            var xPathButtonOkLimitLockAddContact = "//button[contains(@aria-label, 'Entendi')]";

                            Thread.Sleep(new TimeSpan(0,0,3));

                            var hasElementLimitLock = _driver.FindElements(By.XPath(xPathLimitLockAddContact)).Count;

                            if(hasElementLimitLock > 0)
                            {
                                var btnOkLimitLockAddContact = _driver.FindElement(By.XPath(xPathButtonOkLimitLockAddContact));

                                btnOkLimitLockAddContact.Click();

                                Console.WriteLine("Excedeu o limite de convites por semana ! Tente novamente na proxima.");

                                Logout();

                                goto end;
                            }

                            var xPathErrorSendInviteIcon = "//li-icon[contains(@type,'error-pebble-icon')]";
                            var btnExitErrorSendInviteIcon = "//div[contains(@data-test-artdeco-toast-item-type,'error')]/button";

                            var hasElementErrorSendInvite = _driver.FindElements(By.XPath(xPathErrorSendInviteIcon));

                            if(hasElementErrorSendInvite.Count > 0)
                            {
                                var btnExitError = _driver.FindElement(By.XPath(btnExitErrorSendInviteIcon));

                                btnExitError.Click();

                                goto endLoop;
                            }
                            
                            _reportService.WriteFile($"{name};{url};Não", _loginModel.FolderName);

                            Thread.Sleep(new TimeSpan(0,0,6));
                        }
                        else
                        {
                            _reportService.WriteFile($"{name};{url};Sim", _loginModel.FolderName);
                        }                        
                    }
                    catch(Exception e)
                    {
                        System.Console.WriteLine("error");
                    }

                    endLoop:;
                }

                Thread.Sleep(new TimeSpan(0,0,10));

                bool existsElementNext = false;
                var xpathBtnNextPagination = "//button[contains(@aria-label,'Avançar')]";

                while(existsElementNext == false)
                {    
                    existsElementNext = _driver.FindElements(By.XPath(xpathBtnNextPagination)).Count > 0;
                    if(existsElementNext == false)
                    {
                         ArrowDownPage(1);
                    }
                }
               
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

                 ValidateSearchNotFound();
            }

            end:; 
        }

        private void Logout()
        {
            string xPathMenuButtons = "//span[contains(@class,'global-nav__primary-link-text')]";
            string classSubMenuLogout = "artdeco-dropdown__content-inner";

            var topMenuButtons = _driver.FindElements(By.XPath(xPathMenuButtons));

            topMenuButtons[5].Click();

            var buttonsSubMenu = _driver.FindElements(By.ClassName(classSubMenuLogout));

            var btnLogout = buttonsSubMenu[0].FindElements(By.TagName("li")).LastOrDefault();
            
            btnLogout.Click();

            Thread.Sleep(new TimeSpan(0,0,6));
        }

        public void CloseNavigator()
        {
            _driver.Close();
        }

        public void ValidateSearchNotFound()
        {
              string xPathSearchNotFound = "//section[contains(@class,'artdeco-empty-state ember-view')]";

              string XPathChevronUp = "//li-icon[contains(@type,'chevron-up')]";

              var elementNotFound = _driver.FindElements(By.XPath(xPathSearchNotFound));

                 if(elementNotFound.Count > 0)
                 {
                    var buttonUpMessage = _driver.FindElement(By.XPath(XPathChevronUp));
                    buttonUpMessage.Click();
                    
                    SearchKeyWord();
                 }
        }

        public void ApplyToAllJobs()
        {
            Thread.Sleep(new TimeSpan(0,0,6));

            int countPagination = 0;
            
            topJobs:;

            string xPathGridJobs = "//ul[contains(@class,'scaffold-layout__list-container')]/li";
            WaitUntilElementIsVisible(By.XPath(xPathGridJobs), 20);

            var gridJobsResults = _driver.FindElements(By.XPath(xPathGridJobs));

            foreach(IWebElement tileJob in gridJobsResults)
            {
                var tagSimpleApply = tileJob.FindElements(By.TagName("li")).Where(r=>r.Text.ToLower().Contains("simplificada")).FirstOrDefault();
                
                if(tagSimpleApply != null)
                {
                    var panelDescriptionJob = _driver.FindElement(By.XPath("//div[contains(@class,'jobs-unified-top-card__content--two-pane')]"));
                    var btnSimpleApply = panelDescriptionJob.FindElements(By.TagName("button")).Where(r => r.Text.ToLower().Contains("simplificada")).FirstOrDefault();

                    btnSimpleApply.Click();

                    Thread.Sleep(new TimeSpan(0,0,6));

                    var formApplyJob =  _driver.FindElement(By.XPath("//div[contains(@class,'jobs-easy-apply-content')]"));

                    var buttonNext = formApplyJob.FindElement(By.TagName("button"));

                    buttonNext.Click();

                    var resumeArea = _driver.FindElements(By.XPath("//span[contains(@class,'jobs-document-upload__title--is-required')]"));

                    if(resumeArea.Count > 0)
                    {
                        buttonNext.Click();
                    }

                    var finalForm = _driver.FindElements(By.XPath("//div[contains(@class,'jobs-easy-apply-content')]/div/form"));

                    if(finalForm.Count > 0)
                    {
                        var btnCloseModal = _driver.FindElement(By.XPath("//div[contains(@class,'artdeco-modal-overlay ')]/div/button/li-icon"));
                        btnCloseModal.Click();
                        Thread.Sleep(new TimeSpan(0,0,5));

                        var footerDialog = _driver.FindElements(By.XPath("//div[contains(@role,'alertdialog')]/div[contains(@class,'artdeco-modal__actionbar')]/button"));

                        


                    }

                }
            }

            var url = _driver.Url;

            if(!url.Contains("start"))
            {
                countPagination = 25;

                url = url + $"&start={ countPagination }";
            }
            else
            {
                countPagination = countPagination + 25;

                var indexOfStartParameter = url.IndexOf("&start");

                url  = url.Remove(indexOfStartParameter);

                url = url + $"&start={ countPagination }";
            }

            _driver.Navigate().GoToUrl(url);

            goto topJobs;
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
            catch(Exception e)
            {
                Console.WriteLine($"Elemento { element } não foi encontrado no contexto da pagina !");

                throw e;
            }           
        }
    }
}