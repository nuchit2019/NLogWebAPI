using NLog.Web;
using NLogWebAPI.Services;

var builder = WebApplication.CreateBuilder(args); 

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddScoped<ITniLoggerService, TniLoggerService>();     


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure NLog
builder.Logging.ClearProviders(); 

// ãªéä¿Åì nlog.config
builder.Host.ConfigureLogging(logging =>
{
    logging.ClearProviders();
    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);

}).UseNLog();

builder.Host.UseNLog();
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
