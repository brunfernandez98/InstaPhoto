using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Cors;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ServerLogs.Interfaces;
using SharedLog;

namespace LogsServer.WebApi.Controllers
{
    
    [ApiController]
    [Route("[controller]")]
    public class Logs : ControllerBase
    {
        private static bool ServerRunning;
        private static IModel _model;
        private static ILogsService _logsService;
        private static List<Log> _logList = new List<Log>();
        
        [HttpGet]
        public ActionResult<IEnumerable<Log>> Get()
        {
            ConsumeLogs();
            Console.WriteLine(_logList.Count);
            return _logList;
        }
        private static void ConsumeLogs()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "LOGS",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var log = JsonSerializer.Deserialize<Log>(message);
                    
                    Console.WriteLine(" [x] Autor [{0}], Fecha [{1}], Tipo [{2}], Severidad [{3}] " , log.Author, 
                        log.Date,log.Type,log.TypeSeverity);
                    _logList.Add(log);
                    Console.WriteLine(_logList);
                };
                channel.BasicConsume(queue: "LOGS",
                    autoAck: true,
                    consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                //Console.ReadLine();
            }

        }
    }
}