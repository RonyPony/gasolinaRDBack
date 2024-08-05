using combustibleWorker.AppSettingModels;
using combustibleWorker.Interface;
using combustibleWorker.Repository;
using combustibleWorker.Services;
using System.Data;

using System.Data.SqlClient;

public class Program
{
    public static void Main(string[] args)
    {
        // Establecer el directorio de trabajo actual
        Directory.SetCurrentDirectory(AppContext.BaseDirectory);

        IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                // Configurar explícitamente la lectura del archivo appsettings.json
                config.SetBasePath(Directory.GetCurrentDirectory());
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                services.Configure<UrlPage>(context.Configuration.GetSection("UrlPage"));
                services.Configure<XPathExpression>(context.Configuration.GetSection("XPathExpression"));
                services.Configure<WaitingTimeMilisecond>(context.Configuration.GetSection("WaitingTimeMilisecond"));
                services.Configure<OnFailureRetryQty>(context.Configuration.GetSection("OnFailureRetryQty"));
                services.Configure<Schedule>(context.Configuration.GetSection("Schedule"));
                services.AddTransient<ICombustibleService, CombustibleService>();
                services.AddTransient<IDataRepository, DataRepository>();
                services.AddTransient<IDbConnection>(db => new SqlConnection(
                           context.Configuration.GetConnectionString("DefaultConnection")));
                services.AddHostedService<Worker>();
            })
            .UseWindowsService()
            .Build();

        host.Run();
    }
}
