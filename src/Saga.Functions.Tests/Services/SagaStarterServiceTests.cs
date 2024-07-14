using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Saga.Functions.Models;
using Saga.Functions.Services;
using Saga.Orchestration.Models.Transaction;
using Xunit;

namespace Saga.Functions.Tests.Services
{
    public class SagaStarterServiceTests
    {
        [Fact]
        public async Task Valid_request_should_provide_transaction_id()
        {
            var loggerMock = new Mock<ILogger>();
            var clientMock = new Mock<IDurableOrchestrationClient>();

            var item = new TransactionItem
            {
                AccountFromId = Guid.NewGuid().ToString(),
                AccountToId = Guid.NewGuid().ToString(),
                Amount = 100.00M
            };

            var httpRequest = new HttpRequestMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(item))
            };

            clientMock.
              Setup(x => x.StartNewAsync(nameof(Orchestrator.SagaOrchestrator), It.IsAny<string>(), It.IsAny<TransactionItem>())).
              ReturnsAsync(item.Id);

            IActionResult result = await SagaStarterService
                .SagaStarter(httpRequest, clientMock.Object, loggerMock.Object);

            var okObjectResult = result as OkObjectResult;
            Assert.NotNull(okObjectResult);

            var sagaResponse = okObjectResult.Value as SagaStarterResponse;
            Assert.NotNull(sagaResponse);
            Assert.NotNull(sagaResponse.TransactionId);
            Assert.NotEmpty(sagaResponse.TransactionId);
        }

        [Fact]
        public async Task Invalid_request_input_should_not_produce_transaction_id()
        {
            var loggerMock = new Mock<ILogger>();
            var clientMock = new Mock<IDurableOrchestrationClient>();
            var httpRequest = new HttpRequestMessage
            {
                Content = new StringContent(string.Empty)
            };

            IActionResult result = await SagaStarterService
                .SagaStarter(httpRequest, clientMock.Object, loggerMock.Object);

            var badRequestResult = result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
        }

        [Theory]
        [InlineData("", "idTo123", 100.00)]
        [InlineData("idFrom123", "", 100.00)]
        [InlineData("", "", 100.00)]
        [InlineData("idFrom123", "idTo123", 0.00)]
        [InlineData("idFrom123", "idTo123", -1.00)]
        [InlineData("", "", -1.00)]
        [InlineData(null, "idTo123", 100.00)]
        [InlineData("idFrom123", null, 100.00)]
        [InlineData(null, null, 100.00)]
        [InlineData(null, null, -1.00)]
        public async Task Invalid_request_payload_should_not_provide_transaction_id(
            string accountFromId, string accountToId, decimal amount)
        {
            var loggerMock = new Mock<ILogger>();
            var clientMock = new Mock<IDurableOrchestrationClient>();

            var item = new TransactionItem
            {
                AccountFromId = accountFromId,
                AccountToId = accountToId,
                Amount = amount
            };

            var httpRequest = new HttpRequestMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(item))
            };

            clientMock.
              Setup(x => x.StartNewAsync(nameof(Orchestrator.SagaOrchestrator), It.IsAny<string>(), It.IsAny<TransactionItem>())).
              ReturnsAsync(item.Id);

            IActionResult result = await SagaStarterService
                .SagaStarter(httpRequest, clientMock.Object, loggerMock.Object);

            var badRequestResult = result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
        }
    }
}
