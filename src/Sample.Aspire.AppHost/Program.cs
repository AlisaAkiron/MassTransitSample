using Projects;
using Sample.Aspire.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

builder.ConfigureAppHost();

var postgresqlPassword = builder.AddParameter("postgresql-password", true);
var postgresqlTag = builder.AddParameter("postgresql-tag");
var rabbitMqTag = builder.AddParameter("rabbitmq-tag");

var rabbitMqUser = builder.AddParameter("rabbitmq-user");
var rabbitMqPassword = builder.AddParameter("rabbitmq-password", true);

var enablePgadmin = builder.AddParameter("enable-pgadmin");

var postgresql = builder
    .AddResourceWithConnectionString(b =>
    {
        var pg = b
            .AddPostgres("postgresql-instance", password: postgresqlPassword)
            .WithOtlpExporter()
            .WithImageTag(postgresqlTag.Resource.Value)
            .WithDataVolume("masstransit-sample-db");
        if (enablePgadmin.Resource.Value == "true")
        {
            pg.WithPgAdmin(pgadmin => pgadmin.WithImageTag("latest"));
        }

        pg.AddDatabase("postgresql-db", "MassTransitSample");
        return pg;
    }, "PostgreSQL");

var rabbitMq = builder
    .AddResourceWithConnectionString(b => b
        .AddRabbitMQ("rabbit-mq", userName: rabbitMqUser, password: rabbitMqPassword)
        .WithOtlpExporter()
        .WithImageTag(rabbitMqTag.Resource.Value)
        .WithDataVolume("masstransit-sample-mq")
        .WithManagementPlugin(), "RabbitMQ");

var migrator = builder.AddProject<Sample_Migrator>("migrator")
    .WithReference(postgresql, "PostgreSQL");

builder.AddProject<Sample_Web>("web")
    .WithReference(postgresql, "PostgreSQL")
    .WithReference(rabbitMq, "RabbitMQ");

builder.AddProject<Sample_Worker>("worker")
    .WithReference(postgresql, "PostgreSQL")
    .WithReference(rabbitMq, "RabbitMQ");

await builder.Build().RunAsync();
