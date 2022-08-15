using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading;

class Worker
{
    public static async Task Main()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using(var connection = factory.CreateConnection())
        using(var channel = connection.CreateModel())
        {
            channel.QueueDeclare(queue: "task_queue",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            Console.WriteLine(" [*] Waiting for messages.");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [x] Received {0}", message);

                int dots = message.Split('.').Length - 1;
                Thread.Sleep(dots * 1000);
                if(new Random().Next(10) > 5){
                    Console.WriteLine(" [x] Error");
                    // this consumer tag identifies the subscription
                    // when it has to be cancelled
                    // channel.BasicCancel(channel.BasicConsume(queueName, false, consumer));
                    // channel.BasicCancelNoWait(ea.ConsumerTag);
                    channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                    return;
                }

                Console.WriteLine(" [x] Done");

                // Note: it is possible to access the channel via
                //       ((EventingBasicConsumer)sender).Model here
                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            channel.BasicConsume(queue: "task_queue",
                                 autoAck: false,
                                 consumer: consumer);
            while(true){
                Console.WriteLine(" [x] waiting.....");
                await Task.Delay(1000);
                Console.WriteLine(" Press [enter] to continue waiting.");
                Console.ReadLine();
            }
        }
    }
}