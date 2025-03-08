using System;
using System.Linq;
using Zira.Data;
using Zira.Services.Common.Contracts;

namespace Zira.Services.Common.Internals;

public class IdGenerationService : IIdGenerationService
{
    private readonly EntityContext context;

    public IdGenerationService(EntityContext context)
    {
        this.context = context;
    }

    public string GenerateDigitIdAsync()
    {
        string transactionId;
        bool exists;

        do
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHss");
            var random = new Random().Next(10, 99);

            transactionId = timestamp + random;

            exists = this.context.Transactions.Any(t => t.TransactionId == transactionId);
        }
        while (exists);

        return transactionId;
    }
}