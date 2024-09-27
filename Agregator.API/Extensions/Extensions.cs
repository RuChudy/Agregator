namespace Agregator.API.Extensions;

public static class Extensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddOptions<BackgroundTaskOptions>()
            .BindConfiguration(nameof(BackgroundTaskOptions));

        builder.Services.AddSingleton<IAgregatorService, AgregatorService>();

        builder.Services.AddHostedService<AgregatorManagerService>();
    }
}
