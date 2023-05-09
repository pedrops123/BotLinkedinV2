using System;
using System.Collections.Generic;
using System.Threading;
using BotLinkedn.Drivers;
using BotLinkedn.Models;
using BotLinkedn.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace BotLinkedn
{
    public class MainProgram
    {
        private static ServiceProvider ConfigureStartupServices(){

            var serviceCollection  = new ServiceCollection();

            Startup startupConsole = new Startup();

            startupConsole.ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            return serviceProvider;
        }

        public static Dictionary<int,string> ListOptions()
        {
            Dictionary<int,string> dictionaryOptions = new Dictionary<int, string>();

            dictionaryOptions.Add(1, "Adicionar Novos Contatos.");
            dictionaryOptions.Add(2, "Se Aplicar a Novos Empregos.");
            dictionaryOptions.Add(0, "Sair da Aplicação.");

            return dictionaryOptions;
        }

        private static void ClearConsole() => Console.Clear();

        private static void SearchAndAddConnections(LoginModel login, ServiceProvider serviceProvider)
        {
            var driver = ChromeDriverPrepare.PrepareDriver(login, serviceProvider);

            driver.NavigateAndLogin();

            driver.SearchKeyWord();

            driver.SendMessageToAll();

            driver.CloseNavigator();
        }

        private static void SearchJobs(LoginModel login, ServiceProvider serviceProvider)
        {
            var driver = ChromeDriverPrepare.PrepareDriver(login, serviceProvider);

            driver.NavigateAndLogin();
            driver.SearchKeyWord("Vagas");
        }
        
        static void Main(string[] args)
        {
            int lengthMessage = 300;
            var serviceProvider = ConfigureStartupServices();

            int choiceOption;

            LoginModel login = JsonReader.RetrieveDataJson();

            if(login.Message.Length > lengthMessage)
            {
                throw new ArgumentException($"Mensagem para convite maior que { lengthMessage } caracteres. Favor diminuir o tamanho da mensagem !");
            }

            topConsole:;

            var listOptions = ListOptions();

            Console.WriteLine("---------------------- PROJETO AUTOMAÇÃO LINKEDIN V2 ---------------------------");
            
            foreach(KeyValuePair<int,string> option in listOptions)
            {
                Console.WriteLine(string.Format("{0} - {1}", option.Key, option.Value));
            }

            var result = Console.ReadLine();

            var isInteger = int.TryParse(result, out choiceOption);

            if(!isInteger)
            {
                ClearConsole();

                Console.WriteLine("Atenção ! Escolha digitada não é inteiro. Somente numeros inteiros ! \n");

                goto topConsole;
            }

            switch (choiceOption)
            {
                case 0:
                    Console.WriteLine("Saiu da Aplicação.");
                    Thread.Sleep(new TimeSpan(0,0,6));

                    Environment.Exit(-1);
                break;

                case 1:
                    SearchAndAddConnections(login, serviceProvider);
                    goto topConsole;
                break;

                case 2:
                    SearchJobs(login, serviceProvider);
                    goto topConsole;
                break;
                    
                default:

                     ClearConsole();

                     Console.WriteLine("Escolha não existe ! \n");

                     goto topConsole;
            }
        }        
    }
}