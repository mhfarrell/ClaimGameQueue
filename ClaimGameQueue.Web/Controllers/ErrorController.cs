using ClaimGameQueue.Claims;
using EasyNetQ;
using RabbitMQ.Client;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace ClaimGameQueue.Web.Controllers
{
    public class ErrorController : ApiController
    {
        public HttpResponseMessage Post([FromBody] errorQ error)
        {
            var messageBus = RabbitHutch.CreateBus("host=localhost");
            messageBus.Publish(error);
            //add to the queue
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "errorQ",
                                                 durable: false,
                                                 exclusive: false,
                                                 autoDelete: true,
                                                 arguments: null);
                string message = "{\"Region_id\": \"" + error.errorMessage + "\"}";
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: "",
                     routingKey: "errorQ",
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