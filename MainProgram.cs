using System;
using BotLinkedn.Drivers;
using BotLinkedn.Models;
using BotLinkedn.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace BotLinkedn
{
    public class MainProgram
    {
        static void Main(string[] args)
        {
            var serviceProvider = ConfigureStartupServices();

            LoginModel login = JsonReader.RetrieveDataJson();

            var driver = ChromeDriverPrepare.PrepareDriver(login, serviceProvider);

            driver.NavigateAndLogin();

            driver.SearchKeyWord();

            driver.SendMessageToAll();
        }

        private static ServiceProvider ConfigureStartupServices(){

            var serviceCollection  = new ServiceCollection();

            Startup startupConsole = new Startup();

            startupConsole.ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            return serviceProvider;
        }
    }
}
