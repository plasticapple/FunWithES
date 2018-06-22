using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Common;
using Common.messagebus;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using HardwareService.domain;
using HardwareService.domain.events;
using MassTransit;
using MassTransit.Logging;
using Newtonsoft.Json;


namespace HardwareService.command_data_access
{
    public class KafkaEventStore : IEventStore
    {
        private static Producer _kafkaproducer;
        //private string topicName = "Sensors_Events";
        private Dictionary<string, object>  _config;
        IBus _messageBus;


        public bool IsReady { get; internal set; }

        //private void Subscribe()
        //{           
                   
        //}

        public KafkaEventStore()
        {          

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
            }
            
        }

        private void GetKStream()
        {
            //var builder = new KStream();
            //builder.stream(keySerde, valueSerde, "my_entity_events")
            //    .groupByKey(keySerde, valueSerde)
            //    // the folding function: should return the new state
            //    .reduce((currentState, event) -> ..., "my_entity_store");
            //.toStream(); // yields a stream of intermediate states
           // return builder;
        }

        public List<Event> GetEventsForAggregate(Guid aggregateId)
        {
            //List<EventDescriptor> eventDescriptors;

            //if (!_current.TryGetValue(aggregateId, out eventDescriptors))
            //{
            //    throw new AggregateNotFoundException();
            //}

            //return eventDescriptors.Select(desc => desc.EventData).ToList();

            //streams
            //    .store("my_entity_store", QueryableStoreTypes.keyValueStore());
            //    .get(entityId);
            using(var consumer = new Consumer<Null, string>(_config, null, new StringDeserializer(Encoding.UTF8)))
            {
               //consumer.
            }

            return new List<Event>(){new Event(){Version = 1},new Event(){Version = 2}};
        }
       

        public void StartEventListener(IBus bus)
        {
            _messageBus = bus;
            using (var consumer = new Consumer<Null, string>(_config, null, new StringDeserializer(Encoding.UTF8)))
            {
                bool cancelled = false;
                consumer.OnMessage += (_, msg) =>
                {
                    //var binaryevent = HelperMethods.SerialiseIntoBinary(new TemperatureSensorCreated(Guid.NewGuid(), "1234", "nicename"));

                    //endpoint.SendLocal(ev).ConfigureAwait(false);
                    //channel.BasicPublish(exchange: "",
                    //    routingKey: "hello",
                    //    basicProperties: null,
                    //    body: binaryevent);
                    var endp = _messageBus.GetSendEndpoint(new Uri("rabbitmq://localhost/SensorCommands")).Result;

                    // Event ev = null; JsonConvert.DeserializeObject<Event>(msg.Value);


                    if (msg.Topic == typeof(TemperatureSensorCreated).ToString())
                    {
                        var ev = JsonConvert.DeserializeObject<TemperatureSensorCreated>(msg.Value);
                        endp.Send(ev);
                    }
                    else if (msg.Topic == typeof(TemperatureSensorTempUpdated).ToString())
                    {
                        var ev = JsonConvert.DeserializeObject<TemperatureSensorTempUpdated>(msg.Value);
                        endp.Send(ev);
                    }
                };
                   
                    consumer.OnPartitionEOF += (sender, offset) =>
                {
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


                consumer.Subscribe(new List<string>{
                    typeof(TemperatureSensorCreated).ToString(),
                    typeof(TemperatureSensorTempUpdated).ToString()
                });

                while (!cancelled)
                {
                    consumer.Poll(TimeSpan.FromSeconds(1));
                }
            }
        }
    }
}
