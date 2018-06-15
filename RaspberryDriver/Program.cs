using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using HardwareService;

namespace RaspberryDriver
{
    class Program
    {
        const string topicName = "Sensors_Events";

        static void Main(string[] args)
        {
            Console.WriteLine("Raspberry RTU Started!");

            var hm = new HardwareEventsManager();

            Task.Factory.StartNew(hm.Start);

            //var config = new Dictionary<string, object> {
            //    { "bootstrap.servers", "PLAINTEXT://kafkaserver:9092" },
            //    { "group.id", "foo" },
            //    {"client.id", "RaspberryDriver"},
            //    { "default.topic.config", new Dictionary<string, object>
            //        {
            //            { "auto.offset.reset", "earliest" }
            //        }
            //    }
            //    };


            //using (var consumer = new Consumer<Null, string>(config, null, new StringDeserializer(Encoding.UTF8)))
            //{



            //    bool cancelled = false;
            //    consumer.OnMessage += (_, msg) => Console.WriteLine($"Message value: {msg.Value}");

            //    consumer.OnPartitionEOF +=
            //        (_, end) =>
            //        {
            //            Console.WriteLine($"Reached end of topic {end.Topic} partition {end.Partition}.");
            //            //consumer.Assign(new List<TopicPartitionOffset>() { new TopicPartitionOffset(topicName, 0, Offset.Beginning) });
            //        };


            //    consumer.OnError += (_, error) =>
            //    {
            //        Console.WriteLine($"Error: {error}");
            //        cancelled = true;
            //    };

            //    consumer.OnPartitionsAssigned += (obj, partitions) =>
            //    {
            //        var fromBeginning = partitions.Select(p => new TopicPartitionOffset(p.Topic, p.Partition, Offset.Beginning)).ToList();
            //        consumer.Assign(fromBeginning);
            //    };


            //    consumer.Subscribe(topicName);
            //    //consumer.Unassign();
            //    //consumer.Assign(new List<TopicPartitionOffset>() { new TopicPartitionOffset(topicName, 0, Offset.Beginning) });
            //    //consumer.Assign(new List<TopicPartitionOffset>() { new TopicPartitionOffset(topicName, 0, Offset.Beginning), new TopicPartitionOffset(topicName, 1, Offset.Beginning) });


            //    while (!cancelled)
            //    {
            //        consumer.Poll(TimeSpan.FromSeconds(1));
            //    }
            //}

            Console.Read();
        }
    }
}
