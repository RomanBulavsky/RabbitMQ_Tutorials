using System;
using RabbitMQ.Client;
using System.Text;

class NewTask
{
    public static void Main(string[] args)
    {
        while (true)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "task_queue",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
                System.Console.WriteLine("Pls write down the message:");
                var message = System.Console.ReadLine();
                System.Console.WriteLine($"get message{message}");
                var body = Encoding.UTF8.GetBytes(message);

                var properties = channel.CreateBasicProperties();
                // Marking messages as persistent doesn't fully guarantee that a message won't be lost.
                // Although it tells RabbitMQ to save the message to disk, there is still a short time
                // window when RabbitMQ has accepted a message and hasn't saved it yet. 
                properties.Persistent = true;

                channel.BasicPublish(exchange: "",
                                     routingKey: "task_queue",
                                     basicProperties: properties,
                                     body: body);
                Console.WriteLine(" [x] Sent {0}", message);
            }
        }
    }
}