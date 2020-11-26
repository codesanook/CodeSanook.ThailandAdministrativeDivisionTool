using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using OrchardCore.EF;

namespace Codesanook.ThailandAdministrativeDivisionTool.Filters
{
    public class TransactionActionServiceFilter : IAsyncActionFilter
    {
        private readonly OrchardDbContext dbContext;
        private readonly ILogger<TransactionActionServiceFilter> logger;

        public TransactionActionServiceFilter(OrchardDbContext dbContext, ILogger<TransactionActionServiceFilter> logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                var resultContext = await next();
                var noUnhandledException = resultContext.Exception == null || resultContext.ExceptionHandled;
                if (noUnhandledException && context.ModelState.IsValid)
                {
                    await transaction.CommitAsync();
                }
                else
                {
                    await transaction.RollbackAsync();
                }
            }
            catch (Exception commitException)
            {
                logger.LogError(ex, $"Exception in {nameof(TransactionActionServiceFilter)}");

// Attempt to roll back the transaction.
            try
            {
                transaction.Rollback();
            }
            catch (Exception ex2)
            {
                // This catch block will handle any errors that may have occurred
                // on the server that would cause the rollback to fail, such as
                // a closed connection.
                Console.WriteLine("Rollback Exception Type: {0}", ex2.GetType());
                Console.WriteLine("  Message: {0}", ex2.Message);
            }

                await transaction.RollbackAsync();
            }
        }
    }
}
