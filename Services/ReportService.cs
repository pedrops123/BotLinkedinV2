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

        public void CreateFile(string nameFolder)
        {
            var pathComplete = Path.Combine(_localFolder, nameFolder);
            File.Create(pathComplete).Dispose();
        }

        public void WriteFile(ICollection<string> data)
        {
            throw new System.NotImplementedException();
        }
    }
}