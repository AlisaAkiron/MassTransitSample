using MassTransit;
using Sample.Aspire.ServiceDefaults;
using Sample.Database;

var builder = WebApplication.CreateBuilder(args);

builder.AddDefaultServices();
builder.AddSampleDbContext();
builder.Services.AddMassTransit(options =>
{
    options.SetKebabCaseEndpointNameFormatter();

    options.UseAspireRabbitMq((c, cfg) =>
    {
        cfg.ConfigureEndpoints(c);
    });

    options.AddEntityFrameworkOutbox<SampleDbContext>(o =>
    {
        o.UsePostgres();
        o.UseBusOutbox();
    });
});
builder.Services.AddMassTransitObservable();

builder.Services.AddControllers();

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapControllers();

await app.RunAsync();
