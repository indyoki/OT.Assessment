using System.Reflection;
using OT.Assessment.App.BusinessLogic;
using OT.Assessment.App.BusinessLogic.Interfaces;
using OT.Assessment.App.Repository;
using OT.Assessment.App.Repository.Interfaces;
using Serilog;


var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddScoped(_ => configuration);
builder.Services.AddScoped<IQueueManager, QueueManager>();
builder.Services.AddScoped<ISqlRepository, SqlRepository>();
builder.Services.AddScoped<IPlayerWagerService, PlayerWagerService>();


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.File("Logs.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckl
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(opts =>
    {
        opts.EnableTryItOutByDefault();
        opts.DocumentTitle = "OT Assessment App";
        opts.DisplayRequestDuration();
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
