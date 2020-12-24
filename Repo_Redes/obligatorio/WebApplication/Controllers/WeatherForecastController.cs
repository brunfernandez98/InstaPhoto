using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using GrpcClient;

namespace WebApplication.Controllers
{
    [Route("api/admin")]

    public class WeatherForecastController : ControllerBase
    {
        public WeatherForecastController()
        {
            
        }

        [HttpPost]
        public ActionResult<int> Post([FromHeader] string name,string nombre, string password, string email, string apellido)
        {
            try{
                var a = GrpcClient.Program.AddUser(email, nombre, apellido, password).ToString();
                Console.WriteLine("llego " +nombre +" "+ apellido+" "+email+" "+password);
                Console.WriteLine(a);
                 Console.WriteLine(a);
                return Ok(1);
            }catch(Exception e){
                return BadRequest(e.Message + "error acaaa");
            }
        }

        // PUT 
        [HttpPut]
        public ActionResult<string> Put([FromQuery]string name,string opcion ,string valor,string email)
        {
            try{
                Console.WriteLine("PUT");
                Console.WriteLine(opcion+valor+email);
                //Console.WriteLine(opcion + " "+ valor+ " "+ email);
                var a = "";
                switch (opcion)
                {
                    case "PASSWORD":
                         a = GrpcClient.Program.ModifyPasswordUser(valor, email).ToString();
                        break;
                    case "NOMBRE":
                        a = GrpcClient.Program.ModifyNameUser(valor, email).ToString();
                        break;
                    case "APELLIDO":
                        a = GrpcClient.Program.ModifyLastNameUser(valor, email).ToString();
                        break;
                    case "EMAIL":
                        a = GrpcClient.Program.ModifyEmailUser(valor, email).ToString();
                        break;
                }
                
                return Ok("ok");
            }catch(Exception e){
                return BadRequest(e.Message);
            }
        }

        // DELETE 
        [HttpDelete]
        public IActionResult Delete([FromQuery]string email)
        {
            try{
                Console.WriteLine("delete");
                Console.WriteLine(email);
                GrpcClient.Program.DeleteUser(email);
                return Ok("Se borro el Admin " +email);
            }catch(Exception e){
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        public ActionResult<string> Get(string email)
        {
           
            GrpcClient.Program.AddUser("email@email.com", "nombre", "apellido", "password");
            return Ok("Conectado a la webapi Administracion correctamente");
        }
    }
}