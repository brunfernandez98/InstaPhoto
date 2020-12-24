using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using SharedDataTransfer.DataTransfer;
using Util.SettingsManager;
using Util.SettingsManagerInterface;
using SystemService;
using Handlers.User;
using RabbitMQ.Client;
using SharedDataTransfer.DataTransferMQ;



namespace Server
{
    
    public class SynchronousServer
    {
        public Socket ServerSocket
        {
            get ;
            set ;
        }

        private static ISettingManager _settingManager;
        private ConnectionManagment _connectionManagment { get; set; }
        private  IUserHandler _userHandler{ get; set; }
       
        public bool _exit{ get; set; }
       
        public SynchronousServer(ConnectionManagment oneConnectionManagment,IUserHandler userHandler)
        {




            _exit = false;
             _settingManager =new SettingManager();
            ServerSocket = SetUpServer();
            
            _connectionManagment = oneConnectionManagment;
            _userHandler = userHandler;
        }
            
        public SynchronousServer()
        {
            
        }
        
        
        
        
        public  void Receive(string queue)
        {
            var information=new InformationAdmin();
            bool serverRunning = true;
            try
            { 
                while (!_exit && serverRunning)
                {
                    using (IConnection connection = new ConnectionFactory().CreateConnection())
                    {
                        using (IModel channel = connection.CreateModel())
                        {
                            channel.QueueDeclare(queue, false, false, false, null);
                            BasicGetResult result = channel.BasicGet(queue, true);

                            if (result == null) continue;
                            information = JsonSerializer.Deserialize<InformationAdmin>(result.Body.ToArray());
                            Console.WriteLine(information.ToString());
                            HandlerPackage(information);
                        }
                    }
                   

                }
                
            }catch (Exception e)
            {
                serverRunning = false;
            }
        }



        private void HandlerPackage(InformationAdmin information)
        {
            ControlRequestAdmin controlAdmin=new ControlRequestAdmin();
            string message=controlAdmin.HandleRequest(information);
            Console.WriteLine(message);
        }
        
        
        
        private static Socket SetUpServer()
        {
            Console.WriteLine("Servidor esta iniciando...");
            var serverSocket = new Socket(
                AddressFamily.InterNetwork, 
                SocketType.Stream,
                ProtocolType.Tcp);

            var serverIp = _settingManager.ReadSetting(ServerConfig.ServerIpConfigKey);
            var serverPort = _settingManager.ReadSetting(ServerConfig.SeverPortConfigKey);
            int serverPortNumber = int.Parse(serverPort);
            var ipEndPoint = new IPEndPoint(IPAddress.Parse(serverIp), serverPortNumber);

            serverSocket.Bind(ipEndPoint); 
            serverSocket.Listen(100);
            Console.WriteLine($" El Servidor empezara a escuchar conexiones en el puerto: {serverPortNumber} ...");
            return serverSocket;
        }

     
        
        
        
        public void StartServer()
        {
            bool serverRunning = true;
            while (!_exit && serverRunning)
            {
                try
                {
                    Socket acceptedConnection = ServerSocket.Accept();
                    Thread thread = new Thread(() => ListenRequests(acceptedConnection));
                    EstablishedConnection establishedConnection = new EstablishedConnection(acceptedConnection,
                        thread,null);
                    _connectionManagment.AddEstablishedConnection(establishedConnection);
                    thread.Start();
                }
                catch (SocketException msg)
                {
                    Console.WriteLine(msg.Message);
                    serverRunning = false;
                }
               
            }
        }

        public void ShutDownServer()
        {
            _connectionManagment.CloseAllExistConnection();
            ServerSocket.Close();
            SetExit(true);
           
           
            
        }
        
        private void ListenRequests(Socket connection)
        {
            bool serverRunning = true;
            try
            { 
                while (!_exit && serverRunning)
                {
                    
                    InformationTransfer inf = new InformationTransfer();
                    byte[] buffer=inf.ReceiveData(connection);
                    ControlRequest request=new ControlRequest(_connectionManagment,_userHandler);
                    request.HandleRequest(buffer);
                }
                
                
            }
            catch (ObjectDisposedException)
            {
                serverRunning = false;
            }
            catch (SocketException)
            {
                
                serverRunning = false;
            }
           
        }
        
       
        
       
        
        
        
        
        
        private void SetExit(bool exit)
        {
            _exit = exit;
        }
    }
}