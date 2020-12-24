using System;
using System.Net.Sockets;
using System.Threading;
using SystemService;
using Handlers.User;
using Handlers.User.Exceptions;
using Server.RBMSQ;
using SharedDataTransfer.DataTransfer;
using SharedLog;
using SharedLogConstant;
using SharedProtocol;
using SharedProtocol.Commands;
using SharedProtocol.Protocol;

namespace Handlers
{
    public class RegisterHandler
    {
        private ConnectionManagment _connectionManagment { get; set; }
        private IUserHandler _userService { get; set; }
        private InformationHandler _informationToPrepare { get; set; }
        private InformationTransfer _inf { get; set; }
        private LoggerMSQ _loggerMsq { get; set; }
        public RegisterHandler(ConnectionManagment connectionManagment,IUserHandler userHandler)
        {
            _userService = userHandler;
            _connectionManagment = connectionManagment;
            _informationToPrepare= new InformationHandler();
            _inf=new InformationTransfer();
            _loggerMsq=LoggerMSQ.GetInstance();
        }

        public void RegisterResponse(byte[] data)
        {
            InformationHandler infHandler=new InformationHandler();
            string[] dataToProcess= infHandler.ProcessData(data, '#');
            string password = dataToProcess[3];
            string email =dataToProcess[2];
            Domain.User newUser = null;
           
            bool isRegistered = _userService.IsRegisterUser(email);
            
            string responseText = "";
            
            if (isRegistered)
            {
               
                responseText = "USUARIO YA REGISTRADO";
                
            }
        
            else
            {
                var name = dataToProcess[0];
                var lastName = dataToProcess[1];
                try
                {
                
                    newUser= _userService.Register(email,password,name,lastName);
                    responseText = "USUARIO REGISTRADO";
                }
                catch (EmailInvalidException e)
                {
                  
                    responseText = "USUARIO NO PUDO SER REGISTRADO " + e.Message;
                }
                catch (PasswordInvalidException e)
                {
                    
                    responseText = "USUARIO NO PUDO SER REGISTRADO "+ e.Message;
                }
            }
           
            
            
            
            _connectionManagment.SetUserFromThread(Thread.CurrentThread,newUser);
            Socket client = _connectionManagment.GetSocketFromThread(Thread.CurrentThread);
            string[] registerResponse = {responseText};
            byte[] dataPrepared=_informationToPrepare.PreparedData(registerResponse,'#');
            int lengthToSend = Util.Constants.HEADER_LENGTH + dataPrepared.Length;
            Protocol testProtocol = new Protocol(Util.Constants.RES,
                CommandConstants.SignUp.GetHashCode(),lengthToSend,dataPrepared);
            var HeaderCodification = new HeaderCodification(testProtocol);
            byte[] encodedFrame = HeaderCodification.GetRequest();
            
            try
            {
               
                _inf.SendData(client,encodedFrame);
                Log oneLog = null;
                if (responseText != "USUARIO NO PUDO SER REGISTRADO")
                {
                    oneLog = new Log(email,LogEnum.CreateUser,LogEnumSeverity.Info);
                }
                else
                {
                    oneLog = new Log(email,LogEnum.CreateUser,LogEnumSeverity.Warning);
                }
                _loggerMsq.SendMessage(oneLog);
            }
            catch (Exception msg)
            {
                Console.WriteLine("Connection error:"+msg);
                Log oneLog=new Log(email,LogEnum.CreateUser,LogEnumSeverity.Error);
                _loggerMsq.SendMessage(oneLog);
            }
          
        }
    }
    }
