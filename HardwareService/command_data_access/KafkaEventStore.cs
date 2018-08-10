﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Autofac;
using Common;
using Common.messagebus;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using HardwareService.domain;
using HardwareService.domain.consumers;
using HardwareService.domain.events;
using MassTransit;
using MassTransit.Logging;
using Newtonsoft.Json;


namespace HardwareService.command_data_access
{
    public partial class KafkaEventStore : IEventStore
    {
        private static Producer _kafkaproducer;
        //private string topicName = "Sensors_Events";
        private Dictionary<string, object>  _config;
        private Microsoft.Extensions.Logging.ILogger _logger;

        public bool IsReady { get; internal set; }

        public KafkaEventStore(Microsoft.Extensions.Logging.ILogger logger)
        {
            _logger = logger;

            _config = new Dictionary<string, object> {
                { "bootstrap.servers", "PLAINTEXT://10.185.20.197:9092" },
                { "group.id", "foo" },
                {"client.id", "HardwareService"}
                };

            _kafkaproducer = new Producer(_config);
        }

        public void SaveEvents(Guid aggregateId, IEnumerable<Event> events, int expectedVersion)
        {
            Console.WriteLine($"{DateTime.Now} Publishing event {events} to Kafka");

            foreach (var eventToPublish in events)
            {
                var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(eventToPublish));

                var topicName = eventToPublish.EventType.ToString();
                var deliveryReport = _kafkaproducer.ProduceAsync(topicName, null, bytes);
                if(deliveryReport.IsFaulted)
                    throw new Exception("faulted");
            }
            
        }
       
        public void StartEventListener(IContainer container)
        {
            //var busconnectionEvents = container.ResolveNamed<IBusControl>("Events");
            var eventProcessor = container.Resolve<SensorEventProcessor>();

            var busconnectionCommands = container.ResolveNamed<IBusControl>("Commands");

            ManualResetEvent manualResetEvent = new ManualResetEvent(true);

            //busconnectionEvents.Start();

            using (var consumer = new Consumer<Null, string>(_config, null, new StringDeserializer(Encoding.UTF8)))
            {
                
                bool cancelled = false;

                // SensorEventConsumers handlers = new SensorEventConsumers(_logger);

                consumer.OnMessage += (_, msg) =>
                {
                    manualResetEvent.Reset();
                    //var endp = busconnectionEvents.GetSendEndpoint(new Uri("rabbitmq://localhost/SensorEvents")).Result;

                    var topicType = KafkaTopics.FirstOrDefault(a => a.ToString() == msg.Topic);
                    if (topicType == default(Type))
                        throw new Exception("Something went wrong here");
                    
                    var ev = JsonConvert.DeserializeObject(msg.Value,topicType);
                        
                    //prevent poisonous events Id cannot be default guid!
                    var @event = ev as Event;
                    if (@event != null && @event.Id != Guid.Empty)
                    {                       
                        eventProcessor.Process((dynamic) @event);
                        // endp.Send(ev);
                        //handlers.Consume(ev);
                    }

                    manualResetEvent.Set();

                };

                var starting = false;
                consumer.OnPartitionEOF += async (sender, offset) =>
                {                 
                    if (starting) return;

                    starting = true;

                    var handle = await busconnectionCommands.StartAsync();
                    await handle.Ready;
                    //ITS NOT READY DAMN IT
                    IsReady = true;
                };

                consumer.OnError += (_, error) =>
                {
                    Console.WriteLine($"Error: {error}");
                    cancelled = true;
                };

                consumer.OnPartitionsAssigned += (obj, partitions) =>
                {
                    var fromBeginning =
                        partitions.Select(p => new TopicPartitionOffset(p.Topic, p.Partition, Offset.Beginning))
                            .ToList();
                    consumer.Assign(fromBeginning);
                };

                
                consumer.Subscribe(KafkaTopics.Select(a=>a.ToString()));

                while (!cancelled)
                {
                    manualResetEvent.WaitOne();
                    consumer.Poll(TimeSpan.FromSeconds(1));
                }
            }
        }
    }
}
