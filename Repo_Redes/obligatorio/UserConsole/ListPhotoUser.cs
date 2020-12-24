using System;
using System.Net.Sockets;
using Domain;
using SharedDataTransfer.DataTransfer;
using SharedProtocol;
using SharedProtocol.Commands;
using SharedProtocol.Protocol;

namespace UserConsole
{
    public class ListPhotoUser
    {
        public void ListPhoto(Socket clientSocket)
        {
            Console.WriteLine("Ingrese Email del usuario del que desea ver las fotos");
            var userInputNameUser = Console.ReadLine();
            InformationHandler infToPrepare = new InformationHandler();
            InformationTransfer information = new InformationTransfer();
           
            String[] listPhoto = {"QUIERO LA LISTA DE FOTOS",userInputNameUser};
            byte[] dataPrepared = infToPrepare.PreparedData(listPhoto, '#');
            int lengthToSend = Util.Constants.HEADER_LENGTH + dataPrepared.Length;
            Protocol testProtocol = new Protocol(Util.Constants.REQ, CommandConstants.ListPhoto.GetHashCode()
                , lengthToSend, dataPrepared);
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

            ReceiveResponse(clientSocket,userInputNameUser);
        }
        private void ReceiveResponse(Socket clientSocket,string userName)
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
            bool noExistUser=dataToProcess[0].Equals("USUARIO NO VALIDO");
            bool isRes = protocol.Header.Equals("RES");
            if (!isRes || !isOk) Console.WriteLine("ERROR DE COMUNICACION");
            else
            if(noExistUser) Console.WriteLine("USUARIO NO ENCONTRADO");
            else
            ShowPhotos(dataToProcess,userName);
        }
        private void ShowPhotos(string[] photos,string userName)
        {
            Console.WriteLine("___________ Cantidad de fotografias del usuario ___________:"+userName);
            if (photos.Length == 0 || photos[0].Equals("")) {
                Console.Write("Este usuario todavia no subio ninguna fotografia");
            }else{
                Console.WriteLine(photos.Length);
            }

            foreach (var photo in photos)
            {
                Console.WriteLine(photo);
                  
            }
        }

       
        
    }
}