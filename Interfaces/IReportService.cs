using System.Collections.Generic;

namespace BotLinkedn.Interfaces
{
    public interface IReportService
    {
        void CreateFile(string nameFolder);

        void WriteFile(ICollection<string> data);
    }
}