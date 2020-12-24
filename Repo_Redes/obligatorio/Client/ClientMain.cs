using System.Net.Sockets;
using UserConsole;

namespace Client
{
    public class ClientMain
    {
        private static ConsoleUser _consoleIu;

        public static void Main(string[] args)
        {
            SynchronousClient syncC = new SynchronousClient();
            Socket clientSocket = syncC.StartClient();
            _consoleIu = new ConsoleUser(clientSocket);
            _consoleIu.ConsoleMenu();
        }
    }
}