using System.IO;
using System.Text.Json;
using BotLinkedn.Models;

namespace BotLinkedn.Utils
{
    public static class JsonReader
    {
        public static LoginModel RetrieveDataJson()
        {
            using(StreamReader reader = new StreamReader("appsettings.json"))
            {
                return JsonSerializer.Deserialize<LoginModel>(reader.ReadToEnd());
            }
        }
    }
}