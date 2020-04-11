using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Virtualization;

namespace VirtualizationTool
{
    public class AMQPManager
    {
        private string sourceQueue;
        private string destinationQueue;
        private DBHandler dBHandler;
        private const string path = @"C:\Users\12353\Desktop\bachelor\SV\HttpListener\amqpConfig.json";
        Thread thread;

        public AMQPManager(DBHandler dBHandler)
        {
            this.dBHandler = dBHandler;
        }

        public void Manage()
        {
            Initialize();
            thread = new Thread(CommunicateWithSource);
            thread.Start();
        }

        private void Initialize()
        {
            var content = Program.ReadFile(path);
            var configuration = JsonConvert.DeserializeObject<Config>(content);
            this.sourceQueue = configuration.SourceQueue;
            this.destinationQueue = configuration.DestinationQueue;
        }

        public void CommunicateWithSource()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                // declare the queue
                channel.QueueDeclare(queue: sourceQueue, durable: false,
                  exclusive: false, autoDelete: false, arguments: null);

                // In order to spread the load equally over multiple servers if necesary
                channel.BasicQos(0, 1, false);
                var consumer = new EventingBasicConsumer(channel);
                channel.BasicConsume(queue: sourceQueue,
                  autoAck: false, consumer: consumer);
                // Console.WriteLine(" [x] Awaiting RPC requests");

                consumer.Received += (model, ea) =>
                {
                    string response = null;
                    var body = ea.Body;
                    var props = ea.BasicProperties;
                    var replyProps = channel.CreateBasicProperties();
                    replyProps.CorrelationId = props.CorrelationId;
                    try
                    {
                        var message = Encoding.UTF8.GetString(body);
                        //if ()
                        response = CreateResponse(message);
                    }
                    catch (Exception e)
                    {
                        response = "";
                    }
                    finally
                    {
                        var responseBytes = Encoding.UTF8.GetBytes(response);
                        channel.BasicPublish(exchange: "", routingKey: props.ReplyTo,
                          basicProperties: replyProps, body: responseBytes);
                        channel.BasicAck(deliveryTag: ea.DeliveryTag,
                          multiple: false);
                    }
                };

                Console.ReadLine();
            }
        }

        public string CreateResponse(string message)
        {
            //var commObj = JsonConvert.DeserializeObject<CommObject>(message);
            var document = dBHandler.CheckEntryInDB(message);
            if (document != null)
            {
                var resp = document.GetElement("response").Value.ToString();
                return resp;
            }

            var response = Rpc.MakeRequest(message, destinationQueue);
            dBHandler.CreateNewDocumentInDB(message, response);
            return response;
        }
    }

    public class RpcClient
    {
        private readonly IConnection connection;
        private readonly IModel channel;
        private readonly string replyQueueName;
        private readonly EventingBasicConsumer consumer;
        private readonly BlockingCollection<string> respQueue = new BlockingCollection<string>();
        private readonly IBasicProperties props;

        public RpcClient()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };

            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            replyQueueName = channel.QueueDeclare().QueueName;
            consumer = new EventingBasicConsumer(channel);

            props = channel.CreateBasicProperties();
            var correlationId = Guid.NewGuid().ToString();
            props.CorrelationId = correlationId;
            props.ReplyTo = replyQueueName;

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var response = Encoding.UTF8.GetString(body);
                if (ea.BasicProperties.CorrelationId == correlationId)
                {
                    respQueue.Add(response);
                }
            };
        }

        public string Call(string message, string queue)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(
                exchange: "",
                routingKey: queue,
                basicProperties: props,
                body: messageBytes);

            channel.BasicConsume(
                consumer: consumer,
                queue: replyQueueName,
                autoAck: true);

            return respQueue.Take();
        }

        public void Close()
        {
            connection.Close();
        }
    }

    public class Rpc
    {
        public static string MakeRequest(string request, string queue)
        {
            var rpcClient = new RpcClient();
            var response = rpcClient.Call(request, queue);
            rpcClient.Close();
            return response;
        }
    }
}
