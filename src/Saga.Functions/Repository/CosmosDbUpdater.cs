﻿using Microsoft.Azure.Cosmos;
using Saga.Common.Repository;
using Saga.Orchestration.Models.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saga.Functions.Repository
{
    public class CosmosDbUpdater : IRepositoryUpdater<TransactionItem>
    {
        private static CosmosClient cosmosClient = new CosmosClient(Environment.GetEnvironmentVariable("CosmosDbConnectionString"));

        public async Task<TransactionItem> Update(string id, TransactionItem latest)
        {
            var container = cosmosClient.GetContainer(
                Environment.GetEnvironmentVariable("CosmosDbDatabaseName"),
                Environment.GetEnvironmentVariable("CosmosDbOrchestratorCollectionName"));

            var response = await container.ReadItemAsync<TransactionItem>(latest.Id, new PartitionKey(latest.Id));
            TransactionItem transactionItem = response;
            transactionItem.State = latest.State;
            await container.ReplaceItemAsync(transactionItem, transactionItem.Id, new PartitionKey(transactionItem.Id));

            return transactionItem;
        }
    }
}
