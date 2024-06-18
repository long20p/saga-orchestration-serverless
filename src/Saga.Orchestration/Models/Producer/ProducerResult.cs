using System.Text;
using Azure.Messaging.EventHubs;

namespace Saga.Orchestration.Models.Producer
{
    public class ProducerResult
    {
        public bool Valid { get; set; } = true;
        public string Message { get; set; } = string.Empty;
    }
}
