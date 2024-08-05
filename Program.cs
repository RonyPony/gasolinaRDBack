using CombustiblesrdBack.AppSettingModels;
using CombustiblesrdBack.Interface;
using CombustiblesrdBack.Repository;
using CombustiblesrdBack.Services;
using System.Data.SqlClient;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<UrlPage>(builder.Configuration.GetSection("UrlPage"));
builder.Services.Configure<XPathExpression>(builder.Configuration.GetSection("XPathExpression"));
builder.Services.AddTransient<IDbConnection>(db => new SqlConnection(
                   builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddTransient<ICombustibleService, CombustibleService>();
builder.Services.AddTransient<IDataRepository, DataRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
