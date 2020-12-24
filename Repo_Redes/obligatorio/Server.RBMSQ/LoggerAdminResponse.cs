using System;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using Server;
using Server.RBMSQ;
using SharedDataTransfer.DataTransferMQ;
using Util.SettingsManager;
using Util.SettingsManagerInterface;

namespace Server.RBMSQ
{
    public class LoggerAdminResponse
    {
       
        private static IModel _channel;
        private static ISettingManager _settingManager;

        public LoggerAdminResponse()
        {
            _settingManager=new SettingManager();
            _channel = StartService();
        }

        private static IModel StartService()
        {
            var hostName=_settingManager.ReadSetting(ServerConfig.ServerIpConfigKey);
            ConnectionFactory factory = new ConnectionFactory();
            
            var channel = new ConnectionFactory()
            {
                HostName = "localhost",
            }.CreateConnection().CreateModel();
            channel.QueueDeclare(queue: "queueResponse",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
        

            return channel;
        }

        public void SendMessage(InformationAdmin oneType)
        {
            var stringLog = JsonSerializer.Serialize(oneType);
            try
            {
               
                var body = Encoding.UTF8.GetBytes(stringLog);
                _channel.BasicPublish(exchange: "",
                    routingKey: "queueResponse",
                    basicProperties: null,
                    body: body);
                Console.WriteLine("Message {0} sent successfully", stringLog);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
               
            }

           
        }
        
        
       
    }
}
