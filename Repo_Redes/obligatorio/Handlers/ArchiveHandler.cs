using System;
using System.Net.Sockets;
using System.Text;
using Server.RBMSQ;
using SharedProtocol.Protocol;
using Util;
using Util.FileHandler;
using Util.FileHandler.Interfaces;
using Util.Network;
using Util.Network.Interfaces;

namespace SharedDataTransfer.DataTransfer
{
    public class ArchiveHandler
    {

        private readonly IFileStreamHandler _fileStreamHandler;
        private readonly INetworkStreamHandler _networkStreamHandler;
        private LoggerMSQ _loggerMsq { get; set; }
        public ArchiveHandler(Socket connection)
        {
            _networkStreamHandler = new NetworkStreamHandler(connection);
            _fileStreamHandler = new FileStreamHandler();
            _loggerMsq=LoggerMSQ.GetInstance();
        }

        public void ReceiveFile(long fileSize,string fileName)
        {
            long parts = SpecificationArchive.GetParts(fileSize);
            long offset = 0;
            long currentPart = 1;

            while (fileSize > offset)
            {
                byte[] data;
                if (currentPart == parts)
                {
                    var lastPartSize = (int)(fileSize - offset);
                    data = _networkStreamHandler.Read(lastPartSize);
                    offset += lastPartSize;
                }
                else
                {
                    data = _networkStreamHandler.Read(Constants.MaxPacketSize);
                    offset += Constants.MaxPacketSize;
                }
                _fileStreamHandler.Write(fileName, data);
                currentPart++;
            }
        }
        public void SendFile(long fileSize,string path) {
            
            long parts = SpecificationArchive.GetParts(fileSize);
            long offset = 0;
            long currentPart = 1;

            while (fileSize > offset)
            {
                byte[] data;
                if (currentPart == parts)
                {
                    var lastPartSize = (int)(fileSize - offset);
                    data = _fileStreamHandler.Read(path, offset, lastPartSize);
                    offset += lastPartSize;
                }
                else
                {
                    data = _fileStreamHandler.Read(path, offset, Constants.MaxPacketSize);
                    offset += Constants.MaxPacketSize;
                }

                _networkStreamHandler.Write(data);
                currentPart++;
            }
        }
    }
    }
    
