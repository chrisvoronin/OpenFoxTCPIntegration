using OpenFox.Parsing.Utility;

namespace OpenFox.Parsing.Packets
{
    public class NegativeAckPacket : IPacket
    {
        public PacketType Type => PacketType.NegativeAck;

        public uint SizeHint => 16;

        public string ErrorMessage = "";

        private ushort _exchangeId;
        public ushort ExchangeId { get => _exchangeId; set => _exchangeId = value; }

        public void Deserialize(PacketReader reader)
        {
            reader.SetPosition(8);
            _exchangeId = reader.GetUShort();
            reader.SkipBytes(1); // type
            reader.SkipBytes(1); // end indicator

            // total minus 12 beginning and 4 ending
            int size = reader.UserDataSize - 16;
            ErrorMessage = reader.GetString(size);
        }

        public void Serialize(PacketWriter writer)
        {
            uint messageLength = SizeHint + (uint)ErrorMessage.Length;

            writer.Put(MessageConstants.ExchangeFrameStart);
            writer.Put(messageLength);
            writer.Put(ExchangeId);
            writer.Put((byte)Type);
            writer.Put(MessageConstants.ExchangeConstantY);
            writer.Put(ErrorMessage);
            writer.Put(MessageConstants.ExchangeFrameEnd);
        }
    }
}
