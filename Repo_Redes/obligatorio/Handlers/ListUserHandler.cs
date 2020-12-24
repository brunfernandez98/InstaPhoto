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
    public class ListUserHandler
    {
        
        private IUserHandler _userService { get; set; }
        private InformationHandler _informationToPrepare { get; set; }
        private InformationTransfer _inf { get; set; }
        private ConnectionManagment _connectionManagment { get; set; }
        private LoggerMSQ _loggerMsq { get; set; }
        public ListUserHandler(ConnectionManagment connectionManagment,IUserHandler userHandler)
        {
            _userService = userHandler;
            _informationToPrepare= new InformationHandler();
            _inf=new InformationTransfer();
            _connectionManagment = connectionManagment;
            _loggerMsq=LoggerMSQ.GetInstance();
        }

        public void ListUserResponse(byte[] data)
        {
            InformationHandler infHandler = new InformationHandler();
            string[] dataToProcess = infHandler.ProcessData(data, '#');
            string message = dataToProcess[0];
           
            Socket client = _connectionManagment.GetSocketFromThread(Thread.CurrentThread);
            String email = _connectionManagment.GetUserFromThread(Thread.CurrentThread).Email;
            if (!message.Equals("QUIERO LA LISTA DE TODOS LOS USUARIOS"))
            {
               
                string messageToSend = "MENSAJE INVALIDO";
                string[] listResponse = {messageToSend.ToString()};
                byte[] dataPrepared=_informationToPrepare.PreparedData(listResponse,'#');
                int lengthToSend = Util.Constants.HEADER_LENGTH + dataPrepared.Length;
                Protocol testProtocol = new Protocol(Util.Constants.RES,
                    CommandConstants.ListClients.GetHashCode(),lengthToSend,dataPrepared);
                var HeaderCodification = new HeaderCodification(testProtocol);
                byte[] encodedFrame = HeaderCodification.GetRequest();
            
                try
                {
                    _inf.SendData(client,encodedFrame);
                    Log oneLog=new Log(email,LogEnum.SeeAllUser,LogEnumSeverity.Error);
                    _loggerMsq.SendMessage(oneLog); 
                }
                catch (Exception msg)
                {
                    Console.WriteLine("Connection error:"+msg);
                    Log oneLog=new Log(email,LogEnum.SeeAllUser,LogEnumSeverity.Error);
                    _loggerMsq.SendMessage(oneLog); 
                }
            }
            else
            {
                ListUserResponseAll(client,email);
            }

              
            
          
        }

        public void ListUserResponseAll(Socket client,String email)
        {
           
            List<Domain.User> listUser = _userService.GetListUsers();
            byte[] dataPrepared=_informationToPrepare.PreparedData(listUser,'#');
            int lengthToSend = Util.Constants.HEADER_LENGTH + dataPrepared.Length;
            Protocol testProtocol = new Protocol(Util.Constants.RES,CommandConstants.ListClients.GetHashCode(),lengthToSend,dataPrepared);
            var HeaderCodification = new HeaderCodification(testProtocol);
            byte[] encodedFrame = HeaderCodification.GetRequest();
            
            try
            {
                Log oneLog=new Log(email,LogEnum.SeeAllUser,LogEnumSeverity.Info);
                _loggerMsq.SendMessage(oneLog); 
                _inf.SendData(client,encodedFrame);
            }
            catch (Exception msg)
            { 
                Log oneLog=new Log(email,LogEnum.SeeAllUser,LogEnumSeverity.Error);
                _loggerMsq.SendMessage(oneLog); 
                Console.WriteLine("Connection error:"+msg);
            }

        }
    }
}