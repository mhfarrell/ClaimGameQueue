using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClaimGameQueue.Remover
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Remover Starting...");
            //var list = new List<Claim>();
            string qMessageTest = "";
            int i = 0;
            dynamic qMessages;
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "claims",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: true,
                                     arguments: null);       
                
                var consumer = new EventingBasicConsumer(channel); 
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);                    
                    //qMessageTest = message;
                    //qMessages = JsonConvert.DeserializeObject(message);
                    //var region_id = qMessages.RegionId;
                    //var user_id = qMessages.UserId;
                    //int uClaims = qMessages.Claims;
                    //var existingClaim = list.FirstOrDefault(x => x.UserId.Equals(user_id) && x.RegionId.Equals(region_id));
                    //if (existingClaim == null) list.Add(new Claim(user_id, region_id));
                    //else existingClaim.Claims++;
                    Console.WriteLine("Message {0} is: {1}", i, message);
                    //Console.WriteLine("Queue Message {0}, UserID: {1}, RegionID: {2}, Claims: {3}", i, qMessages.userId, qMessages.regionId, qMessages.Claims);
                    //i++;
                };
                channel.BasicConsume(queue: "claims",
                                     autoAck: true,
                                     consumer: consumer);
            }
            Console.WriteLine(qMessageTest);
            //foreach (var item in list)
            //{
            //    Console.WriteLine("{0}, {1}, {2}", item.UserId, item.RegionId, item.Claims);
            //}
            //Console.ReadKey();
        }
    }
}
