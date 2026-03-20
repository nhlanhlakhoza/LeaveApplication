using LeaveApplicationBackend.Services;
using System.Net;
using System.Net.Mail;

namespace backendMpact.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        // Original method without attachments
        public async Task<bool> SendEmailAsync(string to, string subject, string body)
        {
            return await SendEmailAsync(to, subject, body, null);
        }

        // Overload with attachments
        public async Task<bool> SendEmailAsync(string to, string subject, string body, List<string>? attachmentPaths)
        {
            try
            {
                // Read settings
                string smtpHost = _config["EmailSettings:SmtpHost"];
                string smtpPortStr = _config["EmailSettings:SmtpPort"];
                string fromEmail = _config["EmailSettings:FromEmail"];
                string fromName = _config["EmailSettings:FromName"];
                string smtpPass = _config["EmailSettings:SmtpPass"];

                Console.WriteLine($"[DEBUG] Loaded SMTP config:");
                Console.WriteLine($"Host: {smtpHost}");
                Console.WriteLine($"Port: {smtpPortStr}");
                Console.WriteLine($"FromEmail: {fromEmail}");
                Console.WriteLine($"FromName: {fromName}");
                Console.WriteLine($"App Password (length only): {smtpPass?.Length}");

                if (string.IsNullOrWhiteSpace(fromEmail) || string.IsNullOrWhiteSpace(smtpPass))
                {
                    Console.WriteLine("[ERROR] Missing FromEmail OR SmtpPass in appsettings.json");
                    return false;
                }

                if (!int.TryParse(smtpPortStr, out int smtpPort))
                    smtpPort = 587; // default Gmail port

                var client = new SmtpClient(smtpHost, smtpPort)
                {
                    Credentials = new NetworkCredential(fromEmail, smtpPass),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Timeout = 15000
                };

                var msg = new MailMessage
                {
                    From = new MailAddress(fromEmail, fromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = false
                };

                msg.To.Add(to);
                msg.ReplyToList.Add(new MailAddress(fromEmail));

                // Add attachments if any
                if (attachmentPaths != null && attachmentPaths.Count > 0)
                {
                    foreach (var path in attachmentPaths)
                    {
                        if (!string.IsNullOrEmpty(path) && System.IO.File.Exists(path))
                        {
                            msg.Attachments.Add(new Attachment(path));
                        }
                    }
                }

                Console.WriteLine($"[DEBUG] Attempting to send email...");

                await client.SendMailAsync(msg);

                Console.WriteLine("[SUCCESS] Email sent successfully!");
                return true;
            }
            catch (SmtpException ex)
            {
                Console.WriteLine($"[SMTP ERROR] Status: {ex.StatusCode}, Message: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] Exception when sending email: " + ex.Message);
                return false;
            }
        }
    }
}
