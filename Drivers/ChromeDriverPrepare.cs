
using System.IO;
using System.Reflection;
using BotLinkedn.Commands;
using BotLinkedn.Models;
using Microsoft.Extensions.DependencyInjection;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace BotLinkedn.Drivers
{
    public static class ChromeDriverPrepare
    {
        private static int _positionBin = Assembly.GetExecutingAssembly().Location.IndexOf("bin");
        private static readonly string _localFolder = Assembly.GetAssembly(typeof(ChromeDriverPrepare)).Location.Substring(0, _positionBin);

        public static CommandsBot PrepareDriver(LoginModel loginModel, ServiceProvider provider)
        {
            string pathFolderDownloads = Path.Combine(_localFolder, loginModel.FolderName);
             if(!Directory.Exists(pathFolderDownloads))
                     Directory.CreateDirectory(pathFolderDownloads); 
                    //  DirectoryInfo info = new DirectoryInfo(pathFolderDownloads);
                    //  DirectorySecurity
            
            ChromeOptions opt = new ChromeOptions();
            // Define pasta root para salvar downloads
            opt.AddUserProfilePreference("download.default_directory", Path.Combine(_localFolder, loginModel.FolderName));
            opt.AddArguments(new string[] { "start-maximized", "--disable-notifications" });
            IWebDriver driver = new ChromeDriver(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),opt
                );
                
            return new CommandsBot(driver, loginModel, provider);
        }
    }
}