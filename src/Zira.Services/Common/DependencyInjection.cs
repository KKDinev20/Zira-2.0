using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Zira.Services.Common.Constants;
using Zira.Services.Common.Contracts;
using Zira.Services.Common.Internals;
using Zira.Services.Common.Internals.EmailSenders;
using Zira.Services.Common.Options;

namespace Zira.Services.Common;

public static class DependencyInjection
{
    public static IServiceCollection AddCommonServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (configuration.GetSection("Emails:SendGrid").GetValue<bool>("Enabled"))
        {
            services.Configure<EmailSendGridOptions>(configuration.GetSection("Emails:SendGrid"));
            services.AddKeyedScoped<IEmailSender, SendGridSender>(EmailSenderStrategies.SendGrid);
        }
        
        services.AddScoped<IEmailService, EmailService>();
        services.AddKeyedScoped<IEmailSender, NoOpsSender>(EmailSenderStrategies.NoOps);
        
        return services;
    }
}