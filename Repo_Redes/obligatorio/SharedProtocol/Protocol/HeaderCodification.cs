using System;
using System.Text;
using Util;


namespace SharedProtocol
{
    public class HeaderCodification
    {
        private byte[] _direction { get; set; }
        private byte[] _command { get; set; }
        private byte[] _dataLength { get; set; }
        private byte[] _data { get; set; }
      
        private String DecodedDirection;
        private int DecodedCommand;
        private int DecodedDataLength;
        
        

        public HeaderCodification(Protocol.Protocol oneProtocol)
        {
            _direction = Encoding.UTF8.GetBytes(oneProtocol.Header);
            var stringCommand = oneProtocol.Command.ToString("D2");
            _command = Encoding.UTF8.GetBytes(stringCommand);
            var stringData = oneProtocol.DataLength.ToString("D4"); 
            _dataLength = Encoding.UTF8.GetBytes(stringData);
            _data = oneProtocol.Data;
        }
        public HeaderCodification()
        {
            
        }
        public byte[] GetRequest()
        {
            int largeData = _data.Length;
            var header = new byte[largeData+Constants.HEADER_LENGTH];
            Array.Copy(_direction, 0, header, 0, 3);
            Array.Copy(_command, 0, header, Constants.REQ.Length, 2);
            Array.Copy(_dataLength, 0, header, Constants.REQ.Length + Constants.COMMAND_LENGTH, 4);
            Array.Copy(_data, 0, header, Constants.HEADER_LENGTH, largeData);
            return header;
        }
       
        public Protocol.Protocol DecodeData(byte[] data)
        {
           
            try
            {
                DecodedDirection = Encoding.UTF8.GetString(data, 0, Constants.REQ.Length);
                var command =
                    Encoding.UTF8.GetString(data, Constants.REQ.Length, Constants.COMMAND_LENGTH);
                DecodedCommand = int.Parse(command);
                var dataLength = Encoding.UTF8.GetString(data,
                    Constants.REQ.Length + Constants.COMMAND_LENGTH, Constants.LENGTH_IN_HEADER);
                DecodedDataLength = int.Parse(dataLength);
                int sourceIndex = Constants.HEADER_LENGTH;
                int destinationArrayLength = data.Length - Constants.HEADER_LENGTH;
                _data=new byte[destinationArrayLength];
                Array.Copy(data,sourceIndex,_data,0,destinationArrayLength);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                
            }
            Protocol.Protocol oneProtocol = new Protocol.Protocol(DecodedDirection, DecodedCommand, DecodedDataLength,_data);
            return oneProtocol;
        }
        
    }
}