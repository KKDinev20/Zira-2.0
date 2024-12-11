using System.Threading.Tasks;
using Essentials.Results;
using Zira.Common;
using Zira.Services.Common.Contracts;
using Zira.Services.Common.Models;

namespace Zira.Services.Identity.Extensions
{
    public static class EmailServiceExtensions
    {
        public static async Task<StandardResult> SendResetPasswordEmailAsync(this IEmailService emailService, string email, string resetLink)
        {
            var subject = Emails.ResetPassword;
            var body = string.Format(Emails.ResetPassword, resetLink);

            var emailModel = new EmailModel
            {
                ToEmail = email,
                Subject = subject,
                Body = body,
            };

            try
            {
                await emailService.SendEmailAsync(emailModel);
                return StandardResult.SuccessfulResult();
            }
            catch
            {
                return StandardResult.UnsuccessfulResult("Failed to send reset password email.");
            }
        }
    }
}