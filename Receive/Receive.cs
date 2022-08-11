using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

class Receive
{
    public static void Main()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using(var connection = factory.CreateConnection())
        using(var channel = connection.CreateModel())
        {
            
            channel.QueueDeclare(
            //     The name of the queue. Pass an empty string to make the server generate a name.
                                 queue: "hello",
            //     Should this queue will survive a broker restart?
                                 durable: false,
            //     Should this queue use be limited to its declaring connection? Such a queue will
            //     be deleted when its declaring connection closes.
                                 exclusive: false,
            //     Should this queue be auto-deleted when its last consumer (if any) unsubscribes?
                                 autoDelete: false,
            //     Optional; additional queue arguments, e.g. "x-queue-type"
                                 arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [x] Received {0}", message);
            };
            channel.BasicConsume(queue: "hello",
                                 autoAck: true,
                                 consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}