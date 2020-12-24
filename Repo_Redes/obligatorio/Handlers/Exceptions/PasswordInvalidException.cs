namespace Handlers.User.Exceptions
{
    public class PasswordInvalidException: BusinessRuleException
    {
        
            private static readonly string _Message = "Verifique Password";
            public PasswordInvalidException() : base(_Message)
            {
                
            }
        }
    
}