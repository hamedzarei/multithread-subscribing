using System;
using RabbitMQ.Client;
using System.Text;

namespace EmitLog
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            factory.UserName = "rabbitmq";
            factory.Password = "rabbitmq";
            using(var connection = factory.CreateConnection())
            {
                using(var channel = connection.CreateModel())
                {
                    for (int i = 0; i < 9; i++) {
                        channel.ExchangeDeclare(exchange: "logs", type: ExchangeType.Fanout);

                        var message = GetMessage(args);
                        var body = Encoding.UTF8.GetBytes(message);
                        channel.BasicPublish(exchange: "logs",
                                            routingKey: "",
                                            basicProperties: null,
                                            body: body);
                        Console.WriteLine(" [x] Sent {0}", message);
                    }
                }

                using(var channel = connection.CreateModel())
                {
                    for (int i = 0; i < 9; i++) {
                        channel.ExchangeDeclare(exchange: "logs1", type: ExchangeType.Fanout);

                        var message = GetMessage(args);
                        var body = Encoding.UTF8.GetBytes(message);
                        channel.BasicPublish(exchange: "logs1",
                                            routingKey: "",
                                            basicProperties: null,
                                            body: body);
                        Console.WriteLine(" [x] Sent {0}", message);
                    }
                }
            }
            

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }

        private static string GetMessage(string[] args)
        {
            return ((args.Length > 0)
                ? string.Join(" ", args)
                : "info: Hello World!");
        }
    }
}
