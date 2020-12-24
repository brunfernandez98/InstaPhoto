using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using SystemService;
using Domain;
using Handlers.User;
using Handlers.User.Exceptions;
using Server.RBMSQ;
using SharedDataTransfer.DataTransfer;
using SharedLog;
using SharedLogConstant;
using SharedProtocol;
using SharedProtocol.Commands;
using SharedProtocol.Protocol;
using Util;


namespace AdminServer
{
    public class PhotoManager
    {
        
        private ConnectionManagment _connectionManagment { get; set; }
        private IUserHandler _userHandler { get; set; }
        private InformationHandler _informationToPrepare { get; set; }
        private InformationTransfer _inf { get; set; }
        private LoggerMSQ _loggerMsq { get; set; }

        public PhotoManager(ConnectionManagment connectionManagment,IUserHandler userHandler) {
            _connectionManagment = connectionManagment;
            _userHandler = userHandler;
            _inf=new InformationTransfer();
            _informationToPrepare=new InformationHandler();
            _loggerMsq=LoggerMSQ.GetInstance();
        }
        public void ManageAndSaveFile(byte[]data)
        {

            Socket client = _connectionManagment.GetSocketFromThread(Thread.CurrentThread);
            String email = _connectionManagment.GetUserFromThread(Thread.CurrentThread).Email;
            InformationHandler infHandler=new InformationHandler();
            string[] dataToProcess= infHandler.ProcessData(data, '#');
            Int64.TryParse(dataToProcess[1], out var fileSize);
            string fileName = dataToProcess[0];
            string responseText = "";
            responseText = _connectionManagment.OneSystem.ExistPhoto(fileName) ? "NOMBRE DUPLICADO" : "RECIBO DE HEADER";

            string[] loginResponse = {responseText};
            byte[] dataPrepared=_informationToPrepare.PreparedData(loginResponse,'#');
            int lengthToSend = Constants.HEADER_LENGTH + dataPrepared.Length;
            Protocol testProtocol = new Protocol(Util.Constants.RES,CommandConstants.AddPhoto.GetHashCode()
                ,lengthToSend,dataPrepared);
            var HeaderCodification = new HeaderCodification(testProtocol);
            byte[] encodedFrame = HeaderCodification.GetRequest();
            InformationTransfer infoToResponse = new InformationTransfer();
            
             
            try
            {
               
                infoToResponse.SendData(client, encodedFrame);
              
            }
            catch (Exception msg)
            {
                Console.WriteLine("Connection error:"+msg);
                Log oneLog=new Log(email,LogEnum.UploadPhoto,LogEnumSeverity.Error);
                _loggerMsq.SendMessage(oneLog);
            }

            if (!responseText.Equals("NOMBRE DUPLICADO"))
            {
                
                    bool existUser = true;
                    try
                    {
                        var userToInsertPhoto = _connectionManagment.GetUserFromThread(Thread.CurrentThread);
                        Log oneLog=new Log(email,LogEnum.UploadPhoto,LogEnumSeverity.Info);
                        _loggerMsq.SendMessage(oneLog);
                        ArchiveHandler archiveToReceive=new ArchiveHandler(client);
                        archiveToReceive.ReceiveFile(fileSize,fileName);
                        CreatePhotoInSystem(fileName,userToInsertPhoto);
                    }
                    catch (NullReferenceException e)
                    {
                        Console.WriteLine(e.Message);
                       
                        Log oneLog=new Log(email,LogEnum.UploadPhoto,LogEnumSeverity.Error);
                        _loggerMsq.SendMessage(oneLog);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        Log oneLog=new Log(email,LogEnum.UploadPhoto,LogEnumSeverity.Error);
                        _loggerMsq.SendMessage(oneLog);
                    }
            }
            else
            {
                Log oneLog=new Log(email,LogEnum.UploadPhoto,LogEnumSeverity.Warning);
                _loggerMsq.SendMessage(oneLog);   
            }
           
           
            
        }

        private void CreatePhotoInSystem(string nameFile,User oneUser)
        {
            Photo newPhoto= new Photo(nameFile);
            _userHandler.InsertPhoto(oneUser,newPhoto);
            _connectionManagment.OneSystem.UsersPhotos.Add(newPhoto);
            
        }
        
        public void CommentPhoto(byte[] data)
        {
           User oneUser= _connectionManagment.GetUserFromThread(Thread.CurrentThread);
           string email = oneUser.Email;
            string[] dataToProcess= _informationToPrepare.ProcessData(data, '#');
            string namePhoto = dataToProcess[0];
            string commentOfPhoto = dataToProcess[1];
            bool existPhoto = _connectionManagment.OneSystem.ExistPhoto(namePhoto);
            
            string responseText = "";
            if (!existPhoto)
            {
                responseText = "FOTO NO VALIDA";
                Log oneLog=new Log(email,LogEnum.CommentOnePhoto,LogEnumSeverity.Warning);
                _loggerMsq.SendMessage(oneLog); 
                
            }else{
                try
                {
                    Photo onePhoto = _connectionManagment.OneSystem.GetPhoto(namePhoto);
                    onePhoto.AddComment(commentOfPhoto,oneUser.Email);
                    Log oneLog=new Log(email,LogEnum.CommentOnePhoto,LogEnumSeverity.Info);
                    _loggerMsq.SendMessage(oneLog); 
                    responseText = "COMENTARIO REGISTRADO";
                    
                }
                catch (CommentInvalidException e)
                {
                    Log oneLog=new Log(email,LogEnum.CommentOnePhoto,LogEnumSeverity.Warning);
                    _loggerMsq.SendMessage(oneLog); 
                    Console.WriteLine(e.Message);
                    responseText = "COMENTARIO NO PUDO SER REGISTRADO";
                    
                }
                
            }

            Socket client = _connectionManagment.GetSocketFromThread(Thread.CurrentThread);
            string[] registerResponse = {responseText};
            byte[] dataPrepared=_informationToPrepare.PreparedData(registerResponse,'#');
            int lengthToSend = Constants.HEADER_LENGTH + dataPrepared.Length;
            Protocol testProtocol = new Protocol(Constants.RES,CommandConstants.AddPhoto.GetHashCode()
                ,lengthToSend,dataPrepared);
            var HeaderCodification = new HeaderCodification(testProtocol);
            byte[] encodedFrame = HeaderCodification.GetRequest();
            
            try
            {
                _inf.SendData(client,encodedFrame);
            }
            catch (Exception msg)
            {
                Console.WriteLine("Connection error:"+msg);
            }
          
        }
        
        
        public void ListComment(byte[] data)
        {
            InformationHandler infHandler = new InformationHandler();
            string[] dataToProcess = infHandler.ProcessData(data, '#');
            string photoToView = dataToProcess[0];
            bool existPhoto = _connectionManagment.OneSystem.ExistPhoto(photoToView);
            Socket client = _connectionManagment.GetSocketFromThread(Thread.CurrentThread);
            String email = _connectionManagment.GetUserFromThread(Thread.CurrentThread).Email;
            string messageToSend = "";
            byte[] dataPrepared = { };
            
            if (!existPhoto)
            {
                
                messageToSend = "FOTO INVALIDA";
                string[] listResponse = {messageToSend};
                dataPrepared = _informationToPrepare.PreparedData(listResponse, '#');
                Log oneLog=new Log(email,LogEnum.SeeComment,LogEnumSeverity.Warning);
                _loggerMsq.SendMessage(oneLog); 
            }
            else
            { 
                ICollection<Tuple<string, string>> listComments =
                    _connectionManagment.OneSystem.GetCommentsOfPhoto(photoToView);
                dataPrepared=_informationToPrepare.PreparedData(listComments,'#');
                Log oneLog=new Log(email,LogEnum.SeeComment,LogEnumSeverity.Info);
                _loggerMsq.SendMessage(oneLog); 
            }

           
            int lengthToSend = Util.Constants.HEADER_LENGTH + dataPrepared.Length;
            Protocol testProtocol = new Protocol(Util.Constants.RES,
                CommandConstants.SeeComment.GetHashCode(),lengthToSend,dataPrepared);
            var HeaderCodification = new HeaderCodification(testProtocol);
            byte[] encodedFrame = HeaderCodification.GetRequest();
            
            try
            {
                _inf.SendData(client,encodedFrame);
            }
            catch (Exception msg)
            {
                Console.WriteLine("Connection error:"+msg);
            }
                

              
            
          
        }

      
           }
        }
        
    
