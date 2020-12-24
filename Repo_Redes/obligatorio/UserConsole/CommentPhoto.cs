using System;
using System.Net.Sockets;
using SharedDataTransfer.DataTransfer;
using SharedProtocol;
using SharedProtocol.Commands;
using SharedProtocol.Protocol;

namespace UserConsole
{
    public class CommentPhoto
    {
        public bool CommentThePhoto(Socket clientSocket)
        {
            Console.WriteLine("Ingrese nombre de la foto que quieras comentar");
            var photoInputName = Console.ReadLine();
            Console.WriteLine("Ingrese comentario");
            var photoInputComment = Console.ReadLine();
            InformationHandler infToPrepare = new InformationHandler();
            InformationTransfer information = new InformationTransfer();
            String[] photoInputToSend = {photoInputName,photoInputComment};
            byte[] dataPrepared = infToPrepare.PreparedData(photoInputToSend, '#');
            int lengthToSend = Util.Constants.HEADER_LENGTH + dataPrepared.Length;
            Protocol testProtocol = new Protocol(Util.Constants.REQ, CommandConstants.AddComment.GetHashCode()
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

            bool existPhoto = ResponseOKComment(clientSocket);
            

            return existPhoto;
        }
      
        
        private bool ResponseOKComment(Socket clientSocket)
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
            isOk = isOk && dataToProcess[0].Equals("COMENTARIO REGISTRADO");
            bool existPhoto=!dataToProcess[0].Equals("FOTO NO VALIDA");
            bool isRes = protocol.Header.Equals("RES");
            if(!existPhoto) Console.WriteLine("No existe la foto que quiere comentar");
            isOk = isRes && isOk && existPhoto;
            return isOk;
        }
    }
}