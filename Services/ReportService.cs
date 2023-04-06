using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BotLinkedn.Interfaces;

namespace BotLinkedn.Services
{
    public sealed class ReportService : IReportService
    {
        private static int _positionBin = Assembly.GetExecutingAssembly().Location.IndexOf("bin");
        private static readonly string _localFolder = Assembly.GetAssembly(typeof(ReportService)).Location.Substring(0, _positionBin);
        private static string _nameFile = "ReportBot.csv";
        public void CreateFile(string nameFolder)
        {
            var pathComplete = Path.Combine(_localFolder, nameFolder, _nameFile);
            
            File.Create(pathComplete).Dispose();

            WriteFile("Nome;Link Contato;Ja Enviado?;",nameFolder);
        }

        public void WriteFile(string data, string nameFolder)
        {
            var pathFile = Path.Combine(_localFolder, nameFolder, _nameFile);

            using (StreamWriter writer = new StreamWriter(pathFile, true))
            {
                writer.WriteLine(data);
                writer.Close();
            }
        }
    }
}