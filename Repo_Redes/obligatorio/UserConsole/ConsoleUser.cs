using System;
using System.Net.Sockets;
using Client;
using Handlers;
using Handlers.User;

using SharedDataTransfer.DataTransfer;



namespace UserConsole
{
    public class ConsoleUser
    {
        private InformationTransfer _inf;
        private InformationHandler _informationToPrepare;
        private readonly Socket _client;
        private byte[] _dataPrepared;
        private IUserHandler _userHandler;

        public ConsoleUser(Socket clientSocket)
        {
            _inf = new InformationTransfer();
            _informationToPrepare = new InformationHandler();
            _client = clientSocket;
            _userHandler = new UserHandler();
        }

        
        public void ConsoleMenu()
        {
            
            var userInput = 1;
            do
            {
                PrintMainOptions();
                Console.WriteLine("Ingrese el numero asociado con la funcionalidad que desee: ");
                Int32.TryParse(Console.ReadLine(), out userInput);
                if (userInput > 2 || userInput <= 0) Console.WriteLine("Opción elegida no valida, elija otra");
            } while ((userInput > 2 || userInput <= 0));
            switch (userInput)
            {
                case 1:
                    ConnectToServerConsole();
                    break;
                case 2:
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Opcion elegida no es correcta.");
                    break;
            }

        }

        private void PrintMainOptions()
        {
            Console.WriteLine("1 - Conectarse al servidor.");
            Console.WriteLine("2 - Salir.");
        }

        private void PrintOptionsMain()
        {
            Console.WriteLine("1 - Dar de alta un usuario.");
            Console.WriteLine("2 - Login de usuario.");
            Console.WriteLine("3 - Salir.");

        }
        private void PrintLoggedUserOptions()
        {
            Console.WriteLine("");
            Console.WriteLine("Menu Principal");
            Console.WriteLine("---------------------------");
            Console.WriteLine("1 - Subir foto.");
            Console.WriteLine("2 - Listar clientes");
            Console.WriteLine("3 - Listar fotos");
            Console.WriteLine("4 - Ver comentarios de fotos.");
            Console.WriteLine("5 - Comentar foto de un usuario.");
            Console.WriteLine("6 - Salir.");
            Console.WriteLine("---------------------------");
        }

        private void LoggedUserConsoleMenu()
        {
            var userInput = 1;
            do
            {
                PrintLoggedUserOptions();
                Console.WriteLine("Ingrese el numero asociado con la funcionalidad que desee: ");
                Int32.TryParse(Console.ReadLine(), out userInput);
                if (userInput > 6 || userInput <= 0) Console.WriteLine("Opción elegida no valida, elija otra");
            } while (userInput > 6|| userInput <= 0);

            switch (userInput)
            {
                case 1:
                    PhotoMenu newMenu = new PhotoMenu();
                    String sendPhoto = newMenu.SendPhoto(_client);
                    if (sendPhoto.Equals("OK"))
                    {
                        Console.WriteLine("Se subio correctamente la fotografía.");
                    }
                    else if(sendPhoto.Equals("NO OK"))
                    {
                        Console.WriteLine("Hubo un error al subir la fotografía, intente nuevamente.");
                    }
                    LoggedUserConsoleMenu();
                    break;
                case 2:
                    ListOfUsers listOfUsers=new ListOfUsers();
                    listOfUsers.ListOFUsersManagment(_client);
                    LoggedUserConsoleMenu();
                   
                    break;
                case 3:
                    //Listar fotos de un usuario
                    ListPhotoUser listPhotoUser=new ListPhotoUser();
                    listPhotoUser.ListPhoto(_client);
                    LoggedUserConsoleMenu();
                    break;
                case 4:
                    ViewComment view=new ViewComment();
                    view.ViewCommentsManagment(_client);
                    LoggedUserConsoleMenu();
                    break;
                case 5:
                    CommentPhoto comment=new CommentPhoto();
                    if (comment.CommentThePhoto(_client))
                    {
                        Console.WriteLine("Se pudo comentar la foto correctamente");
                    }
                    else
                    {
                        Console.WriteLine("No se pudo comentar la foto, Intente nuevamente");
                    }
                    LoggedUserConsoleMenu();
                    break; 
                case 6:
                    ConnectToServerConsole();
                    break;
                default:
                    Console.WriteLine("Opcion ingresada no es valida.");
                    break;
            }
        }

        private void ConnectToServerConsole()
        {
            var userInput = 1;
            do
            {
                PrintOptionsMain();
                Console.WriteLine("Ingrese el numero asociado con la funcionalidad que desee: ");
                Int32.TryParse(Console.ReadLine(), out userInput);
                if (userInput > 3 || userInput <= 0) Console.WriteLine("Opción elegida no valida, elija otra");
            } while (userInput > 3 || userInput <= 0);
            switch (userInput)
            {
                case 1:
                    //Registro usuario
                    UserRegister newUserRegister = new UserRegister();
                    bool newUserRegisterOk = newUserRegister.RegisterManagment(_client);
                    if (newUserRegisterOk)
                    {
                        Console.WriteLine("Usuario registrado Correctamente");
                    }
                    else
                    {
                        Console.WriteLine("Usuario no se ha podido registrar");
                        
                    }
                    ConnectToServerConsole();
                    break;
                case 2:
                    //Login
                    LoginMenu newLogin = new LoginMenu();
                    bool isLogOk = newLogin.LoginManagment(_client);

                    if (isLogOk)
                    {
                        //Accede al menu de usuarios registado
                        LoggedUserConsoleMenu();
                    }
                    else
                    {
                        Console.WriteLine("Los datos suministrados para el inicio de sesion no son validos.");
                        ConnectToServerConsole();
                    }

                    break;
                case 3:
                     LogoutMenu logout=new LogoutMenu();
                     if (logout.LogOut(_client)) Console.WriteLine("Saliendo...");
                     _client.Shutdown(SocketShutdown.Both);
                     _client.Close();
                     break;
                default:
                    Console.WriteLine("Opcion elegida no es correcta.");
                    break;
            }
        }
    }
}