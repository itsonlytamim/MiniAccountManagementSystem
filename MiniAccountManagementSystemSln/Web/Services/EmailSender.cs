using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Web.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // In a real application, implement actual email sending logic here
            // For development, we'll just log the email to the console
            System.Console.WriteLine($"Email to {email}, subject: {subject}, message: {htmlMessage}");
            return Task.CompletedTask;
        }
    }
}
