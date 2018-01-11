using ClaimGameQueue.Claims;
using EasyNetQ;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace ClaimGameQueue.Processor
{
    class Program
    {
        static void Main(string[] args)
        {
            string keyRead;
            ConsoleKeyInfo input;
            var logNothingLogger = new LogNothingLogger();
            var messageBus = RabbitHutch.CreateBus("host=localhost", reg => reg.Register<IEasyNetQLogger>(IEasyNetQLogger => logNothingLogger));
            Console.WriteLine("Commands: \r\n" +
                               "Press: 'C' for incoming claims \r\n" +
                               "Press: 'E' for errors");
            Console.WriteLine("Waiting for key input...");
            while (true)
            {
                input = Console.ReadKey(true);
                keyRead = input.Key.ToString();
                switch (keyRead)
                {
                    case "C":
                        logNothingLogger = new LogNothingLogger();
                        messageBus = RabbitHutch.CreateBus("host=localhost", reg => reg.Register<IEasyNetQLogger>(IEasyNetQLogger => logNothingLogger));
                        messageBus.Subscribe<gameClaim>("claims", msg =>
                        {
                            Console.WriteLine("Processing Claim; Region: {0} -- User: {1} -- Claims: 1 ", msg.regionId, msg.userId);
                        });
                        keyRead = "";
                        break;
                    case "E":
                        logNothingLogger = new LogNothingLogger();
                        messageBus = RabbitHutch.CreateBus("host=localhost", reg => reg.Register<IEasyNetQLogger>(IEasyNetQLogger => logNothingLogger));
                        messageBus.Subscribe<errorQ>("errorQ", msg =>
                        {
                            Console.WriteLine("Error Message: {0}", msg.errorMessage);
                        });
                        keyRead = "";
                        break;
                }
            }
        }
    }

    public class LogNothingLogger : IEasyNetQLogger
    {
        public void DebugWrite(string format, params object[] args)
        {
        }

        public void ErrorWrite(string format, params object[] args)
        {
        }

        public void ErrorWrite(Exception exception)
        {
        }

        public void InfoWrite(string format, params object[] args)
        {
        }
    }
}
