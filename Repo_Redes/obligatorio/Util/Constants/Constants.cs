namespace Util
{
    public static class Constants
    {
        public static readonly string REQ = "REQ";
        public static readonly string RES = "RES";
        public static readonly int COMMAND_LENGTH = 2;
        public static readonly int LENGTH_IN_HEADER = 4;
        public static readonly int HEADER_LENGTH = 9;
        public static readonly int FixedFileNameLength = 4;
        public static readonly int FixedFileSizeLength = 8;
        public static readonly int MaxPacketSize = 32768; //32kb
    }
}