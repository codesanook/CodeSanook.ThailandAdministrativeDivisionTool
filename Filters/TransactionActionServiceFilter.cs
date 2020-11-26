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
                var actionExecutedContext = await next();
                if (!IsResultValid(context, actionExecutedContext))
                {
                    throw new InvalidOperationException("Invalid action result should be rollback");
                }
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                if (!(ex is InvalidOperationException))
                {
                    logger.LogError(ex, $"Tranaction commit exception in {nameof(TransactionActionServiceFilter)}");
                }

                // Attempt to roll back the transaction.
                try
                {
                    await transaction.RollbackAsync();
                }
                catch (Exception rollbackException)
                {
                    // This catch block will handle any errors that may have occurred
                    // on the server that would cause the rollback to fail, such as
                    // a closed connection.
                    logger.LogError(rollbackException, $"rollback exception in {nameof(TransactionActionServiceFilter)}");
                }
            }
        }

        private static bool IsResultValid(ActionExecutingContext context, ActionExecutedContext actionExecutedContext)
        {
            var noUnhandledException = actionExecutedContext.Exception == null || actionExecutedContext.ExceptionHandled;
            return noUnhandledException && context.ModelState.IsValid;
        }
    }
}
