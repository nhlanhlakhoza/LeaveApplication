using LeaveApplicationBackend.Data;
using LeaveApplicationBackend.Services;
using LeaveApplicationBackend;
using LeaveApplicationBackend.Repositories;
using Microsoft.EntityFrameworkCore;
using backendMpact.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddControllers();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

WebApplication app;
try
{
    app = builder.Build();
}
catch (InvalidOperationException ex)
{
    // If Build() was already called (for example when multiple startup projects are launched)
    // fail gracefully so the debugger doesn't crash with an unhandled exception.
    Console.WriteLine("Warning: WebApplicationBuilder.Build() called more than once. Exiting.");
    Console.WriteLine(ex.Message);
    Environment.Exit(0);
    throw; // unreachable, but keeps compiler happy about definite assignment
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();