using System.Net.Sockets;
using System.Threading;
using Domain;

namespace SystemService
{
    public class EstablishedConnection
    {
        public Socket OneSocket { get; set; }
        public Thread OneThread{ get; set; }
        public User OneUser{ get; set; }

        public EstablishedConnection(Socket socket, Thread thread, User user)
        {
            OneSocket = socket;
            OneThread = thread;
            OneUser = user;
            
        }
        public void CleanConnection()
        {
            OneSocket.Shutdown(SocketShutdown.Both);
            OneSocket.Close();
            
        }
       

    }
}