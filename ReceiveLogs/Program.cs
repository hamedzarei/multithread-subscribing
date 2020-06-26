using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading;

namespace ReceiveLogs
{

    // public class Work
    // {
    //     public static void Main()
    //     {
    //         // Start a thread that calls a parameterized static method.
    //         Thread newThread = new Thread(Work.DoWork);
    //         newThread.Start(42);

    //         // Start a thread that calls a parameterized instance method.
    //         Work w = new Work();
    //         newThread = new Thread(w.DoMoreWork);
    //         newThread.Start("The answer.");
    //     }
    
    //     public static void DoWork(object data)
    //     {
    //         Console.WriteLine("Static thread procedure. Data='{0}'",
    //             data);
    //     }

    //     public void DoMoreWork(object data)
    //     {
    //         Console.WriteLine("Instance thread procedure. Data='{0}'",
    //             data);
    //     }
    // }
    class Program
    {
        static void Main(string[] args)
        {
            Thread t = new Thread(Subscribe);
            Thread t1 = new Thread(Subscribe);

            t.Start("logs");
            t1.Start("logs1");

            // t.Join();
            // t1.Join();
            
        }

        public static void Subscribe(object myExchange)
        {
            Console.WriteLine("here: ", myExchange);
            string myex = myExchange.ToString();
            var factory = new ConnectionFactory() { HostName = "localhost" };
            factory.UserName = "rabbitmq";
            factory.Password = "rabbitmq";
            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: myex, type: ExchangeType.Fanout);

                var queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(queue: queueName,
                                exchange: myex,
                                routingKey: "");

                Console.WriteLine(" [*] Waiting for logs.");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine("{0}: [x] {1}", myex, message);
                };
                channel.BasicConsume(queue: queueName,
                                    autoAck: true,
                                    consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
