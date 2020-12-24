using System;
using System.Net.Sockets;
using System.Text;
using Util;
namespace SharedDataTransfer.DataTransfer
{
    public class InformationTransfer
    {
        public  byte[] ReceiveData(Socket clientConnection)
        {
            
          
            byte[] byteBuffer = new byte[Constants.HEADER_LENGTH];
            int iReceive = 0;
            while (iReceive <  Constants.HEADER_LENGTH)
            {
                try
                {
                    int iRecivedData = clientConnection.Receive(byteBuffer, 0, Constants.HEADER_LENGTH, SocketFlags.None);
                    if (iRecivedData == 0)
                    {
                        clientConnection.Shutdown(SocketShutdown.Both);
                        clientConnection.Close();
                    }
                    iReceive += iRecivedData;
                }
                catch (SocketException e)
                {
                    Console.WriteLine(e.Message);
                    
                }
            }
            
            return ReceiveAllData(clientConnection,byteBuffer);
            
        }

        private byte[] ReceiveAllData(Socket clientConnection,byte[] byteBuffer)
        {
            byte[] largo = new byte[Constants.LENGTH_IN_HEADER];
            Array.Copy(byteBuffer, 5, largo, 0, Constants.LENGTH_IN_HEADER);
            int lengthData = int.Parse(Encoding.UTF8.GetString(largo)) - Constants.HEADER_LENGTH;
            byte[] dataToReceive = new byte[lengthData];
            var iReceive = 0;
            int size = Constants.HEADER_LENGTH;
            while ( iReceive < lengthData)
            {
                var remaining = lengthData -  iReceive;
                if (remaining < size)
                {
                    size = remaining;
                }
                try
                {
                    var localReceived = clientConnection.Receive(dataToReceive,  iReceive, size, SocketFlags.None);
                    if (localReceived == 0)
                    {
                            
                        clientConnection.Shutdown(SocketShutdown.Both);
                        clientConnection.Close();
                    }
                       
                    iReceive += localReceived;
                }
                catch (SocketException e)
                {
                    Console.Write(e.Message);
                                             
                }
            }
            byte[] allFrame = new byte[Constants.HEADER_LENGTH+ lengthData];
            Array.Copy(byteBuffer, 0, allFrame, 0, Constants.HEADER_LENGTH);
            Array.Copy(dataToReceive, 0, allFrame, Constants.HEADER_LENGTH, lengthData);
            return allFrame;  
        }
        public void SendData(Socket acceptedConnection, byte[] dataToSend)
        {
            int totalSent = 0;
            int toSend = dataToSend.Length;
            int frameSize = 1024;
            while (totalSent < toSend)
            {
                var remainingToSend = toSend - totalSent;
                if (remainingToSend < frameSize)
                {
                    frameSize = remainingToSend;
                }
                try
                {
                 int dataSend= acceptedConnection.Send(dataToSend,totalSent,frameSize,SocketFlags.None);
                 
                 if (dataSend == 0)
                 {
                   
                     throw new Exception();
                 }
                 totalSent += dataSend;
                 
                }
                
                catch(SocketException e)
                {
                    Console.WriteLine(e.Message);
                }

                
            }
            
        }

        
        
        
      
    }
}