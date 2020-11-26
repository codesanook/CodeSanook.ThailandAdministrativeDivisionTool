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
            catch (Exception ex)
            {
                logger.LogError(ex, $"Exception in {nameof(TransactionActionServiceFilter)}");
                await transaction.RollbackAsync();
            }
        }
    }
}
