namespace Agregator.API.Extensions;

public static class Extensions
{
    /// <summary>
    /// Zaregistruje všechny důležité služby.
    /// </summary>
    /// <param name="builder">Stavitel aplikace.</param>
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddOptions<BackgroundTaskOptions>()
            .BindConfiguration(nameof(BackgroundTaskOptions));

        builder.Services.AddSingleton<IAgregatorService, AgregatorService>();
        builder.Services.AddScoped<IOrderDatabaseService, OrderDatabaseService>();

        builder.Services.AddHostedService<AgregatorManagerService>();
    }
}
