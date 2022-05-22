using Amazon.S3;

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// builder.Services.AddAWSService<IAmazonS3>(); //This doesn't work
builder.Services.AddSingleton<IAmazonS3>(_ =>
{
    return new AmazonS3Client(new AmazonS3Config {
        UseHttp = true,
        ServiceURL = configuration["AWS:ServiceUrl"],
        ForcePathStyle = true
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