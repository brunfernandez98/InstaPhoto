using System;
using System.Net.Sockets;
using SharedDataTransfer.DataTransfer;
using SharedProtocol;
using SharedProtocol.Commands;
using SharedProtocol.Protocol;

namespace UserConsole
{
    public class ViewComment
    {
        public void ViewCommentsManagment(Socket clientSocket)
        {
            Console.WriteLine("Ingrese nombre de la foto que ver los comentarios");
            var photoInputName = Console.ReadLine();
            InformationHandler infToPrepare = new InformationHandler();
            InformationTransfer information = new InformationTransfer();
            string[] photoToComment = {photoInputName};
            byte[] dataPrepared = infToPrepare.PreparedData(photoToComment, '#');
            int lengthToSend = Util.Constants.HEADER_LENGTH + dataPrepared.Length;
            Protocol testProtocol = new Protocol(Util.Constants.REQ,
                CommandConstants.SeeComment.GetHashCode(), lengthToSend, dataPrepared);
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

            ReceiveResponse(clientSocket, photoInputName);
        }

        private void ReceiveResponse(Socket clientSocket, string namePhoto)
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

            InformationHandler infHandler = new InformationHandler();
            HeaderCodification frameCodification = new HeaderCodification();
            Protocol protocol = frameCodification.DecodeData(received);
            string[] dataToProcess = infHandler.ProcessData(protocol.Data, '#');

            bool isRes = protocol.Header.Equals("RES");
            if (!isRes || !isOk) return;
            if (dataToProcess[0].Equals("FOTO INVALIDA"))
                Console.WriteLine("No existe la fotografia");
            else
                ShowComments(dataToProcess, namePhoto);
        }

        private void ShowComments(string[] photo, string namePhoto)
        {
            Console.WriteLine("FOTO:" + namePhoto);
            Console.WriteLine("___________ Cantidad de comentarios de la foto ___________:" + photo.Length);
            if (photo.Length == 0 || photo[0].Equals(""))
            {
                Console.Write("Esta fotografia todavia no tiene ningun comentario");
            }

            foreach (var user in photo)
            {
                Console.WriteLine(user);
            }
        }
    }
}