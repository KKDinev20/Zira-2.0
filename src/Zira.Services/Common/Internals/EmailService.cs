using System;
using System.Threading.Tasks;
using Essentials.Results;
using Microsoft.Extensions.DependencyInjection;
using Zira.Services.Common.Constants;
using Zira.Services.Common.Contracts;
using Zira.Services.Common.Internals.EmailSenders;
using Zira.Services.Common.Models;

namespace Zira.Services.Common.Internals;

internal class EmailService : IEmailService
{
    private readonly IServiceProvider _serviceProvider;

    public EmailService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<StandardResult> SendEmailAsync(EmailModel model, string senderStrategy)
    {
        try
        {
            var sender = ResolveEmailSender(senderStrategy);

            if (sender == null)
            {
                return StandardResult
                    .UnsuccessfulResult($"No registered email sender found for strategy: '{senderStrategy}'");
            }

            return await sender.SendEmailAsync(model);
        }
        catch (Exception ex)
        {
            return StandardResult
                .UnsuccessfulResult($"Error occurred while sending email: {ex.Message}");
        }
    }

    private IEmailSender? ResolveEmailSender(string senderStrategy)
    {
        return senderStrategy switch
        {
            EmailSenderStrategies.SendGrid => _serviceProvider.GetService<SendGridSender>(),
            EmailSenderStrategies.NoOps => _serviceProvider.GetService<NoOpsSender>(),
            _ => null
        };
    }
}