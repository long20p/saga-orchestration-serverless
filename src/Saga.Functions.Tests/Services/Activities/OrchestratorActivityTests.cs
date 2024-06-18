using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Moq;
using Saga.Common.Repository;
using Saga.Functions.Services.Activities;
using Saga.Orchestration.Models;
using Saga.Orchestration.Models.Transaction;
using Xunit;

namespace Saga.Functions.Tests.Services.Activities
{
    public class OrchestratorActivityTests : OrchestratorActivityTestsBase
    {
        [Fact]
        public async Task Pending_saga_state_should_be_persisted_on_database()
        {
            var documentCollectorMock = new Mock<IAsyncCollector<TransactionItem>>();
            var repositoryUpdaterMock = new Mock<IRepositoryUpdater<TransactionItem>>();

            var item = new TransactionItem
            {
                Id = Guid.NewGuid().ToString(),
                AccountFromId = Guid.NewGuid().ToString(),
                AccountToId = Guid.NewGuid().ToString(),
                Amount = 100.00M,
                State = nameof(SagaState.Pending)
            };

            documentCollectorMock
                .Setup(x => x.AddAsync(It.IsAny<TransactionItem>(), default))
                .Returns(
                    Task.CompletedTask
                );

            TransactionItem resultItem = await OrchestratorActivity
                .SagaOrchestratorActivity(item, documentCollectorMock.Object, repositoryUpdaterMock.Object);

            Assert.Equal(item.Id, resultItem.Id);
        }

        [Theory]
        [MemberData(nameof(OrchestratorActivityInputData))]
        public async Task Saga_states_should_be_updated_on_database(TransactionItem item, TransactionItem newItem)
        {
            var documentCollectorMock = new Mock<IAsyncCollector<TransactionItem>>();
            var repositoryUpdaterMock = new Mock<IRepositoryUpdater<TransactionItem>>();
            
            repositoryUpdaterMock.Setup(x => x.Update(It.IsAny<string>(), It.IsAny<TransactionItem>()))
                .ReturnsAsync(newItem);

            var documents = new List<Document>
            {
                new Document
                {
                    Id = item.Id
                }
            };

            TransactionItem resultItem = await OrchestratorActivity
                .SagaOrchestratorActivity(newItem, documentCollectorMock.Object, repositoryUpdaterMock.Object);

            Assert.NotNull(resultItem);
            Assert.Equal(newItem.Id, resultItem.Id);
            Assert.Equal(newItem.State, resultItem.State);
        }
    }
}
