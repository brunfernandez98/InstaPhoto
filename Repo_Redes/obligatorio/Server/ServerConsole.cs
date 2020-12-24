using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using SystemService;
using Handlers;
using Handlers.User;
using Server.RBMSQ;

namespace Server
{
    public class ServerConsole
    {
        
        private IUserHandler _userHandler { get; set; }
        private ConnectionManagment _connectionManagment { get; set; }
        
        private SynchronousServer _serverConnection{ get; set; }

        
       

        private static bool isServerAcceptConnection;
        
        public ServerConsole(SystemService.System newSystem, IUserHandler userHandler)
        {
            _connectionManagment=new ConnectionManagment(newSystem);
            _userHandler = userHandler;
            isServerAcceptConnection = false;
            
        }
        private void PrintServerOptions()
        {
            Console.WriteLine("");
            Console.WriteLine("Menu Servidor-InstaPhoto");
            Console.WriteLine("---------------------------");
            Console.WriteLine("1 - Aceptar Pedidos");
            Console.WriteLine("2 - Mostrar clientes conectados");
            Console.WriteLine("3 - Alta de un cliente");
            Console.WriteLine("4 - Modificacion de un cliente");
            Console.WriteLine("5 - Baja de un cliente");
            Console.WriteLine("6 - Cerrar el Servidor");
            Console.WriteLine("---------------------------");
        }
       
        public void consoleInput()
        {
          
            ServerOptions options=new ServerOptions(_connectionManagment,_userHandler);
             
            var userInput = 1;
            do
            {
                PrintServerOptions();
                Console.WriteLine("Ingrese el numero asociado con la funcionalidad que desee: ");
                Int32.TryParse(Console.ReadLine(), out userInput);
                if (userInput > 7 || userInput <= 0) Console.WriteLine("Opción elegida no valida, elija otra");
            } while (userInput > 7 || userInput <= 0);
            switch (userInput)
            {
                case 1:
                    try
                    {
                    
                      
                        _userHandler=new UserHandler();
                        _serverConnection = new SynchronousServer(_connectionManagment, _userHandler);
                        Thread thread = new Thread(() => _serverConnection.StartServer());
                        thread.Start();
                        Console.WriteLine("Pedidos de conexion:ON");
                        isServerAcceptConnection = true;
                        consoleInput();
                    }
                    catch (SocketException e)
                    {
                        Console.WriteLine("No se puede aceptar dos veces un pedido"+ e.Message);
                        consoleInput();
                    }
                   
                    break;
                case 2:
                    options.ShowClientsConnected();
                    consoleInput();
                    break;
                case 3:
                    options.RegisterUser();
                    consoleInput();
                    break;
                case 4: 
                    options.ModifyUser();
                    consoleInput();
                    break;
                
                case 5: 
                    options.CleanUser();
                    consoleInput();
                    break;

                case 6:
                    if(isServerAcceptConnection)
                    _serverConnection.ShutDownServer();
                    Console.WriteLine("Cerrando server....");
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Opcion ingresada no es valida.");
                    consoleInput();
                    break;
            }
        }
    }
}