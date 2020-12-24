using System;
using System.Collections.Generic;
namespace Domain
{
    public class Photo
    {
      
        
        public string NamePhoto{ get; set; }
        public ICollection<Tuple<string,string>> CommentList { get; set; }

        public Photo(string namePhoto)
        {
            NamePhoto = namePhoto;
            CommentList = new List<Tuple<string,string>>();
        }


        public void AddComment(string oneComment,string emailUser)
        {
            Tuple<string, string> comment = new Tuple<string, string>(oneComment, emailUser);
            CommentList.Add(comment);
        }

        public override bool Equals(object obj)
        {
            var item = obj as Photo;
            bool areEquals = false;

            if (item != null)
            {
                areEquals = this.NamePhoto.Equals(item.NamePhoto);
            }
            return areEquals;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(NamePhoto, CommentList);
        }
    }
}