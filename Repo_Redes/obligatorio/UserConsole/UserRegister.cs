using System;
using System.Net.Sockets;
using Handlers;
using Handlers.User;
using SharedDataTransfer.DataTransfer;
using SharedProtocol;
using SharedProtocol.Commands;
using SharedProtocol.Protocol;

namespace UserConsole
{
    public class UserRegister
    {
        
        public bool RegisterManagment(Socket clientSocket)
        {
            
            Console.WriteLine("Ingrese Nombre");
            var userInputName = Console.ReadLine();
            Console.WriteLine("Ingrese LastName");
            var userLastName = Console.ReadLine();
            Console.WriteLine("Ingrese Email");
            var userInputEmail = Console.ReadLine();
            Console.WriteLine("Ingrese Password");
            var userInputPassword = Console.ReadLine();
            InformationHandler infToPrepare = new InformationHandler();
            InformationTransfer information = new InformationTransfer();
            String[] userDomain = {userInputName,userLastName,userInputEmail,userInputPassword};
            byte[] dataPrepared = infToPrepare.PreparedData(userDomain, '#');
            int lengthToSend = Util.Constants.HEADER_LENGTH + dataPrepared.Length;
            Protocol testProtocol = new Protocol(Util.Constants.REQ, CommandConstants.SignUp.GetHashCode(), lengthToSend, dataPrepared);
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
            return isRes && isOk && !IsUserRegistered(protocol.Data);
        }
        private bool IsUserRegistered(byte[] package)
        {
            
            bool isRegister = false;
            InformationHandler infHandler = new InformationHandler();
            string[] dataToProcess = infHandler.ProcessData(package, '#');
            string responseServer = dataToProcess[0];
           
            
            if (responseServer == "USUARIO YA REGISTRADO" || responseServer =="USUARIO NO PUDO SER REGISTRADO Verifique Email" || responseServer =="USUARIO NO PUDO SER REGISTRADO Verifique Password")
                isRegister = true;

            if(responseServer =="USUARIO NO PUDO SER REGISTRADO Verifique Email")
                Console.WriteLine("Formato del email no valido");
            if(responseServer =="USUARIO NO PUDO SER REGISTRADO Verifique Password")
                Console.WriteLine("Formato de la contraseña no valida");
            if(responseServer =="USUARIO YA REGISTRADO")
                Console.WriteLine("Ya existe un usuario registrado con ese email");
            return isRegister;
        }
    }
    
    
}