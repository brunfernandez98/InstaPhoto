using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace AdministrariveClientConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            MainMenu();
        }
        
        public static async Task MainMenu(){
            Console.WriteLine("Ingrese la Opcion Deseada:");
            Console.WriteLine("1- Ver Logs");
            Console.WriteLine("2- Agregar un Usuario");
            Console.WriteLine("3- Modificar un Usuario");
            Console.WriteLine("4- Eliminar un Usuario");
            var opcion = 0;
            Int32.TryParse(Console.ReadLine(), out opcion);
            switch (opcion)
            {
                case 1:
                {
                    //Console.WriteLine("Opcion 11");
                    using var client = new HttpClient();
                    WebRequest request = WebRequest.Create ("http://localhost:5009/Logs");
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse ();
                   // var content =  client.GetStringAsync("http://localhost:5009/Swagger");
                    //var result = client.GetAsync("http://google.com");
                    //var result = client.GetAsync("http://localhost:5009/Logs");
                    Console.WriteLine(response);
                    //Console.WriteLine("Opcion 1 final");

                    MainMenu();
                    break;
                }
                case 2:
                    Console.WriteLine("Opcion 2");
                    break;
                case 3:
                    Console.WriteLine("Opcion 3");
                    break;
                case 4:
                    Console.WriteLine("Opcion 4");
                    break;
            }
            
        }
    }
   
}