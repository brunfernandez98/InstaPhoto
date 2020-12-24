using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Domain;

namespace SystemService
{
    public class ConnectionManagment
    {
        public System OneSystem { get; set; }

        public ConnectionManagment(System system)
        {
            OneSystem = system;
        }
        public void AddEstablishedConnection(EstablishedConnection establishedConnection)
        {
            OneSystem.EstablishedConnectionsList.Add(establishedConnection);
        }
        public bool IsUserConnected(string email)
        {
            return OneSystem.EstablishedConnectionsList.Any(th => 
                th.OneUser.Email.Equals(email));
        }
        
        public Socket GetSocketFromThread(Thread currentThread)
        {
            return OneSystem.EstablishedConnectionsList.Find(th => 
                th.OneThread.ManagedThreadId == currentThread.ManagedThreadId).OneSocket;
        }
        
        public User GetUserFromThread(Thread currentThread)
        {
            return OneSystem.EstablishedConnectionsList.Find(us => us.OneThread == currentThread).OneUser;
        }
        public bool ExistConnectionEstablished(Socket socket)
        {
            bool exist = false;
            IPEndPoint clientPoint = (IPEndPoint)socket.RemoteEndPoint;
            foreach (var i in OneSystem.EstablishedConnectionsList)
            {
                IPEndPoint oneClientEndPoint = (IPEndPoint)i.OneSocket.RemoteEndPoint;
                if (clientPoint.Equals(oneClientEndPoint))
                    exist = true;
            }

            return exist;
        }
        
        public void RemoveConnection(Thread currentThread)
        {
            EstablishedConnection establishedConn = 
                OneSystem.EstablishedConnectionsList.Find(ec => ec.OneThread == currentThread);
            OneSystem.EstablishedConnectionsList.Remove(establishedConn);
        }
        
      

        public void SetUserFromThread(Thread currentThread, User oneUser)
        {
            OneSystem.EstablishedConnectionsList.Find(th =>
                th.OneThread.ManagedThreadId == currentThread.ManagedThreadId).OneUser = oneUser;
        }
        
        public void CloseAllExistConnection()
        {
            foreach (var oneConnection in OneSystem.EstablishedConnectionsList)
            {
                oneConnection.OneSocket.Shutdown(SocketShutdown.Both);
                oneConnection.OneSocket.Close();
            }
            OneSystem.EstablishedConnectionsList.Clear();
        }
    }
}