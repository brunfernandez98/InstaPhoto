using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Text.RegularExpressions;
using System.Threading;
using Domain;

namespace SystemService
{
    public sealed class System
    {
        private static  System _instance = null;
        private static readonly object padlock = new object();

       public List<EstablishedConnection> EstablishedConnectionsList{ get; set; }
        
        public List<User> UserList { get; set; }
        public List<Photo> UsersPhotos { get; set; }
        private System()
        {
            EstablishedConnectionsList = new List<EstablishedConnection>();
            UserList =new List<User>();
            UsersPhotos=new List<Photo>();
        }
        
        public static System GetInstance()
        {
           
                if (_instance == null)
                {
                    _instance = new System();
                }
                return _instance;
            
        }

        public void RegisterUser(User oneUser)
        {
            lock (padlock)
            {
              UserList.Add(oneUser);
            }
        }
       
        public bool ExistPhoto(string name){
            lock (padlock)
            {
                
                Photo onePhoto = new Photo(name);
                return UsersPhotos.Contains(onePhoto);
            }
        }
        public Photo GetPhoto(string name){
            lock (padlock)
            {
                
                Photo onePhoto = new Photo(name);
                return UsersPhotos.Find(photo => photo.NamePhoto == onePhoto.NamePhoto);
            }
        }
        
        public ICollection<Tuple<string,string>> GetCommentsOfPhoto(string name){
            lock (padlock)
            {
                Photo onePhoto = new Photo(name);
                return UsersPhotos.Find(photo => photo.NamePhoto == onePhoto.NamePhoto).CommentList;
            }
        }
        
        public void CleanConnection(string userEmail){
            lock (padlock)
            {

                int index = EstablishedConnectionsList.FindIndex(user =>
                    user.OneUser.Email.Equals(userEmail));
               EstablishedConnectionsList.Find(user =>
                    user.OneUser.Email.Equals(userEmail)).CleanConnection();
                EstablishedConnectionsList.RemoveAt(index);
            }
        }
        
}  
    }
