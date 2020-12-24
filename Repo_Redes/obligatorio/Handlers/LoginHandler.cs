using System;
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
    public class LoginHandler
    {
        
        private IUserHandler _userService { get; set; }
        private InformationHandler _informationToPrepare { get; set; }
        private InformationTransfer _inf { get; set; }
        
        private ConnectionManagment _connectionManagment { get; set; }
        private LoggerMSQ _loggerMsq { get; set; }

        public LoginHandler(ConnectionManagment connectionManagment)
        {
            _userService=new UserHandler();
            _informationToPrepare= new InformationHandler();
            _inf=new InformationTransfer();
            _connectionManagment = connectionManagment;
            _loggerMsq=LoggerMSQ.GetInstance();
        }

        public void LoginResponse(byte[] data)
        {
            InformationHandler infHandler=new InformationHandler();
            string[] dataToProcess= infHandler.ProcessData(data, '#');
            string email = dataToProcess[0];
            string userPassword=dataToProcess[1];
            bool isRegistered = _userService.IsValidUser(email,userPassword);
            string responseText = "";
           
            if (isRegistered)
            {
                responseText = "USUARIO VALIDO";
                Log oneLog=new Log(email,LogEnum.LoginUser,LogEnumSeverity.Info);
                _loggerMsq.SendMessage(oneLog);
            }
        
            else
            {
                responseText = "USUARIO NO VALIDO";
                Log oneLog=new Log(email,LogEnum.LoginUser,LogEnumSeverity.Warning);
                _loggerMsq.SendMessage(oneLog);
            }

            Socket client = _connectionManagment.GetSocketFromThread(Thread.CurrentThread);
            Domain.User realUser = _userService.GetUser(email);
            _connectionManagment.SetUserFromThread(Thread.CurrentThread,realUser);
            string[] loginResponse = {responseText};
            byte[] dataPrepared=_informationToPrepare.PreparedData(loginResponse,'#');
            int lengthToSend = Util.Constants.HEADER_LENGTH + dataPrepared.Length;
            Protocol testProtocol = new Protocol(Util.Constants.RES,CommandConstants.Login.GetHashCode()
                ,lengthToSend,dataPrepared);
            var HeaderCodification = new HeaderCodification(testProtocol);
            byte[] encodedFrame = HeaderCodification.GetRequest();
            
            try
            {
                _inf.SendData(client,encodedFrame);
                
            }
            catch (Exception msg)
            {
                Log oneLog=new Log(realUser.Email,LogEnum.LoginUser,LogEnumSeverity.Error);
                _loggerMsq.SendMessage(oneLog);
                Console.WriteLine("Connection error:"+msg);
            }
          
        }
        public void LogoutResponse(byte[] data)
        {
            InformationHandler infHandler=new InformationHandler();
            string[] dataToProcess= infHandler.ProcessData(data, '#');
            string message = dataToProcess[0];
           
            string responseText = "";
            if (message== "Desloguearme")
            {
                responseText = "OK";
               
            }
            

            Socket client = _connectionManagment.GetSocketFromThread(Thread.CurrentThread);
            string email = _connectionManagment.GetUserFromThread(Thread.CurrentThread).Email;
            string[] loginResponse = {responseText};
            byte[] dataPrepared=_informationToPrepare.PreparedData(loginResponse,'#');
            int lengthToSend = Util.Constants.HEADER_LENGTH + dataPrepared.Length;
            Protocol testProtocol = new Protocol(Util.Constants.RES,CommandConstants.Login.GetHashCode()
                ,lengthToSend,dataPrepared);
            var HeaderCodification = new HeaderCodification(testProtocol);
            byte[] encodedFrame = HeaderCodification.GetRequest();
           
            try
            {
             
                _inf.SendData(client,encodedFrame);
                _connectionManagment.RemoveConnection(Thread.CurrentThread);
                client.Shutdown(SocketShutdown.Both);
                client.Close();
                Log oneLog=new Log(email,LogEnum.SignOut,LogEnumSeverity.Info);
                _loggerMsq.SendMessage(oneLog);
            }
            catch (Exception msg)
            {
                Console.WriteLine("Connection error:"+msg);
                Log oneLog=new Log(email,LogEnum.SignOut,LogEnumSeverity.Error);
                _loggerMsq.SendMessage(oneLog);
            }
          
        }
    }
}