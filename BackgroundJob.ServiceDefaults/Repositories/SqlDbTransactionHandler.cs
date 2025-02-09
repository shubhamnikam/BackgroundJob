using Microsoft.EntityFrameworkCore;

namespace BackgroundJob.Common.Repositories;

public static class TransactionHelper
{
    public static async Task<T> ExecuteAsync<T>(
        DbContext context,
        Func<Task<T>> operation)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));
        if (operation == null) throw new ArgumentNullException(nameof(operation));

        using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var result = await operation();
            await transaction.CommitAsync();
            return result;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public static async Task ExecuteAsync(
        DbContext context,
        Func<Task> operation)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));
        if (operation == null) throw new ArgumentNullException(nameof(operation));

        using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            await operation();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}

