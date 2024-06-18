using Azure.Messaging.EventHubs;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using Saga.Common.Messaging;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Saga.Functions.Messaging
{
    public class EventHubMessageProducer : IMessageProducer
    {
        private readonly IAsyncCollector<string> eventCollector;

        public EventHubMessageProducer(IAsyncCollector<string> eventCollector)
        {
            this.eventCollector = eventCollector;
        }

        public async Task ProduceAsync(object message)
        {
            var eventData = CreateMessage(message);
            await eventCollector.AddAsync(eventData);
        }

        [Obsolete]
        private EventData CreateEventData(object message)
        {
            string serializedMsg = JsonConvert.SerializeObject(message);
            byte[] messageBytes = Encoding.UTF8.GetBytes(serializedMsg);

            return new EventData(messageBytes);
        }

        private string CreateMessage(object message)
        {
            return JsonConvert.SerializeObject(message);
        }
    }
}
