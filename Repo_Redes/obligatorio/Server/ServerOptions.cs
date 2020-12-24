using System;
using System.Collections.Generic;
using System.Net.Sockets;
using SystemService;
using Domain;
using Handlers;
using Handlers.User;
using Handlers.User.Exceptions;
using Server.RBMSQ;
using SharedLog;
using SharedLogConstant;


namespace Server
{
    public class ServerOptions
    {
        private readonly IUserHandler _userHandler;
        private readonly ConnectionManagment _connectionManagment;
        private readonly LoggerMSQ _loggerMsq;

        public ServerOptions(ConnectionManagment connectionManagment, IUserHandler userHandler)
        {
            _userHandler = userHandler;
            _connectionManagment = connectionManagment;
            _loggerMsq = LoggerMSQ.GetInstance();
        }


        public ServerOptions()
        {
            _userHandler = new UserHandler();
            _loggerMsq = LoggerMSQ.GetInstance();
        }

        public void ShowClientsConnected()
        {
            List<EstablishedConnection> clientsConnected = _connectionManagment.OneSystem.EstablishedConnectionsList;
            if (clientsConnected.Count == 0)
            {
                Console.WriteLine("No hay clientes conectados");
            }
            else
            {
                foreach (var user in clientsConnected)
                {
                    Console.WriteLine(user.OneUser);
                }
            }
        }

        public void RegisterUser()
        {
            Console.WriteLine("Ingrese Nombre");
            var userInputName = Console.ReadLine();
            Console.WriteLine("Ingrese LastName");
            var userInputLastName = Console.ReadLine();
            Console.WriteLine("Ingrese Email");
            var userInputEmail = Console.ReadLine();
            Console.WriteLine("Ingrese Password");
            var userInputPassword = Console.ReadLine();

            bool isRegistered = _userHandler.IsRegisterUser(userInputEmail);

            string responseText = "";
            if (isRegistered)
            {
                responseText = "Usuario ya registrado";
            }
            else
            {
                try
                {
                    _userHandler.Register(userInputEmail, userInputPassword, userInputName, userInputLastName);
                    responseText = "Usuario registrado";
                }
                catch (EmailInvalidException e)
                {
                    Console.WriteLine(e.Message);
                    responseText = "Usuario no pudo ser registrado";
                }
                catch (PasswordInvalidException e)
                {
                    Console.WriteLine(e.Message);
                    responseText = "Usuario no pudo ser registrado";
                }
            }

            Console.WriteLine(responseText);
        }

        public void ModifyUser()
        {
            bool isModifyOk = true;
            bool isRegistered;
            var userInputEmail = "";
            do
            {
                Console.WriteLine("Ingrese Email del usuario a modificar, sino ingrese exit para salir");
                userInputEmail = Console.ReadLine();
                isRegistered = _userHandler.IsRegisterUser(userInputEmail);
                if (!isRegistered && userInputEmail != "exit")
                    Console.WriteLine("Usuario no esta registrado, ingrese otro nuevamente");
            } while (!isRegistered && userInputEmail != "exit");

            if (userInputEmail != "exit")
            {
                var userInput = 0;

                do
                {
                    var user = _userHandler.GetUser(userInputEmail);
                    var userName = user.Name;
                    var userLastName = user.LastName;
                    var userPassword = user.Password;
                    var userEmail = userInputEmail;
                    Console.WriteLine("1-Modificar Nombre");
                    Console.WriteLine("2-Modificar Apellido");
                    Console.WriteLine("3-Modificar Password");
                    Console.WriteLine("4-Modificar Email");
                    Console.WriteLine("5-Terminar");
                    Console.WriteLine("             ");
                    Console.WriteLine("Ingrese que desea modificar, ingrese 5 para terminar su modificacion");
                    Int32.TryParse(Console.ReadLine(), out userInput);
                    switch (userInput)
                    {
                        case 1:
                            Console.WriteLine("Ingrese nombre");
                            userName = Console.ReadLine();
                            _userHandler.ModifyName(userEmail, userName);
                            break;
                        case 2:
                            Console.WriteLine("Ingrese apellido");
                            userLastName = Console.ReadLine();
                            _userHandler.ModifyLastName(userEmail, userLastName);
                            break;
                        case 3:
                            Console.WriteLine("Ingrese password");
                            userPassword = Console.ReadLine();
                            try
                            {
                                _userHandler.ModifyPassword(userEmail, userPassword);
                            }
                            catch (PasswordInvalidException e)
                            {
                                Console.WriteLine(e.Message);
                                isModifyOk = false;
                            }

                            break;
                        case 4:
                            Console.WriteLine("Ingrese email");
                            userEmail = Console.ReadLine();
                            try
                            {
                                _userHandler.ModifyEmail(userInputEmail, userEmail);
                            }
                            catch (EmailInvalidException e)
                            {
                                Console.WriteLine(e.Message);
                                userEmail = userInputEmail;
                                isModifyOk = false;
                            }

                            if (isModifyOk)
                                userInputEmail = userEmail;

                            break;
                        case 5:
                            break;
                        default:
                            Console.WriteLine("Opcion elegida es incorrecta");
                            break;
                    }
                } while (userInput != 5);

                if (isModifyOk)
                    Console.WriteLine("Pudo modificar correctamente el usuario");
                else
                    Console.WriteLine("Modificacion incorrectamente el usuario, intente otra vez");
            }
        }

        public void CleanUser()
        {
            bool isDeleteOk = true;
            var userInputEmail = "";
            bool isRegistered;
            do
            {
                Console.WriteLine("Ingrese Email del usuario a modificar, sino ingrese exit para salir");
                userInputEmail = Console.ReadLine();
                isRegistered = _userHandler.IsRegisterUser(userInputEmail);
                if (!isRegistered && userInputEmail != "exit")
                    Console.WriteLine("Usuario no esta registrado, ingrese otro nuevamente");
            } while (!isRegistered && userInputEmail != "exit");

            if (isRegistered)
            {
                try
                {
                    _userHandler.DeleteUser(userInputEmail);
                    if (_connectionManagment.IsUserConnected(userInputEmail))
                        _connectionManagment.OneSystem.CleanConnection(userInputEmail);
                    Console.WriteLine("Se elimino el usuario:" + userInputEmail);
                }
                catch (SocketException socketEx)
                {
                    Console.WriteLine("Socket cerrado");
                }
            }
            else
            {
                Console.WriteLine("No se elimino ningun Usuario");
            }
        }

        public String RegisterUserAdmin(User userToRegister)
        {
            bool isRegistered = _userHandler.IsRegisterUser(userToRegister.Email);

            string responseText = "";
            if (isRegistered)
            {
                responseText = "Usuario ya registrado";
                Log oneLog = new Log("Cliente administrativo", LogEnum.CreateUser, LogEnumSeverity.Info);
                _loggerMsq.SendMessage(oneLog);
            }
            else
            {
                try
                {
                    _userHandler.Register(userToRegister.Email, userToRegister.Password, userToRegister.Name,
                        userToRegister.LastName);
                    responseText = "Usuario registrado";
                    Log oneLog = new Log("Cliente administrativo", LogEnum.CreateUser, LogEnumSeverity.Info);
                    _loggerMsq.SendMessage(oneLog);
                }
                catch (EmailInvalidException e)
                {
                    Console.WriteLine(e.Message);
                    responseText = "Usuario no pudo ser registrado";
                    Log oneLog = new Log("Cliente administrativo", LogEnum.CreateUser, LogEnumSeverity.Warning);
                    _loggerMsq.SendMessage(oneLog);
                }
                catch (PasswordInvalidException e)
                {
                    Console.WriteLine(e.Message);
                    responseText = "Usuario no pudo ser registrado";
                    Log oneLog = new Log("Cliente administrativo", LogEnum.CreateUser, LogEnumSeverity.Warning);
                    _loggerMsq.SendMessage(oneLog);
                }
            }

            return responseText;
        }

        public String ModifyUserNameAdmin(User userToModify, string email)
        {
            Log oneLog;
            if (!_userHandler.IsRegisterUser(email))
            {
                oneLog = new Log("Cliente administrativo", LogEnum.ModifyUserName, LogEnumSeverity.Warning);
                _loggerMsq.SendMessage(oneLog);
                return "No existe el usuario, no se pudo completar la modificacion";
            }


            _userHandler.ModifyName(email, userToModify.Name);
            oneLog = new Log("Cliente administrativo", LogEnum.ModifyUserName, LogEnumSeverity.Info);
            _loggerMsq.SendMessage(oneLog);
            return "Nombre del usuario modificado correctamente";
        }

        public String ModifyUserLastNameAdmin(User userToModify, string email)
        {
            Log oneLog;
            if (!_userHandler.IsRegisterUser(email))
            {
                oneLog = new Log("Cliente administrativo", LogEnum.ModifyUserLastName, LogEnumSeverity.Warning);
                _loggerMsq.SendMessage(oneLog);
                return "No existe el usuario, no se pudo completar la modificacion";
            }

            _userHandler.ModifyLastName(email, userToModify.LastName);
            oneLog = new Log("Cliente administrativo", LogEnum.ModifyUserLastName, LogEnumSeverity.Info);
            _loggerMsq.SendMessage(oneLog);
            return "Apellido del usuario modificado correctamente";
        }

        public String ModifyUserPasswordAdmin(User userToModify, string email)
        {
            Log oneLog;
            if (!_userHandler.IsRegisterUser(email))
            {
                oneLog = new Log("Cliente administrativo", LogEnum.ModifyUserPassword, LogEnumSeverity.Warning);
                _loggerMsq.SendMessage(oneLog);
                return "No existe el usuario, no se pudo completar la modificacion";
            }

            try
            {
                _userHandler.ModifyPassword(email, userToModify.Password);
                oneLog = new Log("Cliente administrativo", LogEnum.ModifyUserPassword, LogEnumSeverity.Info);
                _loggerMsq.SendMessage(oneLog);
            }
            catch (PasswordInvalidException e)
            {
                Console.WriteLine(e.Message);
                oneLog = new Log("Cliente administrativo", LogEnum.ModifyUserPassword, LogEnumSeverity.Warning);
                _loggerMsq.SendMessage(oneLog);
            }

            return "Password del usuario modificado correctamente";
        }

        public String ModifyEmailAdmin(User userToModify, string email)
        {
            Log oneLog;
            if (!_userHandler.IsRegisterUser(email))
            {
                oneLog = new Log("Cliente administrativo", LogEnum.ModifyUserEmail, LogEnumSeverity.Warning);
                _loggerMsq.SendMessage(oneLog);
                return "No existe el usuario, no se pudo completar la modificacion";
            }

            if (_userHandler.IsRegisterUser(userToModify.Email))
            {
                oneLog = new Log("Cliente administrativo", LogEnum.ModifyUserEmail, LogEnumSeverity.Warning);
                _loggerMsq.SendMessage(oneLog);
                return "Ya existe el email en el sistema, ingrese otro";
            }

            try
            {
                _userHandler.ModifyEmail(email, userToModify.Email);
                oneLog = new Log("Cliente administrativo", LogEnum.ModifyUserEmail, LogEnumSeverity.Info);
                _loggerMsq.SendMessage(oneLog);
            }
            catch (EmailInvalidException e)
            {
                Console.WriteLine(e.Message);
                oneLog = new Log("Cliente administrativo", LogEnum.ModifyUserEmail, LogEnumSeverity.Warning);
                _loggerMsq.SendMessage(oneLog);
            }

            return "Email del usuario modificado correctamente";
        }


        public String CleanUserAdmin(User userToDelete)
        {
            bool isDeleteOk = true;
            string responseText = "";

            var isRegistered = _userHandler.IsRegisterUser(userToDelete.Email);
            if (!isRegistered)
            {
                Log oneLog = new Log("Cliente administrativo", LogEnum.DeleteUser, LogEnumSeverity.Warning);
                _loggerMsq.SendMessage(oneLog);
                return "Usuario no esta registrado, ingrese otro nuevamente";
            }

            if (isRegistered)
            {
                try
                {
                    _userHandler.DeleteUser(userToDelete.Email);
                    responseText = "Usuario eliminado";
                    Log oneLog = new Log("Cliente administrativo", LogEnum.DeleteUser, LogEnumSeverity.Info);
                    _loggerMsq.SendMessage(oneLog);
                }
                catch (SocketException socketEx)
                {
                    responseText = "Error al eliminar el usuario";
                    Log oneLog = new Log("Cliente administrativo", LogEnum.DeleteUser, LogEnumSeverity.Warning);
                    _loggerMsq.SendMessage(oneLog);
                }
            }

            return responseText;
        }
    }
}