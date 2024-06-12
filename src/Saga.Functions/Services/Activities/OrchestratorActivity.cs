using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Saga.Orchestration.Models;
using Saga.Orchestration.Models.Transaction;
using Saga.Orchestration.Utils;

namespace Saga.Functions.Services.Activities
{
    public static class OrchestratorActivity
    {
        private static CosmosClient cosmosClient = new CosmosClient(Environment.GetEnvironmentVariable("CosmosDbConnectionString"));

        [FunctionName(nameof(SagaOrchestratorActivity))]
        public static async Task<TransactionItem> SagaOrchestratorActivity(
          [ActivityTrigger] TransactionItem item,
          [CosmosDB(
            databaseName: @"%CosmosDbDatabaseName%",
            containerName: @"%CosmosDbOrchestratorCollectionName%",
            Connection = @"CosmosDbConnectionString")]
            IAsyncCollector<TransactionItem> documentCollector
          //[CosmosDB(
          //  databaseName: @"%CosmosDbDatabaseName%",
          //  containerName: @"%CosmosDbOrchestratorCollectionName%",
          //  Connection = @"CosmosDbConnectionString")] IDocumentClient client
            )
        {
            if (item.State == SagaState.Pending.ToString())
            {
                await documentCollector.AddAsync(item);
                return item;
            }

            //Uri collectionUri = UriUtils.CreateTransactionCollectionUri();

            //var document = client
            //    .CreateDocumentQuery(collectionUri)
            //    .Where(t => t.Id == item.Id)
            //    .AsEnumerable()
            //    .FirstOrDefault();

            //document.SetPropertyValue("state", item.State);
            //await client.ReplaceDocumentAsync(document);

            var container = cosmosClient.GetContainer(
                Environment.GetEnvironmentVariable("CosmosDbDatabaseName"), 
                Environment.GetEnvironmentVariable("CosmosDbOrchestratorCollectionName"));

            var response = await container.ReadItemAsync<TransactionItem>(item.Id, new Microsoft.Azure.Cosmos.PartitionKey(item.Id));
            TransactionItem transactionItem = response;
            transactionItem.State = item.State;
            await container.ReplaceItemAsync(transactionItem, transactionItem.Id, new Microsoft.Azure.Cosmos.PartitionKey(transactionItem.Id));

            return transactionItem;
        }
    }
}
