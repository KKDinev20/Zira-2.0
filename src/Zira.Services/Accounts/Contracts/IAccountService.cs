using System;
using System.Threading.Tasks;

namespace Zira.Services.Accounts.Contracts;

public interface IAccountService
{
    Task CreateAccountServiceAsync(Guid identityUserId);
}