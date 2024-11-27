using System.Threading.Tasks;
using Zira.Services.Common.Models;
using Essentials.Results;

namespace Zira.Services.Common.Contracts;

internal interface IEmailSender
{
    Task<StandardResult> SendEmailAsync(EmailModel model);
}