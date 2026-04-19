using LeaveApplicationBackend.Models;
using LeaveApplicationBackend.Repositories;
using LeaveApplicationBackend.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class LeaveAccrualService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    private DateTime _lastAccrualRun = DateTime.MinValue;
    private DateTime _lastWeeklyRun = DateTime.MinValue;

    public LeaveAccrualService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await CheckMonthlyAccrual();
            await CheckWeeklyReport();

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }

   
    // MONTHLY ACCRUAL
    
    private async Task CheckMonthlyAccrual()
    {
        var now = DateTime.Now;

        if (now.Day == 1 && _lastAccrualRun.Month != now.Month)
        {
            _lastAccrualRun = now;
            await ProcessAccrual();
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
            var balance = await leaveRepo.GetBalanceByUserId(user.Id);

            if (balance == null)
                continue;

            int years = GetYearsOfService(user);

            double accrual = years >= 5 ? 1.6 : 1.25;

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

   
    // WEEKLY REPORT (MONDAY 06:00)
   
    private async Task CheckWeeklyReport()
    {
        var now = DateTime.Now;

        if (now.DayOfWeek == DayOfWeek.Monday &&
            now.Hour == 6 &&
            now.Minute == 0 &&
            _lastWeeklyRun.Date != now.Date)
        {
            _lastWeeklyRun = now;
            await ProcessWeeklyReport();
        }
    }

    private async Task ProcessWeeklyReport()
    {
        using var scope = _serviceProvider.CreateScope();

        var leaveService = scope.ServiceProvider.GetRequiredService<ILeaveService>();

        await leaveService.SendWeeklyLeaveReport();
    }
}