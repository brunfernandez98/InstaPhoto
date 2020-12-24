using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using SystemService;
using Handlers.User;
using Server.RBMSQ;
using SharedDataTransfer.DataTransfer;
using SharedLog;
using SharedLogConstant;
using SharedProtocol;
using SharedProtocol.Commands;
using SharedProtocol.Protocol;

namespace Handlers
{
    public class ListPhotoHandler
    {
        private IUserHandler _userService { get; set; }
        private InformationHandler _informationToPrepare { get; set; }
        private InformationTransfer _inf { get; set; }
        private ConnectionManagment _connectionManagment { get; set; }
        private LoggerMSQ _loggerMsq { get; set; }
        public ListPhotoHandler(ConnectionManagment connectionManagment)
        {
            _userService=new UserHandler();
            _informationToPrepare= new InformationHandler();
            _inf=new InformationTransfer();
            _connectionManagment = connectionManagment;
            _loggerMsq=LoggerMSQ.GetInstance();
        }
        public void ListPhotoResponse(byte[] data)
        {
            InformationHandler infHandler = new InformationHandler();
            string[] dataToProcess = infHandler.ProcessData(data, '#');
            string message = dataToProcess[0];
            string user=dataToProcess[1];
            Socket client = _connectionManagment.GetSocketFromThread(Thread.CurrentThread);
            
            if (!message.Equals("QUIERO LA LISTA DE FOTOS"))
            {
               
                string messageToSend = "MENSAJE INVALIDO";
                string[] listResponse = {messageToSend};
                byte[] dataPrepared=_informationToPrepare.PreparedData(listResponse,'#');
                int lengthToSend = Util.Constants.HEADER_LENGTH + dataPrepared.Length;
                Protocol testProtocol = new Protocol(Util.Constants.RES,
                    CommandConstants.ListClients.GetHashCode(),lengthToSend,dataPrepared);
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
            else
            {
                ListUserResponseAll(user,client);
            } 
        }
        public void ListUserResponseAll(string oneUser,Socket client)
        {

          
            byte[] dataPrepared = { };

            int lengthToSend = 0; 
            if (_userService.IsRegisterUser(oneUser))
            {
                Domain.User getUser = _userService.GetUser(oneUser);
                ICollection<string> listPhotoName = _userService.GetListOfPhotoUsers(getUser);
              dataPrepared =  _informationToPrepare.PreparedData(listPhotoName,'#');
                lengthToSend = Util.Constants.HEADER_LENGTH + dataPrepared.Length;
                Log oneLog=new Log(oneUser,LogEnum.SeePhotoOneUser,LogEnumSeverity.Info);
                _loggerMsq.SendMessage(oneLog);
            }
            else
            {
                string[] listResponse= {"USUARIO NO VALIDO"} ;
                dataPrepared= _informationToPrepare.PreparedData(listResponse,'#');
                lengthToSend = Util.Constants.HEADER_LENGTH + dataPrepared.Length;
                Log oneLog=new Log(oneUser,LogEnum.SeeAllUser,LogEnumSeverity.Warning);
                _loggerMsq.SendMessage(oneLog); 
            }
           
            
            Protocol testProtocol = new Protocol(Util.Constants.RES,CommandConstants.ListPhoto.GetHashCode(),
                lengthToSend,dataPrepared);
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