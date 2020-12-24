using System;
using System.Collections.Generic;
namespace Domain
{
    public class User
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public ICollection<Photo> PhotoList { get; set; }


        public User(string name, string lastname, string password, string email,
            ICollection<Photo> photoList)
        {
            this.Name = name;
            this.LastName = lastname;
            this.Password = password;
            this.Email = email;
            PhotoList = photoList;
        }

        public User()
        {
            PhotoList = new List<Photo>();
        }

        public void InsertPhoto(Photo OnePhoto)
        {
            
            PhotoList.Add(OnePhoto);
        }

        public override bool Equals(Object o)
        {
            var item = o as User;
            bool areEquals = false;

            if (item != null)
            {
                areEquals = this.Email.Equals(item.Email);
            }
            return areEquals;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, LastName, Password, Email, PhotoList);
        }
        
        public override String ToString()
        {
            return "El email es:" + Email + "\n su nombre:" + Name + "\n y su apellido:" + LastName;
        }
    }


}