using System;
using System.Collections.Generic;
using System.Text;
using Azure.Messaging.EventHubs;
using Newtonsoft.Json;
using Saga.Common.Commands;
using Saga.Common.Enums;
using Saga.Common.Messaging;
using Saga.Common.Models.Transaction;

namespace Saga.Functions.Tests.Services.Participants
{
    public class TransferServiceTestsBase
    {
        public static IEnumerable<object[]> TransferFunctionInputData => new List<object[]>
        {
            new object[] {
                CreateTransferEventsData()
            },
            new object[] {
                CreateCancelTransferCommandEventsData()
            }
        };

        private static string[] CreateTransferEventsData()
        {
            var command = new TransferCommand
            {
                Header = new MessageHeader(Guid.NewGuid().ToString(), nameof(TransferCommand), nameof(Sources.Orchestrator)),
                Content = new TransferCommandContent
                {
                    Transaction = new TransactionDetails
                    {
                        AccountFromId = Guid.NewGuid().ToString(),
                        AccountToId = Guid.NewGuid().ToString(),
                        Amount = 100.00M
                    }
                }
            };

            return CreateEventsData(command);
        }

        private static string[] CreateCancelTransferCommandEventsData()
        {
            var command = new CancelTransferCommand
            {
                Header = new MessageHeader(Guid.NewGuid().ToString(), nameof(CancelTransferCommand), nameof(Sources.Orchestrator)),
                Content = new CancelTransferCommandContent
                {
                    Transaction = new TransactionDetails
                    {
                        AccountFromId = Guid.NewGuid().ToString(),
                        AccountToId = Guid.NewGuid().ToString(),
                        Amount = 100.00M
                    }
                }
            };

            return CreateEventsData(command);
        }

        private static string[] CreateEventsData(Command command)
        {
            string serializedMsg = JsonConvert.SerializeObject(command);

            return new string[]
            {
                serializedMsg
            };
        }
    }
}
