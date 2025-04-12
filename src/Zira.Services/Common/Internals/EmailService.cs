using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using Zira.Services.Common.Contracts;
using Zira.Services.Common.Models;

namespace Zira.Services.Common.Internals
{
    public class EmailService : IEmailService
    {
        private readonly string? apiKey;
        private readonly bool emailEnabled;
    
        public EmailService(IConfiguration configuration)
        {
            this.apiKey = configuration.GetValue<string>("SendGrid:ApiKey");
            this.emailEnabled = configuration.GetValue<bool>("EnableEmailSending");
    
            if (string.IsNullOrEmpty(this.apiKey) && emailEnabled)
            {
                throw new Exception("SendGrid API key is not configured.");
            }
        }
    
        public async Task SendEmailAsync(EmailModel emailModel)
        {
            if (!emailEnabled)
            {
                Console.WriteLine("Email sending disabled (dev/offline mode).");
                return;
            }
    
            try
            {
                var client = new SendGridClient(this.apiKey);
                var from = new EmailAddress("kkdinev20@codingburgas.bg");
                var to = new EmailAddress(emailModel.ToEmail);
                var msg = MailHelper.CreateSingleEmail(from, to, emailModel.Subject, emailModel.Body, emailModel.Body);
    
                var response = await client.SendEmailAsync(msg);
    
                var responseBody = await response.Body.ReadAsStringAsync();
                Console.WriteLine($@"SendGrid Response: {response.StatusCode}, {responseBody}");
    
                if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
                {
                    throw new Exception($"Error sending email. Status Code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($@"Error sending email: {ex.Message}");
                throw;
            }
        }
    }
}