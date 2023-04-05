using System;
using BotLinkedn.Drivers;
using BotLinkedn.Models;
using BotLinkedn.Utils;

namespace BotLinkedn
{
    public class MainProgram
    {
        static void Main(string[] args)
        {
            LoginModel login = JsonReader.RetrieveDataJson();

            var driver = ChromeDriverPrepare.PrepareDriver(login);

            driver.NavigateAndLogin();

            driver.SearchKeyWord();

        }
    }
}
