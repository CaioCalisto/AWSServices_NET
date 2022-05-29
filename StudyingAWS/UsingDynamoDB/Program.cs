using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// builder.Services.AddAWSService<IAmazonS3>(); //This doesn't work
builder.Services.AddTransient<IDynamoDBContext, DynamoDBContext>();
builder.Services.AddSingleton<IAmazonDynamoDB>(_ =>
{
    return new AmazonDynamoDBClient(new AmazonDynamoDBConfig() {
        UseHttp = true,
        ServiceURL = configuration["AWS:ServiceUrl"]
    });
});

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