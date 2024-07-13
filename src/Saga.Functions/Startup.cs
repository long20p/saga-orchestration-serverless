using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Saga.Common.Repository;
using Saga.Functions.Repository;
using Saga.Orchestration.Models.Transaction;

[assembly: FunctionsStartup(typeof(Saga.Functions.Startup))]

namespace Saga.Functions
{
    /// <summary>
    /// Startup class.
    /// </summary>
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient();
            builder.Services.AddSingleton<IRepositoryClient<TransactionItem>, OrchestratorRepositoryClient>();
        }
    }
}
