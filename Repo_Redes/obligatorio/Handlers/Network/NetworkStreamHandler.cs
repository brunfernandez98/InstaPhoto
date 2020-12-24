using System;
using System.Net.Sockets;
using Util.Network.Interfaces;

namespace Util.Network
{
    public class NetworkStreamHandler : INetworkStreamHandler
    {
        private readonly Socket _networkStream;

        public NetworkStreamHandler(Socket acceptedConnection)
        {
            _networkStream = acceptedConnection;
            
        }

        public byte[] Read(int length)
        {
            int dataReceived = 0;
            var data = new byte[length];
            while (dataReceived < length)
            {
                var received = _networkStream.Receive(data, dataReceived, length - dataReceived,SocketFlags.None);
                if (received == 0)
                {
                    throw new SocketException();
                }
                dataReceived += received;
            }

            return data;
        }

        public void Write(byte[] data)
        {
            
            _networkStream.Send(data, 0, data.Length,SocketFlags.None);
        }
    }
}