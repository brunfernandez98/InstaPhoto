using System;

namespace Handlers.User.Exceptions
{
    public abstract class BusinessRuleException : Exception
    {
        public BusinessRuleException(string message) : base(message)
        {
        }
    }
}