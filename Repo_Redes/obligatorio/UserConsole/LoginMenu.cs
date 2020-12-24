using System;
using System.Net.Sockets;
using SharedDataTransfer.DataTransfer;
using SharedProtocol;
using SharedProtocol.Commands;
using SharedProtocol.Protocol;

namespace UserConsole
{
    public class LoginMenu
    {
        public bool LoginManagment(Socket clientSocket)
        {
            Console.WriteLine("Ingrese Email");
            var userInputEmail = Console.ReadLine();
            Console.WriteLine("Ingrese Password");
            var userInputPassword = Console.ReadLine();
            InformationHandler infToPrepare = new InformationHandler();
            InformationTransfer information = new InformationTransfer();
            String[] userDomain = { userInputEmail, userInputPassword };
            byte[] dataPrepared = infToPrepare.PreparedData(userDomain, '#');
            int lengthToSend = Util.Constants.HEADER_LENGTH + dataPrepared.Length;
            Protocol testProtocol = new Protocol(Util.Constants.REQ, CommandConstants.Login.GetHashCode(), lengthToSend, dataPrepared);
            var HeaderCodification = new HeaderCodification(testProtocol);
            byte[] encodedFrame = HeaderCodification.GetRequest();
            try
            {
                information.SendData(clientSocket, encodedFrame);
            }
            catch (Exception)
            {
                Console.WriteLine("Connection Error");
            }

            return ReceiveResponse(clientSocket);
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
            return isRes && isOk && IsUserRegistered(protocol.Data);
        }

        private bool IsUserRegistered(byte[] package)
        {
            bool isRegister = true;
            InformationHandler infHandler = new InformationHandler();
            string[] dataToProcess = infHandler.ProcessData(package, '#');
            string responseServer = dataToProcess[0];
            if (responseServer == "USUARIO NO VALIDO")
                isRegister = false;

            return isRegister;
        }
    }

}