

using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Grpc.Core;
using GrpcService;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Server.RBMSQ;
using SharedDataTransfer.DataTransferMQ;

using SharedProtocol.Protocol;



namespace GrpcSampleService
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        private static LoggerAdmin _loggerAdmin;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
           
        }
        
        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                
                Message = "Bienvenido al servidor Administrativo "
            });
        }
        public override Task<UserDeleteResponse> DeleteOneUser(Email request, ServerCallContext context)
        {
            return Task.FromResult(new UserDeleteResponse(){Message = Delete(request)});
           
        }
        
        public override Task<StartServiceResponse> StartService(Service request, ServerCallContext context)
        {
            return Task.FromResult((new StartServiceResponse() {Message = StartServiceProduce()}));
        }
        public override Task<UserRegisterResponse> GiveMeOneUser(User request, ServerCallContext context)
        {
            return Task.FromResult(new UserRegisterResponse() {Message = Register(request)});
           
        }

        public override Task<UserModifyResponse> ModifyEmailOneUser(UserModifyRequest request, ServerCallContext context)
        {
            return Task.FromResult(new UserModifyResponse(){Message = ModifyUserEmail(request)});
        }
        
        public override Task<UserModifyResponse> ModifyPasswordOneUser(UserModifyRequest request, ServerCallContext context)
        {
            return Task.FromResult(new UserModifyResponse(){Message = ModifyUserPassword(request)});
        }
        public override Task<UserModifyResponse> ModifyNameOneUser(UserModifyRequest request, ServerCallContext context)
        {
            return Task.FromResult(new UserModifyResponse(){Message = ModifyUserName(request)});
        }
        public override Task<UserModifyResponse> ModifyLastNameOneUser(UserModifyRequest request, ServerCallContext context)
        {
            return Task.FromResult(new UserModifyResponse(){Message = ModifyUserLastName(request)});
        }

       
        private static string StartServiceProduce(){
            _loggerAdmin= new LoggerAdmin();
            return "Servicio iniciado";
        }
        private static string Register(User request)
        {
            Domain.User userToCreate = new Domain.User() {Email = request.Email, Password = request.Password,Name = request.Name,LastName = request.Lastname};
            userToCreate = new Domain.User() {Email = request.Email, Password = request.Password,Name = request.Name
                ,LastName = request.Lastname};
            var oneMessage=new InformationAdmin(userToCreate,InfoAdminEnum.Register,"",Option.Request);
            _loggerAdmin.SendMessage(oneMessage);
            Thread.Sleep(1000);
            return Receive("queueResponse");

        }
        private static string Delete(Email request)
           {
               var userToCreate = new Domain.User() {Email = request.EmailUser};
               var oneMessage=new InformationAdmin(userToCreate,InfoAdminEnum.Delete,"",Option.Request);
               _loggerAdmin.SendMessage(oneMessage);
               Thread.Sleep(1000);
               return Receive("queueResponse");

           }
        private static string ModifyUserEmail(UserModifyRequest request)
           {
               var userToModify = new Domain.User() {Email = request.EmailNew, Password = request.Password,Name = request.Name
                   ,LastName = request.Lastname};
               var oneMessage=new InformationAdmin(userToModify,InfoAdminEnum.ModifyEmail,""+request.Email,Option.Request);
               _loggerAdmin.SendMessage(oneMessage);
               Thread.Sleep(1000);
               return Receive("queueResponse");

           }
        
        private static string ModifyUserPassword(UserModifyRequest request)
        {
            var userToModify = new Domain.User() {Email = request.EmailNew, Password = request.Password,Name = request.Name
                ,LastName = request.Lastname};
            var oneMessage=new InformationAdmin(userToModify,InfoAdminEnum.ModifyPassword,""+request.Email,Option.Request);
            _loggerAdmin.SendMessage(oneMessage);
            Thread.Sleep(1000);
            return Receive("queueResponse");

        }
        
        private static string ModifyUserName(UserModifyRequest request)
        {
            var userToModify = new Domain.User() {Email = request.EmailNew, Password = request.Password,Name = request.Name
                ,LastName = request.Lastname};
            var oneMessage=new InformationAdmin(userToModify,InfoAdminEnum.ModifyName,""+request.Email,Option.Request);
            _loggerAdmin.SendMessage(oneMessage);
            Thread.Sleep(1000);
            return Receive("queueResponse");

        }
        
        private static string ModifyUserLastName(UserModifyRequest request)
        {
            var userToModify = new Domain.User() {Email = request.EmailNew, Password = request.Password,Name = request.Name
                ,LastName = request.Lastname};
            var oneMessage=new InformationAdmin(userToModify,InfoAdminEnum.ModifyLastName,""+request.Email,Option.Request);
            _loggerAdmin.SendMessage(oneMessage);
            Thread.Sleep(1000);
            return Receive("queueResponse");

        }
        
        private static string Receive(string queue)
           {
               
               string message="";
               try
               {
                   using (IConnection connection = new ConnectionFactory().CreateConnection())
                   {
                       using (IModel channel = connection.CreateModel())
                       {
                           channel.QueueDeclare(queue, false, false, false, null);
                           BasicGetResult result = channel.BasicGet(queue, true);

                           if (result == null) return "Error";
                           var information = JsonSerializer.Deserialize<InformationAdmin>(result.Body.ToArray());
                           message= information.MessageTransfer;
                       }
                   }
                   
               }catch (Exception e)
               {
                Console.WriteLine(e.Message);
               }

               return message;
           }
        

        

        

    }
}