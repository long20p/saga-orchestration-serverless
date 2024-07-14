using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Moq;
using Saga.Common.Repository;
using Saga.Functions.Models;
using Saga.Functions.Services;
using Saga.Orchestration.Models;
using Saga.Orchestration.Models.Transaction;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Saga.Functions.Tests.Services
{
    public class SagaStatusCheckerServiceTests : SagaStatusCheckerServiceTestsBase
    {
        [Fact]
        public async Task Request_should_return_saga_status()
        {
            var transactionId = Guid.NewGuid().ToString();
            Mock<IDurableOrchestrationClient> clientMock = CreateDurableOrchestrationMock();
            var repositoryClientMock = new Mock<IRepositoryClient<TransactionItem>>();
            HttpClient httpClient = CreateValidHttpClient();
            var loggerMock = new Mock<ILogger>();

            var item = new TransactionItem
            {
                Id = transactionId,
                AccountFromId = Guid.NewGuid().ToString(),
                AccountToId = Guid.NewGuid().ToString(),
                Amount = 100.00M,
                State = nameof(SagaState.Pending)
            };

            repositoryClientMock.Setup(x => x.GetAsync(transactionId)).ReturnsAsync(item);

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($@"http://localhost:7071/api/saga/state/{transactionId}"),
            };

            var sagaStatusService = new SagaStatusCheckerService(httpClient, repositoryClientMock.Object);
            var result = await sagaStatusService
                .SagaStatusChecker(request, transactionId, clientMock.Object, loggerMock.Object);

            var okObjectResult = result as OkObjectResult;
            Assert.NotNull(okObjectResult);

            var sagaStatusResponse = okObjectResult.Value as SagaStatusResponse;

            Assert.NotNull(sagaStatusResponse);
            Assert.NotNull(sagaStatusResponse.Status);
            Assert.NotNull(sagaStatusResponse.Status.SagaState);
            Assert.NotEmpty(sagaStatusResponse.Status.SagaState);
            Assert.NotNull(sagaStatusResponse.Status.OrchestrationEngineState);
            Assert.NotEmpty(sagaStatusResponse.Status.OrchestrationEngineState);
        }

        [Fact]
        public async Task Request_should_not_return_saga_status()
        {
            Mock<IDurableOrchestrationClient> clientMock = CreateDurableOrchestrationMock();
            HttpClient httpClient = CreateValidHttpClient();
            var repositoryClientMock = new Mock<IRepositoryClient<TransactionItem>>();
            var loggerMock = new Mock<ILogger>();

            var transactionId = Guid.NewGuid().ToString();
            var documents = new List<TransactionItem>();

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($@"http://localhost:7071/api/saga/state/{transactionId}"),
            };

            var sagaStatusService = new SagaStatusCheckerService(httpClient, repositoryClientMock.Object);
            var result = await sagaStatusService
                .SagaStatusChecker(request, transactionId, clientMock.Object, loggerMock.Object);

            Assert.NotNull(result as NotFoundObjectResult);
        }
    }
}
