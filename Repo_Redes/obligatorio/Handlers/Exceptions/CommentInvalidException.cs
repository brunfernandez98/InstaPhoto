namespace Handlers.User.Exceptions
{
    public class CommentInvalidException:BusinessRuleException
    {
        private static readonly string _message = "Verifique Foto a comentar";
        public CommentInvalidException() : base(_message)
        {
        }
    }
}