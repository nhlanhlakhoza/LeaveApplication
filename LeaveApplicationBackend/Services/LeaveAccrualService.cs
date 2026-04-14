using LeaveApplicationBackend.Models;
using LeaveApplicationBackend.Repositories;
using LeaveApplicationBackend.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class LeaveAccrualService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private DateTime _lastRunDate = DateTime.MinValue;
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

            // check every 30 seconds (safe & lightweight)
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
    private DateTime _lastAccrualRun = DateTime.MinValue;

    private async Task CheckMonthlyAccrual()
    {
        var now = DateTime.Now;

        // run once per month (e.g. 1st of month)
        if (now.Day == 1 && _lastAccrualRun.Month != now.Month)
        {
            _lastAccrualRun = now;

            await ProcessAccrual();
        }
    }
    private DateTime _lastWeeklyRun = DateTime.MinValue;

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
    private async Task ProcessWeeklyReport()
    {
        using var scope = _serviceProvider.CreateScope();

        var leaveService = scope.ServiceProvider.GetRequiredService<ILeaveService>();

        await leaveService.SendWeeklyLeaveReport();
    }
  

    private bool IsTimeToRun()
    {
        var now = DateTime.Now;

        if (now.DayOfWeek == DayOfWeek.Monday &&
            now.Hour == 6 &&   // 🔥 changed to 06:00
            now.Minute == 0 &&
            _lastRunDate.Date != now.Date)
        {
            _lastRunDate = now;
            return true;
        }

        return false;
    }
}