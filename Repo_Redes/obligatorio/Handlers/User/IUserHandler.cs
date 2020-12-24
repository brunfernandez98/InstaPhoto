using System.Collections.Generic;
using Domain;

namespace Handlers.User
{
    public interface IUserHandler
    {
        public Domain.User Register(string email, string password, string name, string lastname);
        public bool IsRegisterUser(string email);
        public void ModifyName(string email, string name);
        public void ModifyEmail(string originalEmail,string emailToModify);
        public void ModifyPassword(string email,string password);
        public void ModifyLastName(string email,string lastName);
        public void DeleteUser(string email);
        public bool IsValidUser(string email,string password);
        public void InsertPhoto(Domain.User oneUser, Domain.Photo onePhoto);
        public List<Domain.User> GetListUsers();
        public Domain.User GetUser(string email);
        public ICollection<string> GetListOfPhotoUsers(Domain.User user);
        
        
    }
}