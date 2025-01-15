using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Zira.Data;
using Zira.Data.Models;
using Zira.Services.Accounts.Contracts;

namespace Zira.Services.Accounts.Internals;

public class AccountService : IAccountService
{
    private readonly EntityContext context;
    private readonly ILogger<AccountService> logger;

    public AccountService(
        EntityContext context,
        ILogger<AccountService> logger)
    {
        this.context = context;
        this.logger = logger;
    }

    public async Task CreateAccountServiceAsync(Guid identityUserId)
    {
        try
        {
            this.context.Users.Add(new User
            {
                Id = identityUserId,
            });

            await this.context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "An error occured while creating profile");
        }
    }
}