using LeaveApplicationBackend.Models;
using LeaveApplicationBackend.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class LeaveAccrualService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public LeaveAccrualService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await ProcessAccrual();

            // 🔥 runs once every 24 hours
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }

    private async Task ProcessAccrual()
    {
        using var scope = _serviceProvider.CreateScope();

        var userRepo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var leaveRepo = scope.ServiceProvider.GetRequiredService<ILeaveRepository>();

        var users = await userRepo.GetAllUsers();

        foreach (var user in users)
        {
            var balance = await leaveRepo.GetBalance(user.Email);

            if (balance == null)
                continue;

            int years = GetYearsOfService(user);

            double accrual = years >= 5 ? 1.6 : 1.25;

            //  add leave
            balance.AnnualLeave += accrual;
            balance.LastUpdated = DateTime.Now;

            await leaveRepo.UpdateBalance(balance);
        }
    }

    private int GetYearsOfService(User user)
    {
        int years = DateTime.Now.Year - user.HireDate.Year;

        if (user.HireDate.Date > DateTime.Now.AddYears(-years))
            years--;

        return years;
    }
}