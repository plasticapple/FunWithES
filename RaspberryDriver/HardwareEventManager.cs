using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Common;
using Common.messagebus;
using MassTransit;
using Newtonsoft.Json;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace RaspberryDriver
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
        private  IBusControl _bus;

        public void Start()
        {
            SetupBus();
            Subscribe();
        }

        private void SetupBus()
        {
            _bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var busSettings = new BusSettings { HostAddress = "localhost", Username = "guest", Password = "guest", QueueName = "SensorCommands" };

                var host = cfg.Host(busSettings.HostAddress, "/", h =>
                {
                    h.Username(busSettings.Username);
                    h.Password(busSettings.Password);
                });

                //cfg.ReceiveEndpoint(busSettings.QueueName, e =>
                //{
                //    e.Consumer(() => new SensorCommandsConsumer(sensorRepo, loggerFactory.CreateLogger("CommandConsumerLogger")));
                //});
            });
        }

        public void Subscribe()
        {


            var caCert = new X509Certificate2(@"C:\playground\FunWithES\democert\ca.crt", @"1234");
            var keyCert = new X509Certificate2(@"C:\playground\FunWithES\democert\cert.pfx", @"");



            //var store = new X509Store(StoreName.Root, StoreLocation.CurrentUser);
            //store.Open(OpenFlags.ReadWrite);
            //store.Add(keyCert);
            //store.Close();

            // create client instance           
           

            _client = new MqttClient("localhost",8883, true, caCert, keyCert, MqttSslProtocols.TLSv1_0, client_RemoteCertificateValidationCallback);
         
            // register to message received
            _client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;

            string clientId = "HardwareService";
            _client.Connect(clientId, "unisondriver", "unisondriver");

            _client.ConnectionClosed +=
                (sender, args) => { Console.WriteLine($"{DateTime.Now} mosquito connection closed"); };

            // subscribe to the topic "/home/temperature" with QoS 2
            _client.Subscribe(new string[] { "/hardware/tempsensors" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
            
        }

        private bool client_RemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslpolicyerrors)
        {
            return true;
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
         
            var addUserEndpoint = _bus.GetSendEndpoint(new Uri("rabbitmq://localhost/SensorCommands")).Result;

            //TODO: need to find out real id of the sensor 

            var command = new UpdateSensorTempCommand(new Guid(),
                message.Name == "28-0000097100be" ? new Guid("f34bb461-ad5c-47b5-a6c9-33fc904955d1") : new Guid("f34bb461-ad5c-47b5-a6c9-33fc904955d2"),
                message.Temperature);

            addUserEndpoint.Send(command);

            Console.WriteLine($"Sent a UpdateSensorTempCommand");          
        }
    }
}
