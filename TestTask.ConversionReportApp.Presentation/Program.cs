using Confluent.Kafka;
using TestTask.ConversionReportApp.Presentation;
using TestTask.ConversionReportApp.Presentation.ServiceExtensions;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddDomainServices();
builder.Services.AddConfiguredOptions(builder.Configuration);

builder.Services
    .AddPostgresConfiguration(builder.Configuration)
    .AddRedisCache(builder.Configuration)
    .AddMigrations()
    .AddPostgresRepositories()
    .AddExternalServices()
    .AddRedisCacheServices();

builder.Services
    .AddKafkaBackgroundService()
    .AddKafkaHandler<Ignore, string, ConversionRequestHandler>()
    .AddQuartzConfiguration();

builder.Services
    .AddGrpcConfiguration()
    .AddGrpcSwagger()
    .AddSwaggerGen()
    .AddFluentValidation();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MigrateUp();

app.MapGrpcServices();

app.Run();