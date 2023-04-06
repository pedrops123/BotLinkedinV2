using BotLinkedn.Interfaces;
using BotLinkedn.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BotLinkedn {

    public sealed class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
             services.AddTransient<IReportService, ReportService>();
        }
    }
}