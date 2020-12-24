using System;
using Domain;
using SharedProtocol.Protocol;

namespace SharedDataTransfer.DataTransferMQ
{
    public class InformationAdmin
    {
        public User user { get; set; }
        public InfoAdminEnum Type { get; set; }
        
        public String MessageTransfer { get; set; }
        
        public Option TypeOption{ get; set; }
        
        
        public InformationAdmin()
        {
            
        }
        
        public InformationAdmin(User oneUser, InfoAdminEnum oneType,String oneTypeMessage,Option  oneOption)
        {
            user = oneUser;
            Type = oneType;
            MessageTransfer = oneTypeMessage;
            TypeOption= oneOption;
        }

        public string ToString()
        {
            return user.ToString() + " realizo:" + Type + " - " + " Resultado:" + MessageTransfer;
        }
    }
}