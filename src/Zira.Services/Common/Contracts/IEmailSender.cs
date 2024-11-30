using System.Threading.Tasks;
using Essentials.Results;
using Zira.Services.Common.Models;

namespace Zira.Services.Common.Contracts;

internal interface IEmailSender
{
    Task<StandardResult> SendEmailAsync(EmailModel model);
}