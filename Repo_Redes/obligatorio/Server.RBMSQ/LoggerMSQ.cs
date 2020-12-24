using System;
using System.Text;
using System.Text.Json;

using RabbitMQ.Client;
using SharedDataTransfer.DataTransferMQ;
using SharedLog;

using Util.SettingsManager;
using Util.SettingsManagerInterface;

namespace Server.RBMSQ
{
    public class LoggerMSQ
    {
        private static  LoggerMSQ _instance;
        private static IModel _channel;
        private static ISettingManager _settingManager;

        private LoggerMSQ()
        {
            _settingManager=new SettingManager();
            _channel = StartService();
        }

        private static IModel StartService()
        {
            var hostName=_settingManager.ReadSetting(ServerConfig.ServerIpConfigKey);
          
            var channel = new ConnectionFactory() {HostName = hostName}.CreateConnection().CreateModel();
            channel.QueueDeclare(queue: "LOGS",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            return channel;
        }

        public void SendMessage(Log oneType)
        {
            var stringLog = JsonSerializer.Serialize(oneType);
            try
            {
                var body = Encoding.UTF8.GetBytes(stringLog);
                _channel.BasicPublish(exchange: "",
                    routingKey: "LOGS",
                    basicProperties: null,
                    body: body);
               
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
               
            }

           
        }
        
        
        public static LoggerMSQ GetInstance()
        {
            return _instance ??= new LoggerMSQ();
        }
    }
}