﻿namespace NServiceBus.Gateway.Tests.Idempotency.Raven
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Transactions;
    using global::Raven.Client;
    using global::Raven.Client.Document;
    using NUnit.Framework;
    using Persistence;
    using Persistence.Raven;

    public class in_the_raven_storage
    {
        protected IPersistMessages ravenPersister;
        protected IDocumentStore store;

        [SetUp]
        public void SetUp()
        {
            store = new DocumentStore
                        {
                            Url = "http://localhost:8080",
                        };

            store.Initialize();

            ravenPersister = new RavenDBPersistence(store);
        }

        [TearDown]
        public void TearDown()
        {
            store.Dispose();
        }

        protected bool Store(TestMessage message)
        {
            using (var scope = new TransactionScope())
            using (var msgStream = new MemoryStream(message.OriginalMessage))
            {
                var result = ravenPersister.InsertMessage(message.ClientId, message.TimeReceived, msgStream, message.Headers);
                scope.Complete();

                return result;
            }
        }

        protected TestMessage GetStoredMessage(string clientId)
        {
            using (var session = store.OpenSession())
            {
                var messageStored = session.Load<GatewayMessage>(RavenDBPersistence.EscapeClientId(clientId));

                return new TestMessage
                {
                    ClientId = messageStored.Id,
                    Headers = messageStored.Headers,
                    TimeReceived = messageStored.TimeReceived,
                    OriginalMessage = messageStored.OriginalMessage,
                    Acknowledged = messageStored.Acknowledged
                };
            }

        }


        protected TestMessage CreateTestMessage()
        {
            var headers = new Dictionary<string, string>();

            headers.Add("Header1", "Value1");
            headers.Add("Header2", "Value2");
            headers.Add("Header3", "49710.06:28:15");


            return new TestMessage
            {
                ClientId = Guid.NewGuid() + "\\12345",
                TimeReceived = DateTime.UtcNow,
                Headers = headers,
                OriginalMessage = new byte[] { 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8 }

            }
            ;
        }

    }


    public class TestMessage
    {
        public IDictionary<string, string> Headers { get; set; }

        public DateTime TimeReceived { get; set; }

        public string ClientId { get; set; }

        public byte[] OriginalMessage { get; set; }

        public bool Acknowledged { get; set; }
    }
}