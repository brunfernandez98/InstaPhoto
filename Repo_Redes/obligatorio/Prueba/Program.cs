using System;
using System.Text;
using SharedProtocol;
using SharedProtocol.Commands;
using SharedProtocol.Protocol;
using Util;
namespace obligatorio
{
    public class Program
    {
        static void Main(string[] args)
        {
            Protocol testProtocol = new Protocol(Constants.REQ,CommandConstants.Connect,10);
            var HeaderCodification = new HeaderCodification(testProtocol);
            var HeaderBytes = HeaderCodification.GetRequest();
            Console.WriteLine(Encoding.UTF8.GetString(HeaderBytes,0,9));
            Console.WriteLine(HeaderCodification.DecodeData(HeaderBytes));
            
        }
        
    }
}