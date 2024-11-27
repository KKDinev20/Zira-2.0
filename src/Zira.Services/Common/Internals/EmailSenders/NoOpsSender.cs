using System.Threading.Tasks;
using Zira.Services.Common.Contracts;
using Zira.Services.Common.Models;
using Essentials.Results;
using Microsoft.Extensions.Logging;

namespace Zira.Services.Common.Internals.EmailSenders;

internal class NoOpsSender : IEmailSender
{
    private readonly ILogger<NoOpsSender> logger;

    public NoOpsSender(ILogger<NoOpsSender> logger)
    {
        this.logger = logger;
    }
    
    public Task<StandardResult> SendEmailAsync(EmailModel model)
    {
        this.logger.LogDebug("Sending email via NoOps");
        this.logger.LogDebug($"Recipient: {model.Email}");
        this.logger.LogDebug($"Subject: {model.Subject}");
        this.logger.LogDebug($"Message: {model.Message}");
        
        return Task.FromResult(StandardResult.SuccessfulResult());
    }
}