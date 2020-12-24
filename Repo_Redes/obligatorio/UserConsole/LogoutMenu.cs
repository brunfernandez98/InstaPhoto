using System;
using System.Net.Sockets;
using System.Text;
using SharedDataTransfer.DataTransfer;
using SharedProtocol;
using SharedProtocol.Commands;
using SharedProtocol.Protocol;
using Util.FileHandler;
using Util.FileHandler.Interfaces;

namespace UserConsole
{
    public class LogoutMenu
    {
        private readonly IFileHandler _fileHandler;

        public LogoutMenu()
        {
            _fileHandler=new FileHandler();
        }
        public bool LogOut(Socket clientSocket)
        {
            InformationHandler infToPrepare = new InformationHandler();
            InformationTransfer information = new InformationTransfer();
           
            String[] logout = {"Desloguearme"};
            byte[] dataPrepared = infToPrepare.PreparedData(logout, '#');
            int lengthToSend = Util.Constants.HEADER_LENGTH + dataPrepared.Length;
            Protocol testProtocol = new Protocol(Util.Constants.REQ, CommandConstants.Logout.GetHashCode()
                , lengthToSend, dataPrepared);
            var headerCodification = new HeaderCodification(testProtocol);
            byte[] encodedFrame = headerCodification.GetRequest();
            try
            {
                information.SendData(clientSocket, encodedFrame);
            }
            catch (Exception)
            {
                Console.WriteLine("Connection Error");
            }

          var response=  ReceiveResponse(clientSocket);
          return response;
        }
        private bool ReceiveResponse(Socket clientSocket)
        {
            bool isOk = true;
            InformationTransfer information = new InformationTransfer();
            byte[] received = { };
            try
            {
                received = information.ReceiveData(clientSocket);
            }
            catch (Exception e)
            {
                Console.WriteLine("Connection Error" + e);
                isOk = false;
            }
            HeaderCodification frameCodification = new HeaderCodification();
            Protocol protocol = frameCodification.DecodeData(received);
            bool isRes = protocol.Header.Equals("RES");
            InformationHandler infHandler = new InformationHandler();
            string[] dataToProcess = infHandler.ProcessData(protocol.Data, '#');
            string responseServer = dataToProcess[0];
            if (responseServer != "OK")
                isOk = false;
            
            return isRes && isOk;
        }
    }
}