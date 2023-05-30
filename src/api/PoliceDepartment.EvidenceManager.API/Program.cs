using PoliceDepartment.EvidenceManager.API.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
       .SetBasePath(builder.Environment.ContentRootPath)
       .AddJsonFile("appsettings.json", true, true)
       .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
       .AddEnvironmentVariables();

var isDevelopment = builder.Environment.IsDevelopment();

builder.Services.AddDependencyInjection(builder.Configuration, isDevelopment);

var app = builder.Build();

app.UseDependencyInjection(isDevelopment);

app.Run();
