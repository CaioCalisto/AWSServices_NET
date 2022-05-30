using Amazon.SQS;
using Sender;

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager configuration = builder.Configuration;

builder.Services.AddHostedService<CustomBackgroundService>();

var options = configuration.GetAWSOptions();
builder.Services.AddDefaultAWSOptions(options);
builder.Services.AddAWSService<IAmazonSQS>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();