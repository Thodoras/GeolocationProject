using GeolocationAPI.Infrastructure.DB.MSSQLExpress.Models;
using GeolocationAPI.Infrastructure.External.Http.Clients;
using GeolocationAPI.Application.GeolocationIP;
using GeolocationAPI.Application.GeolocationIP.Interfaces;
using GeolocationAPI.Application.BackgroundGeolocationIP;
using GeolocationAPI.Application.BackgroundGeolocationIP.Interfaces;
using GeolocationAPI.Infrastructure.DB.MSSQLExpress.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var settings = builder.Configuration.GetSection("DISettings").Get<DISettings>() ??
    throw new Exception("Failed to load DISettings from configuration.");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

builder.Services.AddDbContext<GeolocationAPIDbContext>(options =>
{
    options.UseSqlServer(settings.GetMSSQLExpressConnectionString());
});

builder.Services.AddScoped<IBatchProcessRepository, BatchProcessRepository>();
builder.Services.AddScoped<IBatchProcessItemBackgroundRepository, BatchProcessItemRepository>();
builder.Services.AddScoped<IBatchProcessBackgroundRepository, BatchProcessRepository>();

builder.Services.AddScoped<IGeoIPService>(provider =>
{
    var logger = provider.GetRequiredService<ILogger<FreeGeoIPAPI>>();
    var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
    var minDelayBetweenRequests = TimeSpan.FromMilliseconds(settings.minDelayBetweenRequestsInMilliseconds);
    var client = httpClientFactory.CreateClient();
    return new FreeGeoIPAPI(
        settings.FreeGeoIPURL,
        settings.FreeGeoIPKey,
        minDelayBetweenRequests,
        client,
        logger
    );
});

builder.Services.AddScoped<IBackgroundGeolocationIPService>(provider =>
{
    var geoIPService = provider.GetRequiredService<IGeolocationIPService>();
    var batchProcessRepository = provider.GetRequiredService<IBatchProcessRepository>();
    var serviceScopeFactory = provider.GetRequiredService<IServiceScopeFactory>();
    var logger = provider.GetRequiredService<ILogger<BackgroundGeolocationIPService>>();
    int maxConcurrency = settings.MaxConcurrency;
    int updateInterval = settings.UpdateInterval;
    return new BackgroundGeolocationIPService(
        geoIPService,
        batchProcessRepository,
        serviceScopeFactory,
        maxConcurrency,
        updateInterval,
        logger
    );
});
builder.Services.AddHostedService<BackgroundGeolocationIPWorker>();

builder.Services.AddScoped<IGeolocationIPService, GeolocationIPService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();
app.MapControllers();
app.Run();