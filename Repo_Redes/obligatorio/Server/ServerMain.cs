using System;
using System.Threading;
using SystemService;
using Handlers;
using Handlers.User;

using Server.RBMSQ;


namespace Server
{
    public static class ServerMain
    {
        public static UserHandler _userHandler { get; set; }

        public static void Main(string[] args)
        {
           
            
           SynchronousServer _serverConnection=new SynchronousServer();
            Thread thread2 = new Thread(() => _serverConnection.Receive("queueRequest"));
            thread2.Start();
            _userHandler = new UserHandler();
            SystemService.System oneSystem = SystemService.System.GetInstance();
            ServerConsole consoleServer = new ServerConsole(oneSystem, _userHandler);
            consoleServer.consoleInput();
            
        }
    }
}