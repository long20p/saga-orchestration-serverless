using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Saga.Common.Messaging;
using Saga.Common.Processors;
using Saga.Functions.Factories;
using Saga.Common.Repository;
using Saga.Functions.Messaging;
using Saga.Functions.Repository;
using Saga.Participants.Transfer.Factories;
using Saga.Participants.Transfer.Models;
using Azure.Messaging.EventHubs;

namespace Saga.Functions.Services.Participants
{
    public static class TransferService
    {
        [FunctionName(nameof(TransferMoney))]
        public static async Task TransferMoney(
          [EventHubTrigger(@"%TransferEventHubName%", Connection = @"EventHubsNamespaceConnection")] string[] eventsData,
          [EventHub(@"%ReplyEventHubName%", Connection = @"EventHubsNamespaceConnection")]IAsyncCollector<string> eventCollector,
          [CosmosDB(
            databaseName: @"%CosmosDbDatabaseName%",
            containerName: @"%CosmosDbTransferCollectionName%",
            Connection = @"CosmosDbConnectionString")]
          IAsyncCollector<CheckingAccountLine> stateCollector,
          ILogger logger)
        {
            IMessageProducer eventProducer = new EventHubMessageProducer(eventCollector);
            IRepository<CheckingAccountLine> repository = new CosmosDbRepository<CheckingAccountLine>(stateCollector);
            var processors = TransferServiceCommandProcessorFactory.BuildProcessorMap(eventProducer, repository);
            var dispatcher = new CommandProcessorDispatcher(processors);

            foreach (var eventData in eventsData)
            {
                try
                {
                    var commandContainer = CommandContainerFactory.BuildCommandContainer(eventData);
                    await dispatcher.ProcessCommandAsync(commandContainer);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, ex.Message);
                }
            }
        }
    }
}
