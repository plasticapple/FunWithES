using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.messagebus;
using Confluent.Kafka;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

using Confluent.Kafka.Serialization;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace HardwareService
{
    public class SensorMessage
    {
        public string Name { get; set; }
        public float Temperature { get; set; }
        public Int64 Time { get; set; }

        
    }

    //public class EventBase
    //{
    //    public Guid AggregateRootId { get; set; }
    //    public Guid MessageId { get; set; }
    //    public DateTime TimeStamp { get; set; }
    //}

    //public class TemperatureChangedEvent : EventBase
    //{
    //    public string Name { get; set; }
    //    public float Temperature { get; set; }
    //}

    public class HardwareEventsManager
    {
        //private static Producer _kafkaproducer;
        private MqttClient _client;
        private IModel _channel;

        public void Start()
        {
            SetupBus();
            Subscribe();
        }

        private void SetupBus()
        {           
           // var endpointConfiguration = new EndpointConfiguration("RaspberryDriverEP");
           // var transport = endpointConfiguration.UseTransport<LearningTransport>();
           //transport.StorageDirectory(@"c:\nbustemp");

           // endpointConfiguration.UsePersistence<LearningPersistence>();

           // endpointConfiguration.SendFailedMessagesTo("error");

           // // Use XML to serialize and deserialize messages (which are just
           // // plain classes) to and from message queues
           // endpointConfiguration.UseSerialization<XmlSerializer>();

           // // Ask NServiceBus to automatically create message queues
           // endpointConfiguration.EnableInstallers();

           // //var routing = transport.Routing();

           // _endpoint = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();
        }

        public void Subscribe()
        {
            //var config = new Dictionary<string, object> {
            //    { "bootstrap.servers", "PLAINTEXT://kafkaserver:9092" },
            //    { "group.id", "foo" },
            //    {"client.id", "RaspberryDriver"}                
            //    };
            //_kafkaproducer = new Producer(config);

            // create client instance
            _client = new MqttClient("localhost");

            // register to message received
            _client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;

            string clientId = "HardwareService";
            _client.Connect(clientId, "unisondriver", "unisondriver");

            _client.ConnectionClosed +=
                (sender, args) => { Console.WriteLine($"{DateTime.Now} mosquito connection closed"); };

            // subscribe to the topic "/home/temperature" with QoS 2
            _client.Subscribe(new string[] { "/hardware/tempsensors" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
            
        }

        public void UnSubscribe()
        {
            _client.Disconnect();
          //  _kafkaproducer.Flush(TimeSpan.FromSeconds(10));
        }

        void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            
            var topicName = "Sensors_Events";

            var receivedMessageStr = System.Text.Encoding.Default.GetString(e.Message);

            SensorMessage message;
            try
            {
                message = JsonConvert.DeserializeObject<SensorMessage>(receivedMessageStr);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to Deserialize {receivedMessageStr}, message: {ex.Message}");
                return;
            }

            var time = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            
            time = time.AddMilliseconds(message.Time);

            var binarycommand = HelperMethods.SerialiseIntoBinary(new UpdateSensorTempCommand(new Guid(), new Guid(),20));
            
            _channel.BasicPublish(exchange: "",
                routingKey: "hello",
                basicProperties: null,
                body: binarycommand);

            Console.WriteLine($"Sent a UpdateSensorTempCommand");

            /// 
            /// What Should it be pushing through kafka or MESSAGE BUS oooooohhhhh....
            /// 

            //var eventToPublish = new TemperatureChangedEvent()
            //{
            //    AggregateRootId = Guid.Empty,
            //    MessageId = Guid.NewGuid(),
            //    Temperature = message.Temperature,
            //    TimeStamp = time
            //};

            ////var eventToPublish = "event:" + receivedMessageStr;

            //Console.WriteLine($"{DateTime.Now} Publishing event {receivedMessageStr} to Kafka");
            //var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(eventToPublish));

            //var deliveryReport = await _kafkaproducer.ProduceAsync(topicName, null, bytes);                

            //Console.WriteLine($"{DateTime.Now} Pushed Event {eventToPublish} to Kafka with result {deliveryReport?.Error?.Reason}");
            //    // Tasks are not waited on synchronously (ContinueWith is not synchronous),
            //    // so it's possible they may still in progress here.
        }
    }
}
