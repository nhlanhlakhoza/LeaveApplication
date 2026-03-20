namespace LeaveApplicationBackend.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string to, string subject, string body, List<string>? attachmentPaths);
    }
}
