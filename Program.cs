using API_Backend_App_Honorarios.Services;
using API_Backend_App_Industrializacion.FontResolvers;
using API_Backend_App_Industrializacion.Persistence;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using PdfSharp.Fonts;
using System.Text.Json;
using System.Text.Json.Serialization;

using API_Backend_App_Honorarios.ExpressionInterpreting;
using API_Backend_App_Honorarios.Persistence;

var builder = WebApplication.CreateBuilder(args);

GlobalFontSettings.FontResolver = MontserratFontResolver.Instance;

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

string configuredDbPath = builder.Configuration.GetValue<string>("DatabaseLegacy:Path")
    ?? Path.Combine("Data", "datosIndustralizacion.db");

string dbPath = Path.IsPathRooted(configuredDbPath)
    ? configuredDbPath
    : Path.Combine(builder.Environment.ContentRootPath, configuredDbPath);

builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);

// REPLACED BY SINGLE SQLSERVER INSTANCE
/*builder.Services.AddDbContext<datosEstructuraDb>(options =>
    options.UseSqlite($"Data Source={dbPath};Cache=Shared;"));

builder.Services.AddDbContext<datosPorcentajesDb>(options =>
    options.UseSqlite($"Data Source={dbPath};Cache=Shared;"));*/

// TO BE REPLACED BY THE NEW VALUES


builder.Services.AddDbContext<EdificacionCalcDataDb>(options =>
    options.UseSqlServer(builder.Configuration.GetValue<string>("Database:Path")));

builder.Services.AddDbContext<EdificacionPlantasCalcDataDb>(options =>
    options.UseSqlServer(builder.Configuration.GetValue<string>("Database:Path")));

builder.Services.AddDbContext<ObraCivilCalcDataDb>(options =>
    options.UseSqlServer(builder.Configuration.GetValue<string>("Database:Path")));

builder.Services.AddDbContext<CoefsObraCivilCalcDataDb>(options =>
    options.UseSqlServer(builder.Configuration.GetValue<string>("Database:Path")));

builder.Services.AddDbContext<CoefHObraCivilCalcDataDb>(options =>
    options.UseSqlServer(builder.Configuration.GetValue<string>("Database:Path")));

builder.Services.AddDbContext<PEMRObraCivilCalcDataDb>(options =>
    options.UseSqlServer(builder.Configuration.GetValue<string>("Database:Path")));

builder.Services.AddDbContext<UrbanizacionCalcDataDb>(options =>
    options.UseSqlServer(builder.Configuration.GetValue<string>("Database:Path")));

/*builder.Services.AddDbContext<ProyectoDb>(options =>
    options.UseSqlServer(builder.Configuration.GetValue<string>("Database:Path")));*/

//builder.Services.AddScoped<ProjectPersistenceService>();

builder.Services.AddScoped<CalculationService>();


const string corsPolicyName = "ConfiguredCorsPolicy";

builder.Services.AddCors(options =>
{
    string[] allowedOrigins = builder.Configuration
        .GetSection("Cors:AllowedOrigins")
        .Get<string[]>() ?? [];

    options.AddPolicy(corsPolicyName, policy =>
    {
        if (allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins);
        }

        policy.AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddBlobServiceClient(builder.Configuration["DevBlobStorage:blobServiceUri"]!).WithName("DevBlobStorage");
    clientBuilder.AddQueueServiceClient(builder.Configuration["DevBlobStorage:queueServiceUri"]!).WithName("DevBlobStorage");
    clientBuilder.AddTableServiceClient(builder.Configuration["DevBlobStorage:tableServiceUri"]!).WithName("DevBlobStorage");
});

builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase == null ? null : null));
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.Logger.LogWarning("Application started in {EnvironmentName}; stdout logging probe emitted.", app.Environment.EnvironmentName);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
else
{
    app.UseExceptionHandler(exceptionHandlerApp =>
    {
        exceptionHandlerApp.Run(async context =>
        {
            IExceptionHandlerPathFeature? exceptionFeature =
                context.Features.Get<IExceptionHandlerPathFeature>();

            ILogger<Program> logger =
                context.RequestServices.GetRequiredService<ILogger<Program>>();

            if (exceptionFeature?.Error is not null)
            {
                logger.LogError(
                    exceptionFeature.Error,
                    "Unhandled exception while processing {Method} {Path}",
                    context.Request.Method,
                    exceptionFeature.Path);
            }

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/problem+json";

            await Results.Problem(
                title: "Internal Server Error",
                detail: "An unexpected error occurred.",
                statusCode: StatusCodes.Status500InternalServerError)
                .ExecuteAsync(context);
        });
    });
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors(corsPolicyName);

app.UseAuthorization();

app.MapControllers();

app.Run();
