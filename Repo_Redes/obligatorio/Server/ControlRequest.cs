using System;
using System.Net.Sockets;
using System.Text;
using SystemService;
using AdminServer;
using Domain;
using Handlers;
using Handlers.User;
using Server.RBMSQ;
using SharedDataTransfer.DataTransfer;
using SharedProtocol;
using SharedProtocol.Protocol;

namespace Server
{
    public class ControlRequest
    {
        private ConnectionManagment _connectionManagment { get; set; }
        private IUserHandler _userHandler { get; set; }


        public ControlRequest(ConnectionManagment connectionManagment, IUserHandler userHandler)
        {
            _connectionManagment = connectionManagment;
            _userHandler = userHandler;
        }

        public void HandleRequest(byte[] package)
        {
            HeaderCodification frameCodification = new HeaderCodification();
            Protocol request = frameCodification.DecodeData(package);
            int command = request.Command;
            byte[] data = request.Data;
            // IF(!REQ) send COMANDO INCORRECTO

            switch (command)
            {
                case 01:
                    LoginHandler loginHandler = new LoginHandler(_connectionManagment);
                    loginHandler.LoginResponse(data);
                    break;
                case 02:
                    LoginHandler logout = new LoginHandler(_connectionManagment);
                    logout.LogoutResponse(data);
                    break;
                case 03:
                    RegisterHandler registerHandler = new RegisterHandler(_connectionManagment, _userHandler);
                    registerHandler.RegisterResponse(data);
                    break;
                case 04:
                    PhotoManager photoManager = new PhotoManager(_connectionManagment, _userHandler);
                    photoManager.ManageAndSaveFile(data);
                    break;
                case 05:
                    ListUserHandler listUserHandler = new ListUserHandler(_connectionManagment, _userHandler);
                    listUserHandler.ListUserResponse(data);
                    break;
                case 06:
                    ListPhotoHandler listPhotoHandler = new ListPhotoHandler(_connectionManagment);
                    listPhotoHandler.ListPhotoResponse(data);
                    break;
                case 07:
                    photoManager = new PhotoManager(_connectionManagment, _userHandler);
                    photoManager.ListComment(data);
                    break;
                case 08:
                    photoManager = new PhotoManager(_connectionManagment, _userHandler);
                    photoManager.CommentPhoto(data);
                    break;
            }
        }
    }
}