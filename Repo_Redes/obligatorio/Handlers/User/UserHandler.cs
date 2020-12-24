using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text.RegularExpressions;
using Handlers.User;
using SystemService;
using Domain;
using Handlers.User.Exceptions;

namespace Handlers
{
    public class UserHandler : IUserHandler
    {
        private readonly SystemService.System _oneSystem;
        private readonly object locker;
        
        
        

         
        public UserHandler()
        {
            _oneSystem= SystemService.System.GetInstance();
            locker = new object();
        }
        public Domain.User Register(string email, string password, string name, string lastname)
        {
            lock (locker)
            {
                validateEmail(email);
                validatePassword(password);
                Domain.User user = new Domain.User(name,lastname,password,email, new List<Domain.Photo>());
                _oneSystem.RegisterUser(user);
                return user;
            }
        }
        public Domain.User GetUser(string email)
        {
            return  GetListUsers().Find(user => user.Email.Equals(email));
        }
        
        public void InsertPhoto(Domain.User oneUser,Domain.Photo onePhoto)
        {
            lock (locker)
            {
             GetListUsers().Find(user => user.Email.Equals(oneUser.Email)).InsertPhoto(onePhoto);
            }
        }
        
        private void validateEmail(string email)
        {
            bool isEmail = Regex.IsMatch(email, @"^[\w!#$%&'+\-/=?\^_`{|}~]+(\.[\w!#$%&'+\-/=?\^_`{|}~]+)*@"
                                                + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$",
                RegexOptions.IgnoreCase);
            
            if (!isEmail)
            {
                throw new EmailInvalidException();
            }
        }

        private void validatePassword(string password)
        {
            bool validPassword = password.Length >= 8;
            if (!validPassword)
            {
                throw new PasswordInvalidException();
            }
        }

        public bool IsValidUser(string email, string password)
        {
            lock (locker)
            {
                return GetListUsers().Any(user => user.Email.Equals(email)&& user.Password.Equals(password));
            }
        }

        public bool IsRegisterUser(string email)
        {
            lock (locker)
            {
                return GetListUsers().Any(user => user.Email.Equals(email));
            }
        }

      
        public List<Domain.User> GetListUsers()
        {
            lock (locker)
            {
                return _oneSystem.UserList;
            }
        }
        
        public void DeleteUser(string email)
        {
            lock (locker)
            {
                Domain.User user=new Domain.User();
                user.Email = email;
                _oneSystem.UserList.Remove(user);
               
            }
        }
        
        public void ModifyEmail(string originalEmail,string emailToModify)
        {
            lock (locker)
            {
                validateEmail(emailToModify);
                _oneSystem.UserList.Find(ec => ec.Email.Equals(originalEmail)).Email= emailToModify;
            }
        }
        public void ModifyName(string email,string name)
        {
            lock (locker)
            {
                _oneSystem.UserList.Find(ec => ec.Email.Equals(email)).Name= name;
            }
        }
        public void ModifyLastName(string email,string LastName)
        {
            lock (locker)
            {
                _oneSystem.UserList.Find(ec => ec.Email.Equals(email)).LastName= LastName;
            }
        }
        public void ModifyPassword(string email,string password)
        {
            lock (locker)
            {
                validatePassword(password);
                _oneSystem.UserList.Find(ec => ec.Email.Equals(email)).Password = password;
            }
        }

        public ICollection<string> GetListOfPhotoUsers(Domain.User user)
        {
            lock (locker)
            {
             ICollection<string> returnNamePhoto=new List<string>();
            Domain.User oneUser = GetUser(user.Email);
             foreach (var photo in oneUser.PhotoList)
            {
                returnNamePhoto.Add(photo.NamePhoto);
            }

            return returnNamePhoto; 
            }
        }
    }
}