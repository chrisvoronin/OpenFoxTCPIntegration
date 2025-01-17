using OpenFox.Parsing.Utility;

namespace OpenFox.Parsing.Packets
{
    public class DataMessagePacket : IPacket
    {
        public PacketType Type => PacketType.DataMessage;

        public uint SizeHint => 16;

        public string OFML;

        public byte endIndicator;

        private ushort _exchangeId;
        public ushort ExchangeId { get => _exchangeId; set => _exchangeId = value; }

        public void Deserialize(PacketReader reader)
        {
            reader.SetPosition(8);
            _exchangeId = reader.GetUShort(); //2 bytes
            reader.SkipBytes(1); // type
            endIndicator = reader.GetByte();

            // total minus 12 beginning and 4 ending
            int size = reader.UserDataSize - 16;
            OFML = reader.GetString(size);
        }

        public void Serialize(PacketWriter writer)
        {
            uint messageLength = SizeHint + (uint)OFML.Length;

            writer.Put(MessageConstants.ExchangeFrameStart);
            writer.Put(messageLength);
            writer.Put(ExchangeId);
            writer.Put((byte)Type);
            writer.Put(MessageConstants.ExchangeConstantY);
            writer.Put(OFML);
            writer.Put(MessageConstants.ExchangeFrameEnd);
        }
    }
}
