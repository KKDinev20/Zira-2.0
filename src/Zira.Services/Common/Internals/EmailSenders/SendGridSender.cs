using System;
using System.Threading.Tasks;
using Essentials.Results;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using Zira.Services.Common.Contracts;
using Zira.Services.Common.Models;
using Zira.Services.Common.Options;

namespace Zira.Services.Common.Internals.EmailSenders;

internal class SendGridSender : IEmailSender
{
    private readonly IOptions<EmailSendGridOptions> optionsAccessor;

    public SendGridSender(
        IOptions<EmailSendGridOptions> optionsAccessor
        )
    {
        this.optionsAccessor = optionsAccessor;
    }
    
    public async Task<StandardResult> SendEmailAsync(EmailModel model)
    {
        try
        {
            var options = optionsAccessor.Value;
            var client = new SendGridClient(options.ApiKey);
            var from = new EmailAddress(options.Email, options.Name);
            var to = new EmailAddress(model.Email);
            var message = MailHelper.CreateSingleEmail(from, to, model.Subject, plainTextContent: null, model.Message);
            var response = await client.SendEmailAsync(message);
            return StandardResult.ResultFrom(response.IsSuccessStatusCode);
        }
        catch (Exception ex)
        {
            var errorMessage = "An error occured while sending email via SendGrid.";
            return StandardResult.UnsuccessfulResult(errorMessage);
        }
    }
}