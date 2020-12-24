using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Grpc.Net.Client;
using GrpcClient;


namespace AdministrativeServer.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdminController : ControllerBase
    {
        public AdminController()
        {
            
        }
        
        // POST
        [HttpPost]
        public ActionResult<string> Post([FromHeader]string nombre, string password, string email, string apellido)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport",true);
            Console.WriteLine("Starting gRPC client example......");
            var channel = GrpcChannel.ForAddress("http://localhost:5005");
            var client = new Greeter.GreeterClient(channel);
            
            var responseActive =  client.StartService(new Service());
            Console.WriteLine("Respuesta: " + responseActive.Message);
            
            
            var response =  await client.SayHelloAsync(new HelloRequest());
            Console.WriteLine("Respuesta: " + response.Message);

            var userList = await client.GiveMeOneUserAsync(new User{Email = email,Lastname = apellido,Name = nombre,Password = password});
            Console.WriteLine(userList);
            try
            {
                
            return Ok(userList);
            }catch(Exception e){
                return BadRequest(e.Message);
            }
        }

        // PUT 
        [HttpPut]
        public ActionResult<string> Put([FromHeader]string opcion ,string valor,string email)
        {
            try{
              
                return Ok("ok");
            }catch(Exception e){
                return BadRequest(e.Message);
            }
        }

        // DELETE 
        [HttpDelete("{id}")]
        public IActionResult Delete([FromHeader]string email)
        {
            try{
                
                return Ok("Se borro el Admin " +email);
            }catch(Exception e){
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        public ActionResult<string> Get()
        {
            return Ok("COnectadoo");
        }
    }
}