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
            int lengthMessage = 300;
            var serviceProvider = ConfigureStartupServices();

            LoginModel login = JsonReader.RetrieveDataJson();

            if(login.Message.Length > lengthMessage)
            {
                throw new ArgumentException($"Mensagem para convite maior que { lengthMessage } caracteres. Favor diminuir o tamanho da mensagem !");
            }

            var driver = ChromeDriverPrepare.PrepareDriver(login, serviceProvider);

            driver.NavigateAndLogin();

            driver.SearchKeyWord();

            driver.SendMessageToAll();

            driver.CloseNavigator();
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
