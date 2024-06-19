using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Saga.Common.Repository;
using Saga.Orchestration.Models;
using Saga.Orchestration.Models.Transaction;
using Saga.Orchestration.Utils;

namespace Saga.Functions.Services.Activities
{
    public static class OrchestratorActivity
    {
        [FunctionName(nameof(SagaOrchestratorActivity))]
        public static async Task<TransactionItem> SagaOrchestratorActivity(
          [ActivityTrigger] TransactionItem item,
          [CosmosDB(
            databaseName: @"%CosmosDbDatabaseName%",
            containerName: @"%CosmosDbOrchestratorCollectionName%",
            Connection = @"CosmosDbConnectionString")]
            IAsyncCollector<TransactionItem> documentCollector,
            IRepositoryClient<TransactionItem> repositoryClient
            )
        {
            if (item.State == SagaState.Pending.ToString())
            {
                await documentCollector.AddAsync(item);
                return item;
            }

            return await repositoryClient.UpdateAsync(item.Id, item);
        }
    }
}
