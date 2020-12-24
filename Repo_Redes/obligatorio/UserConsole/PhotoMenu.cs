using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using SharedDataTransfer.DataTransfer;
using SharedProtocol;
using SharedProtocol.Commands;
using SharedProtocol.Protocol;
using Util;
using Util.FileHandler;
using Util.FileHandler.Interfaces;

namespace Client
{
    public  class PhotoMenu
    {
        private readonly IFileHandler _fileHandler;
     

        public PhotoMenu()
        {
            _fileHandler=new FileHandler();
        }
        public String SendPhoto(Socket clientSocket)
        {

            string path = string.Empty;
            String isOk = "OK";
            bool isOKFileName = _fileHandler.GetFileName(path)!=null;
            bool exit = false;
            while (!CorrectInput((!_fileHandler.IsValidPath(path) || !isOKFileName || 
                                  !_fileHandler.CheckFileType(_fileHandler.GetFileName(path))),!exit))
            {
                Console.WriteLine("Ingrese ubicacion del archivo válida, o ingrese exit para volver");
                path = Console.ReadLine();
                if (path.ToLower().Equals("exit")) exit = true;
                isOKFileName =_fileHandler.GetFileName(path)!=null;
            }

            if (path.ToLower().Equals("exit")) return "exit";
            
            string fileName = _fileHandler.GetFileName(path);
            long fileSize = _fileHandler.GetFileSize(path);
            InformationTransfer information = new InformationTransfer();
            InformationHandler infToPrepare=new InformationHandler();
            string[] dataToPrepare = { fileName,fileSize.ToString()};
            byte[] dataPrepared=infToPrepare.PreparedData(dataToPrepare,'#');
            Protocol testProtocol = new Protocol(Constants.REQ,CommandConstants.AddPhoto.GetHashCode(),
                Constants.HEADER_LENGTH+dataPrepared.Length,dataPrepared);
            var headerCodification = new HeaderCodification(testProtocol);
            byte[] encodedFrame = headerCodification.GetRequest();
            try
            {
                information.SendData(clientSocket, encodedFrame);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, "Connection Error");
                isOk = "NO OK";
            }

            try
            { 
                byte[] received = information.ReceiveData(clientSocket);
                Protocol response = headerCodification.DecodeData(received);
                string answer = Encoding.UTF8.GetString(response.Data);
                if (answer != "RECIBO DE HEADER") return "NO OK";
                ArchiveHandler fileToSend=new ArchiveHandler(clientSocket);
                fileToSend.SendFile(fileSize,path);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                isOk = "NO OK";
            }
            return isOk;
        }
        private static bool CorrectInput(bool x, bool y) {
            return ( ( x || y ) && ! ( x && y ) );
        }
    }
}