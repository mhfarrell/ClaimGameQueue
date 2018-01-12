using ClaimGameQueue.Claims;
using EasyNetQ;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace ClaimGameQueue.Web.Controllers
{
    public class ClaimController : ApiController
    {
        public HttpResponseMessage Post([FromBody] gameClaim claim)
        {
            var messageBus = RabbitHutch.CreateBus("host=localhost");
            messageBus.Publish(claim);
            //add to the queue
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "claims",
                                                 durable: false,
                                                 exclusive: false,
                                                 autoDelete: true,
                                                 arguments: null);
                string message = "{\"RegionId\": \"" + claim.regionId + ",\"UserId\":\"" + claim.userId + "\", \"Claims\": \"1\"}";
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: "",
                     routingKey: "claims",
                     basicProperties: null,
                     body: body);
            }
            //end add to queue

            var response = new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent("Created")
            };
            return response;
        }
    }
}