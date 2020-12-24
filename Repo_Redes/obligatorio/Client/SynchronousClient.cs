using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Server;
using Util.SettingsManager;
using Util.SettingsManagerInterface;

namespace Client
{
    public class SynchronousClient
    {
        private static readonly ISettingManager SettingsMgr = new SettingManager();

        public Socket StartClient()
        {
            Console.WriteLine("Client starting...");
            var clientSocket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);

            var serverIp = SettingsMgr.ReadSetting(ClientConfig.ServerIpAddress);
            var serverPort = SettingsMgr.ReadSetting(ClientConfig.ServerPort);
            var clientIp = SettingsMgr.ReadSetting(ClientConfig.ClientIpAddress);
            var clientPort = SettingsMgr.ReadSetting(ClientConfig.ClientPort);

            var clientIpEndPoint = new IPEndPoint(IPAddress.Parse(clientIp), int.Parse(clientPort));
            var serverIpEndPoint = new IPEndPoint(IPAddress.Parse(serverIp), int.Parse(serverPort));

            clientSocket.Bind(clientIpEndPoint);
            Console.WriteLine("Intentado conectar con el servidor");
            clientSocket.Connect(serverIpEndPoint);
            return clientSocket;
        }
    }
}