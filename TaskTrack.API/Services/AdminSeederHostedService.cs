using TaskTrack.Service.Interfaces;

namespace TaskTrack.API.Services;

public sealed class AdminSeederHostedService : IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;

    public AdminSeederHostedService(IServiceScopeFactory scopeFactory, IConfiguration configuration)
    {
        _scopeFactory = scopeFactory;
        _configuration = configuration;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var accountService = scope.ServiceProvider.GetRequiredService<IAccountService>();
        var email = _configuration["SeedAdmin:Email"] ?? "admin@system.com";
        var password = _configuration["SeedAdmin:Password"] ?? "Admin@123";
        await accountService.EnsureSeedAdminAsync(email, password);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}