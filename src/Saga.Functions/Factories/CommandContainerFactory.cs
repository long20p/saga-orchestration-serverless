using Azure.Messaging.EventHubs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Saga.Common.Processors;
using System.Text;

namespace Saga.Functions.Factories
{
    public static class CommandContainerFactory
    {
        public static CommandContainer BuildCommandContainer(EventData eventData)
        {
            var jsonBody = Encoding.UTF8.GetString(eventData.Body.ToArray());
            var jObject = JsonConvert.DeserializeObject<JObject>(jsonBody);
            return new CommandContainer(jObject);
        }
    }
}
