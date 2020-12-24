using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Domain;

namespace SharedDataTransfer.DataTransfer
{
    public class InformationHandler
    {
        public byte[] PreparedData(string []dataToPrepare,char separator)
        {
            string returnData = "";
            for (int i = 0; i < dataToPrepare.Length; i++)
            {
                if (i == dataToPrepare.Length-1)
                {
                    returnData += dataToPrepare[i] ;    
                }
                else
                {
                    returnData += dataToPrepare[i]+ separator;
                }
                
            }
            byte[] dataInBytes = Encoding.UTF8.GetBytes(returnData);
            return dataInBytes;
        }
        
        public byte[] PreparedData(List<User> listUsers, char separator)
        {
            string joinList= string.Join(separator, listUsers);
         
            byte[] dataInBytes = Encoding.UTF8.GetBytes(joinList);
            return dataInBytes;
        }
        
        public byte[] PreparedData(ICollection<string> listPhoto, char separator)
        {
            string joinList= string.Join(separator, listPhoto);
         
            byte[] dataInBytes = Encoding.UTF8.GetBytes(joinList);
            return dataInBytes;
        }
        public byte[] PreparedData(ICollection<Tuple<string, string>> comment, char separator)
        {
            string joinList = string.Join(separator, 
                comment.Select(t => $"{t.Item1} - {t.Item2}"));
            byte[] dataInBytes = Encoding.UTF8.GetBytes(joinList);
            return dataInBytes;
        }
        public string[] ProcessData(byte[] data, char separator)
        {
            string getData = Encoding.UTF8.GetString(data);
            string[] separateData = getData.Split(separator);
            return separateData;
        }
        
        
    }
}