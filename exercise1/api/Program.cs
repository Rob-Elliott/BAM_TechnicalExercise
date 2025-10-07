using Microsoft.EntityFrameworkCore;
using Serilog;
using StargateAPI;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    // Prefix controller routes with /api so Razor Pages are at root
    options.Conventions.Add(new RoutePrefixConvention(new Microsoft.AspNetCore.Mvc.RouteAttribute("api")));
});

builder.Services.AddRazorPages();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<StargateContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("StarbaseApiDatabase")));

// All REST Validation (IRequestPreProcessors) need to be added here
builder.Services.AddMediatR(cfg =>
{
    cfg.AddRequestPreProcessor<CreatePersonPreProcessor>();
    cfg.AddRequestPreProcessor<UpdatePersonPreProcessor>();
    cfg.AddRequestPreProcessor<CreateAstronautDutyPreProcessor>();
    cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly);
});

var approot = builder.Configuration.GetValue<string>(WebHostDefaults.ContentRootKey);
var dbPath = Path.Combine(approot, "starbase.db");

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.SQLite(dbPath, "Logs") // Specify the SQLite database file
    .WriteTo.Console()
    .WriteTo.Debug()
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.Run();


