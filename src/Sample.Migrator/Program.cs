using Microsoft.EntityFrameworkCore;
using Sample.Aspire.ServiceDefaults;
using Sample.Database;
using Sample.Migrator;

var builder = Host.CreateApplicationBuilder(args);

if (EF.IsDesignTime is false)
{
    builder.AddDefaultServices();
    builder.Services.AddHostedService<Worker>();
}

builder.AddSampleDbContext();

var host = builder.Build();

await host.RunAsync();
