using System;
using System.Threading.Tasks;
using Grpc.Net.Client;
using GrpcService;


namespace GrpcClient
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport",true);
            Console.WriteLine("Iniciando el Cliente GRPC.....");
            var channel = GrpcChannel.ForAddress("http://localhost:5005");
            var client = new Greeter.GreeterClient(channel);
            
            var responseActive =  client.StartService(new Service());
            Console.WriteLine("Respuesta: " + responseActive.Message);

            Console.WriteLine("EMAIL");
            var email = Console.ReadLine();
            Console.WriteLine("password");
            var password = Console.ReadLine();

            var userResponseModify = await client.ModifyPasswordOneUserAsync(new UserModifyRequest{Email = email,Password = password});
            Console.WriteLine(userResponseModify);
        
        }

        public static async Task<UserDeleteResponse> DeleteUser(string email)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport",true);
            Console.WriteLine("Starting gRPC client example......");
            var channel = GrpcChannel.ForAddress("http://localhost:5005");
            var client = new Greeter.GreeterClient(channel);
            var responseActive =  client.StartService(new Service());
            Console.WriteLine("Respuesta: " + responseActive.Message);
            var userResponse = await client.DeleteOneUserAsync(new Email{EmailUser = email});
            return userResponse;

        }
        
        public static async Task<UserModifyResponse> ModifyEmailUser(string email,string emailOriginal)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport",true);
            Console.WriteLine("Starting gRPC client example......");
            var channel = GrpcChannel.ForAddress("http://localhost:5005");
            var client = new Greeter.GreeterClient(channel);
            var responseActive =  await client.SayHelloAsync(new HelloRequest());
            Console.WriteLine("Respuesta: " + responseActive.Message);
            var userResponseModify = await client.ModifyEmailOneUserAsync(new UserModifyRequest{Email = emailOriginal,EmailNew = email});
            return userResponseModify;

        }
        
        public static async Task<UserModifyResponse> ModifyLastNameUser(string lastName,string emailOriginal)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport",true);
            Console.WriteLine("Starting gRPC client example......");
            var channel = GrpcChannel.ForAddress("http://localhost:5005");
            var client = new Greeter.GreeterClient(channel);
            var responseActive =  await client.SayHelloAsync(new HelloRequest());
            Console.WriteLine("Respuesta: " + responseActive.Message);
            var userResponseModify = await client.ModifyLastNameOneUserAsync(new UserModifyRequest{Email = emailOriginal,Lastname = lastName});
            return userResponseModify;

        }
        
        public static async Task<UserModifyResponse> ModifyPasswordUser(string password,string emailOriginal)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport",true);
            Console.WriteLine("Starting gRPC client example......");
            var channel = GrpcChannel.ForAddress("http://localhost:5005");
            var client = new Greeter.GreeterClient(channel);
            var responseActive =  await client.SayHelloAsync(new HelloRequest());
            Console.WriteLine("Respuesta: " + responseActive.Message);
            var userResponseModify = await client.ModifyPasswordOneUserAsync(new UserModifyRequest{Email = emailOriginal,Password = password});
            return userResponseModify;

        }
        
        
        
        public static async Task<UserModifyResponse> ModifyNameUser(string name,string emailOriginal)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport",true);
            Console.WriteLine("Starting gRPC client example......");
            var channel = GrpcChannel.ForAddress("http://localhost:5005");
            var client = new Greeter.GreeterClient(channel);
            var responseActive =  await client.SayHelloAsync(new HelloRequest());
            Console.WriteLine("Respuesta: " + responseActive.Message);
            var userResponseModify = await client.ModifyNameOneUserAsync(new UserModifyRequest{Email = emailOriginal,Name = name});
            return userResponseModify;

        }
        
        
        
        
        
        
       public static async Task<UserRegisterResponse> AddUser(string email,string name,string lastName,string password)

        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport",true);
            Console.WriteLine("Iniciando el Cliente GRPC.....");
            var channel = GrpcChannel.ForAddress("http://localhost:5005");
            var client = new Greeter.GreeterClient(channel);
            var responseActive =  client.StartService(new Service());
            Console.WriteLine("Respuesta: " + responseActive.Message);
            var response =  await client.SayHelloAsync(new HelloRequest());
            Console.WriteLine("Respuesta: " + response.Message);
            var userList = await client.GiveMeOneUserAsync(new User{Email = email,Lastname = lastName,Name = name,Password = password});
            Console.WriteLine("{0}",userList);
            return userList;
        }
    }
}