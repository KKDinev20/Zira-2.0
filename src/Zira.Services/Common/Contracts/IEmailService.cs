using System.Threading.Tasks;
using Zira.Services.Common.Models;
using Essentials.Results;

namespace Zira.Services.Common.Contracts;

public interface IEmailService
{
    Task<StandardResult> SendEmailAsync(EmailModel model, string senderStrategy);
}