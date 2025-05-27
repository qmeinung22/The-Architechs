using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using project_wildfire_web.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public class NotificationBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<NotificationBackgroundService> _logger;
    private readonly IActiveUserTracker _activeUserTracker;

    public NotificationBackgroundService(IServiceProvider serviceProvider, ILogger<NotificationBackgroundService> logger, IActiveUserTracker activeUserTracker)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _activeUserTracker = activeUserTracker;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("🔥 NotificationBackgroundService is starting...");

        while (!stoppingToken.IsCancellationRequested)
        {
            await CheckUserLocationsAsync();

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // Run every 1 minute
        }
    }

private async Task CheckUserLocationsAsync()
{
    using (var scope = _serviceProvider.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<FireDataDbContext>();
        var notificationService = scope.ServiceProvider.GetRequiredService<NotificationService>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        var now = DateTime.UtcNow;

        var activeUserIds = _activeUserTracker.GetActiveUserIds().ToHashSet();


        if (!activeUserIds.Any())
        {
            _logger.LogInformation("No active users. Skipping notification check.");
            return;
        }

        var userLocations = await context.UserLocations
            .Where(loc => activeUserIds.Contains(loc.UserId))
            .ToListAsync();

        foreach (var location in userLocations)
        {
            var lastSent = location.LastNotificationSent ?? DateTime.MinValue;
            var intervalMinutes = location.TimeInterval == 0 ? 1 : location.TimeInterval * 60;

                if ((now - lastSent).TotalMinutes >= intervalMinutes)
                {
                    var identityUser = await userManager.FindByIdAsync(location.UserId);

                if (identityUser != null && !string.IsNullOrEmpty(identityUser.PhoneNumber))
                {
                    _logger.LogInformation($"Checking fires for active user {identityUser.Id}, location {location.Title}");
                    await notificationService.CheckFiresNearUserLocationsAsync(identityUser.Id, identityUser.PhoneNumber);
                    location.LastNotificationSent = now;
                }
            }
        }

        await context.SaveChangesAsync();
    }
}


}
