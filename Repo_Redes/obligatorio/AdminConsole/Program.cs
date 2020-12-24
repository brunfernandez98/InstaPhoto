using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


namespace AdminConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            MainMenu();
        }

        public static void MainMenu()
        {
            var userInput = 1;
            Console.WriteLine("Ingrese la opcion deseada");
            Console.WriteLine("1- Agregar Nuevo Usuario");
            Console.WriteLine("2- Editar Usuario");
            Console.WriteLine("3- Eliminar Usuario");
            Console.WriteLine("4- Consultar logs");
            Console.WriteLine("5- SALIR");
            Int32.TryParse(Console.ReadLine(), out userInput);
            var email = "";
            var nombre = "";
            var apellido = "";
            var password = "";
            switch (userInput){
                case 1:
                    Console.WriteLine("Ingrese el Nombre");
                    nombre = Console.ReadLine();
                    Console.WriteLine("Ingrese el Apellido");
                    apellido = Console.ReadLine();
                    Console.WriteLine("Ingrese el Email");
                    email = Console.ReadLine();
                    Console.WriteLine("Ingrese el Password");
                    password = Console.ReadLine();
                    if (email !="" && password!="" && nombre!="" && apellido!=""){
                        PostUser(nombre, apellido, email, password);
                    }else
                    {
                        Console.WriteLine("Los datos no pueden ser Vacios");
                    }
                    MainMenu();
                    break;
                case 2:
                    Console.WriteLine("Ingrese el Email del Usuario que quiere modificar");
                    email = "";
                    email = Console.ReadLine();
                    Console.WriteLine("Ingrese el campo que quiere modificar");
                    Console.WriteLine("1- Email");
                    Console.WriteLine("2- Nombre");
                    Console.WriteLine("3- Apellido");
                    Console.WriteLine("4- Password");
                    var eleccionModificador=0;
                    Int32.TryParse(Console.ReadLine(),out eleccionModificador);
                    var amodificar = "";
                    Console.WriteLine(eleccionModificador);
                    switch (eleccionModificador)
                    {
                        case 1:
                            amodificar = "EMAIL";
                            break;
                        case 2:
                            amodificar = "NOMBRE";
                            break;
                        case 3:
                            amodificar = "APELLIDO";
                            break;
                        case 4:
                            amodificar = "PASSWORD";
                           
                            break;
                        default:
                            Console.WriteLine("Opcion elegida no es correcta.");
                            MainMenu();
                            break;
                    }
                    Console.WriteLine("Ingrese el nuevo valor de "+ amodificar.ToLower());
                    var nuevoValor = Console.ReadLine();
                    if (email != "" && amodificar != "" && nuevoValor != "")
                    {
                        ModifyUser(email, amodificar, nuevoValor);
                    }else
                    {
                        Console.WriteLine("Ingreso Valores Incorrectos");
                    }
                    MainMenu();
                        break;
                case 3:
                    Console.WriteLine("Ingrese el Email del Usuario que quiere eliminar");
                    email = "";
                    email = Console.ReadLine();
                    if (email!=""){
                        DeleteUser(email);
                    }
                    MainMenu();
                    break;
                case 4:
                    Console.WriteLine("Puede consultar los Logs de manera mas simple en la webpage");
                    GetLogs();
                    MainMenu();
                    break;
                case 5:
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Opcion elegida no es correcta.");
                    MainMenu();
                    break;
            }
            
        }
        public static async Task PostUser(string nombre, string apellido, string email, string password){
            using (var client = new WebClient())
            {
                string url = "http://localhost:5055/WeatherForecast"; // Just a sample url
                WebClient wc = new WebClient();
 
                wc.QueryString.Add("nombre", nombre);
                wc.QueryString.Add("apellido", apellido);
                wc.QueryString.Add("email", email);
                wc.QueryString.Add("password", password);
 
                var data = wc.UploadValues(url, "POST", wc.QueryString);
                Console.WriteLine(data);
 
// data here is optional, in case we recieve any string data back from the POST request.
               
            }
        }
        
        public static async Task ModifyUser(string email, string amodificar, string nuevoValor){
            using (var client = new WebClient())
            {
                string url = "http://localhost:5055/WeatherForecast"; // Just a sample url
                WebClient wc = new WebClient();
 
                wc.QueryString.Add("opcion", amodificar);
                wc.QueryString.Add("valor", nuevoValor);
                wc.QueryString.Add("email", email);

                var data = wc.UploadValues(url, "PUT", wc.QueryString);
                Console.WriteLine(data);

            }
        }
        
        public static async Task DeleteUser(string email){
            using (var client = new WebClient())
            {
                string url = "http://localhost:5055/WeatherForecast"; // Just a sample url
                WebClient wc = new WebClient();
                wc.QueryString.Add("email", email);

                var data = wc.UploadValues(url, "DELETE", wc.QueryString);
                Console.WriteLine(data);

            }
        }
        public static async Task GetLogs(){
            using (var client = new WebClient())
            {
                string url = "http://localhost:5010/Logs"; // Just a sample url
                WebClient wc = new WebClient();
                var data = wc.UploadValues(url, "GET", wc.QueryString);
                Console.WriteLine(data);

            }
        }
    }

}