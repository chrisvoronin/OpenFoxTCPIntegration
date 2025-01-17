
namespace OpenFox.Parsing.Utility
{
    public static class MessageConstants
    {
        public const uint ExchangeFrameStart = 4278233685;

        public const uint ExchangeFrameEnd = 1437204735;

        public const byte ExchangeConstantY = (byte)'Y';
        public const byte ExchangeConstantN = (byte)'N';

        public const int MessageTypePosition = 10;

        public static byte[] MessageStartIndicatorBytes(bool isBigEndian)
        {
            PacketWriter writer = new PacketWriter(true, 16, isBigEndian);
            writer.Put(MessageConstants.ExchangeFrameStart);
            return writer.CopyData();
        }

        public static byte[] MessageEndIndicatorBytes(bool isBigEndian)
        {
            PacketWriter writer = new PacketWriter(true, 16, isBigEndian);
            writer.Put(MessageConstants.ExchangeFrameEnd);
            return writer.CopyData();
        }
    }

    static class MessageImageStartIndicator
    {
        public static readonly byte[] PHOTO = new byte[4] { (byte)'I', (byte)'M', (byte)'R', (byte)'/' };
        public static readonly byte[] SIGNATURE = new byte[4] { (byte)'S', (byte)'I', (byte)'G', (byte)'/' };
    }

    static class MessageObjectCoding
    {
        public static readonly byte[] NON = new byte[3] { (byte)'N', (byte)'O', (byte)'N' };
        public static readonly byte[] HEX = new byte[3] { (byte)'H', (byte)'E', (byte)'X' };
        public static readonly byte[] B64 = new byte[3] { (byte)'B', (byte)'6', (byte)'4' };
    }

    static class MessageNewLineConstants
    {
        public static readonly byte[] LF = new byte[4] { (byte)'L', (byte)'F', (byte)' ', (byte)' ' };
        public static readonly byte[] CR = new byte[4] { (byte)'C', (byte)'R', (byte)' ', (byte)' ' };
        public static readonly byte[] CRLF = new byte[4] { (byte)'C', (byte)'R', (byte)'L', (byte)'F' };
    }
}
