using System.Threading.Tasks;
using Essentials.Results;
using Zira.Services.Common.Models;


namespace Zira.Services.Common.Contracts;

public interface IEmailService
{
    Task<StandardResult> SendEmailAsync(EmailModel model, string senderStrategy);
    
}