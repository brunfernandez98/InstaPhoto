using Server.RBMSQ;
using SharedDataTransfer.DataTransferMQ;
using SharedProtocol.Protocol;

namespace Server
{
    public class ControlRequestAdmin
    {
        public string HandleRequest(InformationAdmin oneInfo)
        {
            string message = "";

            switch (oneInfo.Type)
            {
                case InfoAdminEnum.Register:
                    message = CaseRegister(oneInfo);
                    break;
                case InfoAdminEnum.Delete:
                    message = CaseDelete(oneInfo);
                    break;
                case InfoAdminEnum.ModifyName:
                    message = CaseModifyName(oneInfo);
                    break;
                case InfoAdminEnum.ModifyEmail:
                    message = CaseModifyEmail(oneInfo);
                    break;
                case InfoAdminEnum.ModifyPassword:
                    message = CaseModifyPassword(oneInfo); 
                    break;
                case InfoAdminEnum.ModifyLastName:
                    message = CaseModifyLastName(oneInfo);
                    break;
                default:
                    message = "error de protocolo";
                    break;
            }

            return message;
        }

        private static string CaseRegister(InformationAdmin oneInfo)
        {
            var loggerAdmin = new LoggerAdminResponse();
            ServerOptions options = new ServerOptions();
            string message = options.RegisterUserAdmin(oneInfo.user);
            var oneMessage = new InformationAdmin(null, InfoAdminEnum.Register, message, Option.Response);
            loggerAdmin.SendMessage(oneMessage);
            return message;
        }

        private static string CaseDelete(InformationAdmin oneInfo)
        {
            var loggerAdmin = new LoggerAdminResponse();
            ServerOptions options = new ServerOptions();
            string message = options.CleanUserAdmin(oneInfo.user);
            var oneMessage = new InformationAdmin(null, InfoAdminEnum.Delete, message, Option.Response);
            loggerAdmin.SendMessage(oneMessage);

            return message;
        }


        private static string CaseModifyName(InformationAdmin oneInfo)
        {
            var loggerAdmin = new LoggerAdminResponse();
            ServerOptions options = new ServerOptions();
            string message = options.ModifyUserNameAdmin(oneInfo.user, oneInfo.MessageTransfer);
            var oneMessage = new InformationAdmin(null, InfoAdminEnum.ModifyName, message, Option.Response);
            loggerAdmin.SendMessage(oneMessage);
            return message;
        }
        private static string CaseModifyEmail(InformationAdmin oneInfo)
        {
            var loggerAdmin = new LoggerAdminResponse();
            ServerOptions options = new ServerOptions();
            string message = options.ModifyEmailAdmin(oneInfo.user, oneInfo.MessageTransfer);
            var oneMessage = new InformationAdmin(null, InfoAdminEnum.ModifyName, message, Option.Response);
            loggerAdmin.SendMessage(oneMessage);
            return message;
        }
        private static string CaseModifyLastName(InformationAdmin oneInfo)
        {
            var loggerAdmin = new LoggerAdminResponse();
            ServerOptions options = new ServerOptions();
            string message = options.ModifyUserLastNameAdmin(oneInfo.user, oneInfo.MessageTransfer);
            var oneMessage = new InformationAdmin(null, InfoAdminEnum.ModifyName, message, Option.Response);
            loggerAdmin.SendMessage(oneMessage);
            return message;
        }
        private static string CaseModifyPassword(InformationAdmin oneInfo)
        {
            var loggerAdmin = new LoggerAdminResponse();
            ServerOptions options = new ServerOptions();
            string message = options.ModifyUserPasswordAdmin(oneInfo.user, oneInfo.MessageTransfer);
            var oneMessage = new InformationAdmin(null, InfoAdminEnum.ModifyName, message, Option.Response);
            loggerAdmin.SendMessage(oneMessage);
            return message;
        }
    }
}