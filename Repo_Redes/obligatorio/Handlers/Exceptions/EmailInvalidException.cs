using System;
using System.Security.Authentication;

namespace Handlers.User.Exceptions
{
    
    public class EmailInvalidException : BusinessRuleException
    {
        private static readonly string _Message = "Verifique Email";
        public EmailInvalidException() : base(_Message)
        {
        }
    }
}