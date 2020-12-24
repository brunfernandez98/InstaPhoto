using System;
using System.Net.Sockets;
using Domain;
using SharedDataTransfer.DataTransfer;
using SharedProtocol;
using SharedProtocol.Commands;
using SharedProtocol.Protocol;

namespace UserConsole
{
    public class ListOfUsers
    {
        public void ListOFUsersManagment(Socket clientSocket)
        {
            InformationHandler infToPrepare = new InformationHandler();
            InformationTransfer information = new InformationTransfer();
            String[] listUser = {"QUIERO LA LISTA DE TODOS LOS USUARIOS"};
            byte[] dataPrepared = infToPrepare.PreparedData(listUser, '#');
            int lengthToSend = Util.Constants.HEADER_LENGTH + dataPrepared.Length;
            Protocol testProtocol = new Protocol(Util.Constants.REQ, CommandConstants.ListClients.GetHashCode(), lengthToSend, dataPrepared);
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

            ReceiveResponse(clientSocket);
        }
        private void ReceiveResponse(Socket clientSocket)
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
            InformationHandler infHandler =new InformationHandler();
            HeaderCodification frameCodification = new HeaderCodification();
            Protocol protocol = frameCodification.DecodeData(received);
            string[] dataToProcess= infHandler.ProcessData(protocol.Data, '#');
            
            bool isRes = protocol.Header.Equals("RES");
            if (!isRes || !isOk) return ;
            ShowUsers(dataToProcess);
        }
        
        private void ShowUsers(string[] users)
        {
            Console.WriteLine("___________ Cantidad de usuarios en el sistema ___________:");
            Console.WriteLine(users.Length);
            
            foreach (var user in users)
            {
                Console.WriteLine(user);
                  
            }

            
        }
    }
}