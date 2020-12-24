using Util;

namespace SharedProtocol.Protocol
{
    public static class SpecificationArchive
    {
        public static long GetParts(long fileSize)
        {
            var parts = fileSize / Constants.MaxPacketSize;
            return parts * Constants.MaxPacketSize == fileSize ? parts : parts + 1;
        }
    }
}