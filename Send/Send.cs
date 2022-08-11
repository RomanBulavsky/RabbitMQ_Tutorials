using System;
using RabbitMQ.Client;
using System.Text;

class Send
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

            string message = "Hello World!";
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "",
                                 routingKey: "hello",
                                 basicProperties: null,
                                 body: body);
            Console.WriteLine(" [x] Sent {0}", message);
        }

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }
}