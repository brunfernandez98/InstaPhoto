using System;
using Util;

namespace SharedProtocol.Protocol
{
    public class Protocol
    {
        public string Header { get; set; }
        public int Command { get; set; }
        public int DataLength { get; set; }
        public byte[] Data{ get; set; }
        public Protocol(string header, int cmd, int dataLength, byte[] data)
        {
            this.Header = header;
            this.Command = cmd;
            this.DataLength = dataLength;
            this.Data = data;
        }
       
    }
}