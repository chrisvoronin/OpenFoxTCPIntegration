using OpenFox.Parsing.Utility;

namespace OpenFox.Parsing.Packets
{
    public class ConnectionPacket : IPacket
    {
        public PacketType Type => PacketType.Connection;

        public uint SizeHint => 36;

        private ushort _exchangeId;

        public ushort ExchangeId { get => _exchangeId; set => _exchangeId = value; }

        ushort majorVersion = 1;
        ushort minorVersion = 0;
        uint maxFrameLen = 65000;

        public ushort maxIdleTime = 0;
        public ushort defaultTimeOut = 60;

        byte useEnc = MessageConstants.ExchangeConstantN;
        public byte[] objectCoding = MessageObjectCoding.B64;
        public byte[] newLine = MessageNewLineConstants.LF;

        public void Deserialize(PacketReader reader)
        {
            reader.SetPosition(8);
            _exchangeId = reader.GetUShort();
            reader.SkipBytes(1); // type
            reader.SkipBytes(1); // end indicator
            minorVersion = reader.GetUShort();
            majorVersion = reader.GetUShort();
            maxFrameLen = reader.GetUInt();
            maxIdleTime = reader.GetUShort();
            defaultTimeOut = reader.GetUShort();
            useEnc = reader.GetByte();
            reader.GetBytes(objectCoding, 3);
            reader.GetBytes(newLine, 4);
        }

        public void Serialize(PacketWriter writer)
        {
            // it's fixed length
            uint messageLength = SizeHint;

            writer.Put(MessageConstants.ExchangeFrameStart);
            writer.Put(messageLength);
            writer.Put(ExchangeId);
            writer.Put((byte)Type);
            writer.Put(MessageConstants.ExchangeConstantY);
            writer.Put(majorVersion);
            writer.Put(minorVersion);
            writer.Put(maxFrameLen);
            writer.Put(maxIdleTime);
            writer.Put(defaultTimeOut);
            writer.Put(useEnc);
            writer.Put(objectCoding);
            writer.Put(newLine);
            writer.Put(MessageConstants.ExchangeFrameEnd);
        }
    }
}
