using System;
using System.Threading.Tasks;
using Essentials.Results;
using Microsoft.Extensions.DependencyInjection;
using Zira.Services.Common.Contracts;
using Zira.Services.Common.Models;

namespace Zira.Services.Common.Internals;

internal class EmailService : IEmailService
{
    private readonly IServiceProvider serviceProvider;

    public EmailService(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public async Task<StandardResult> SendEmailAsync(EmailModel model, string senderStrategy)
    {
        var sender = this.serviceProvider.GetKeyedService<IEmailSender>(senderStrategy);

        if (sender == null)
        {
            return StandardResult
                .UnsuccessfulResult($"There is no registered email sender strategy: '{senderStrategy}'");
        }

        return await sender.SendEmailAsync(model);
    }
}