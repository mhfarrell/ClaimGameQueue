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
            var list = new List<Claim>();
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
                    qMessages = JsonConvert.DeserializeObject(message);
                    var region_id = new Guid(qMessages.RegionId);
                    var user_id = new Guid(qMessages.UserId);
                    int uClaims = qMessages.Claims;
                    var existingClaim = list.FirstOrDefault(x => x.UserId.Equals(user_id) && x.RegionId.Equals(region_id));
                    if (existingClaim == null) list.Add(new Claim(user_id, region_id));
                    else existingClaim.Claims++;
                    Console.WriteLine("Queue Message {0}, UserID: {1}, RegionID: {2}, Claims: {3}", i, qMessages.userId, qMessages.regionId, qMessages.Claims);
                    i++;
                    //keep for adding up claims

                    //string URI;
                    //try
                    //{
                    //    //URI = "http://local.api.claimgame/api/claim/update";
                    //    URI = "http://mockbin.org/bin/2ecbf077-2ffe-4b78-8d70-d50685df1481/";

                    //    using (WebClient wc = new WebClient())
                    //    {
                    //        wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                    //        wc.Headers.Add("X-Auth-Header");
                    //        wc.Headers["X-Auth-Header"] = "test";
                    //        string HtmlResult = wc.UploadString(URI, message);
                    //    }
                    //}
                    //catch
                    //{
                    //    reAdd(message);
                    //    string catchError = "{\"Region_id\": \"Unable to post to api\"}";
                    //    URI = "http://ec2-18-220-94-98.us-east-2.compute.amazonaws.com/api/error";
                    //    using (WebClient wc = new WebClient())
                    //    {
                    //        wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    //        string HtmlResult = wc.UploadString(URI, catchError);
                    //    }
                    //}

                };
                channel.BasicConsume(queue: "claims",
                                     autoAck: true,
                                     consumer: consumer);
            }
            Console.WriteLine("");
            foreach (var item in list)
            {
                Console.WriteLine("{0}, {1}, {2}", item.UserId, item.RegionId, item.Claims);
            }
            Console.ReadKey();
        }

        //private static void reAdd(string m)
        //{
        //    dynamic qMessages = JsonConvert.DeserializeObject(m);
        //    var factory = new ConnectionFactory() { HostName = "localhost" };
        //    using (var connection = factory.CreateConnection())
        //    using (var channel = connection.CreateModel())
        //    {
        //        channel.QueueDeclare(queue: "claims",
        //                                         durable: false,
        //                                         exclusive: false,
        //                                         autoDelete: true,
        //                                         arguments: null);
        //        string message = "{\"Regionid\": \"" + qMessages.regionId + ",\"Userid\":\"" + qMessages.userId + "\", \"Claims\": \"1\"}";
        //        var body = Encoding.UTF8.GetBytes(message);
        //        channel.BasicPublish(exchange: "",
        //             routingKey: "claims",
        //             basicProperties: null,
        //             body: body);
        //    }
        //    return;
        //}
    }
}
