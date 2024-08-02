using combustibleWorker.AppSettingModels;
using combustibleWorker.Interface;
using combustibleWorker.Repository;
using combustibleWorker.Services;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;


IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context,services)=>
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
    .Build();

await host.RunAsync();
