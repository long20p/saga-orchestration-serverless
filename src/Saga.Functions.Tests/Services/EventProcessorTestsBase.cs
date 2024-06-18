using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Azure.Messaging.EventHubs;
using Newtonsoft.Json;
using Saga.Common.Enums;
using Saga.Common.Events;
using Saga.Common.Messaging;

namespace Saga.Functions.Tests.Services
{
    public class EventProcessorTestsBase
    {
        public static IEnumerable<object[]> InputData => new List<object[]>
        {
            new object[] {
                CreateEvents()
            }
        };

        private static string[] CreateEvents()
        {
            var eventsData = new string[]
            {
                CreateDefaultEvent(),
                CreateInvalidAccountEvent(),
                CreateInvalidAmountEvent(),
                CreateInvalidTransactionEvent(),
                CreateOtherReasonReceiptFailedEvent(),
                CreateOtherReasonTransferFailedEvent(),
                CreateOtherReasonValidationFailedEvent(),
                CreateReceiptIssuedEvent(),
                CreateTransferCanceledEvent(),
                CreateTransferNotCanceledEvent(),
                CreateTransferSucceededEvent(),
                CreateTransferValidatedEvent()
            };

            return eventsData;
        }

        private static string CreateDefaultEvent()
        {
            var defaultEvent = new DefaultEvent
            {
                Header = CreateDefaultMessageHeader(nameof(DefaultEvent), nameof(Sources.Processor))
            };
            return CreateEventData(defaultEvent);
        }

        private static string CreateInvalidAccountEvent()
        {
            var invalidAccountEvent = new InvalidAccountEvent
            {
                Header = CreateDefaultMessageHeader(nameof(InvalidAccountEvent), nameof(Sources.Validator)),
                Content = new InvalidAccountEventContent()
            };

            return CreateEventData(invalidAccountEvent);
        }

        private static string CreateInvalidAmountEvent()
        {
            var invalidAmountEvent = new InvalidAmountEvent
            {
                Header = CreateDefaultMessageHeader(nameof(InvalidAmountEvent), nameof(Sources.Validator)),
                Content = new InvalidAmountEventContent()
            };

            return CreateEventData(invalidAmountEvent);
        }

        private static string CreateInvalidTransactionEvent()
        {
            var invalidTransactionEvent = new InvalidTransactionEvent
            {
                Header = CreateDefaultMessageHeader(nameof(InvalidTransactionEvent), nameof(Sources.Validator)),
                Content = new InvalidTransactionEventContent()
            };

            return CreateEventData(invalidTransactionEvent);
        }

        private static string CreateOtherReasonReceiptFailedEvent()
        {
            var otherReasonReceiptFailed = new OtherReasonReceiptFailedEvent
            {
                Header = CreateDefaultMessageHeader(nameof(OtherReasonReceiptFailedEvent), nameof(Sources.Receipt)),
                Content = new OtherReasonReceiptFailedEventContent()
            };

            return CreateEventData(otherReasonReceiptFailed);
        }

        private static string CreateOtherReasonTransferFailedEvent()
        {
            var otherReasonTransferFailedEvent = new OtherReasonTransferFailedEvent
            {
                Header = CreateDefaultMessageHeader(nameof(OtherReasonTransferFailedEvent), nameof(Sources.Transfer)),
                Content = new OtherReasonTransferFailedEventContent()
            };

            return CreateEventData(otherReasonTransferFailedEvent);
        }

        private static string CreateOtherReasonValidationFailedEvent()
        {
            var otherReasonValidationFailed = new OtherReasonValidationFailedEvent
            {
                Header = CreateDefaultMessageHeader(nameof(OtherReasonValidationFailedEvent), nameof(Sources.Validator)),
                Content = new OtherReasonValidationFailedEventContent()
            };

            return CreateEventData(otherReasonValidationFailed);
        }

        private static string CreateReceiptIssuedEvent()
        {
            var receiptIssuedEvent = new ReceiptIssuedEvent
            {
                Header = CreateDefaultMessageHeader(nameof(ReceiptIssuedEvent), nameof(Sources.Receipt)),
                Content = new ReceiptIssuedEventContent()
            };

            return CreateEventData(receiptIssuedEvent);
        }

        private static string CreateTransferCanceledEvent()
        {
            var transferCanceledEvent = new TransferCanceledEvent
            {
                Header = CreateDefaultMessageHeader(nameof(TransferCanceledEvent), nameof(Sources.Transfer))
            };

            return CreateEventData(transferCanceledEvent);
        }

        private static string CreateTransferNotCanceledEvent()
        {
            var transferNotCanceledEvent = new TransferNotCanceledEvent
            {
                Header = CreateDefaultMessageHeader(nameof(TransferNotCanceledEvent), nameof(Sources.Transfer)),
                Content = new TransferNotCanceledEventContent()
            };

            return CreateEventData(transferNotCanceledEvent);
        }

        private static string CreateTransferSucceededEvent()
        {
            var transferSucceededEvent = new TransferSucceededEvent
            {
                Header = CreateDefaultMessageHeader(nameof(TransferSucceededEvent), nameof(Sources.Transfer))
            };

            return CreateEventData(transferSucceededEvent);
        }

        private static string CreateTransferValidatedEvent()
        {
            var transferValidatedEvent = new TransferValidatedEvent
            {
                Header = CreateDefaultMessageHeader(nameof(TransferValidatedEvent), nameof(Sources.Validator))
            };

            return CreateEventData(transferValidatedEvent);
        }

        protected static string[] CreateInvalidEventsData()
        {
            var defaultEvent = new DefaultEvent
            {
                Header = null
            };

            return new string[]
            {
                CreateEventData(defaultEvent)
            };
        }

        //private static string CreateEventData(Event sagaEvent)
        //{
        //    string serializedMsg = JsonConvert.SerializeObject(sagaEvent);
        //    byte[] messageBytes = Encoding.UTF8.GetBytes(serializedMsg);

        //    return new EventData(messageBytes);
        //}

        private static string CreateEventData(Event sagaEvent)
        {
            return JsonConvert.SerializeObject(sagaEvent);
        }

        private static MessageHeader CreateDefaultMessageHeader(string messageType, string source)
        {
            return new MessageHeader(Guid.NewGuid().ToString(), messageType, source);
        }
    }
}
