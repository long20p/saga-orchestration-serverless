using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Saga.Common.Repository;
using Saga.Functions.Factories;
using Saga.Functions.Models;
using Saga.Orchestration.Models.Transaction;

namespace Saga.Functions.Services
{
    public class SagaStatusCheckerService
    {
        private readonly HttpClient httpClient;
        private readonly IRepositoryClient<TransactionItem> repositoryClient;

        public SagaStatusCheckerService(HttpClient httpClient, IRepositoryClient<TransactionItem> repositoryClient)
        {
            this.httpClient = httpClient;
            this.repositoryClient = repositoryClient;
        }

        [FunctionName(nameof(SagaStatusChecker))]
        public async Task<IActionResult> SagaStatusChecker(
           [HttpTrigger(AuthorizationLevel.Function, methods: "get", Route = "saga/state/{id}")] HttpRequestMessage request,
           string id,
           [DurableClient] IDurableOrchestrationClient client,
           ILogger log)
        {
            TransactionItem item = await repositoryClient.GetAsync(id);

            if (item == null)
            {
                return new NotFoundObjectResult($@"Saga with transaction ID = {id} not found.");
            }

            HttpResponseMessage clientResponse = client.CreateCheckStatusResponse(request, id);

            var durableOrchestratorFactory = new DurableClientFactory(httpClient, log);
            var orchestratorRuntimeStatus = await durableOrchestratorFactory.GetRuntimeStatusAsync(clientResponse);
            var response = new SagaStatusResponse(item.State.ToString(), orchestratorRuntimeStatus);

            return new OkObjectResult(response);
        }
    }
}
