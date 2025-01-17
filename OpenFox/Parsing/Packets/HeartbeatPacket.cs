using OpenFox.Parsing.Utility;

namespace OpenFox.Parsing.Packets
{
    public class HeartbeatPacket : IPacket
    {
        public PacketType Type => PacketType.HeartBeat;

        private ushort _exchangeId;
        public ushort ExchangeId { get => _exchangeId; set => _exchangeId = value; }

        public uint SizeHint => 16;

        public void Deserialize(PacketReader reader)
        {
            reader.SetPosition(8);
            _exchangeId = reader.GetUShort();
        }

        public void Serialize(PacketWriter writer)
        {
            uint messageLength = SizeHint;

            writer.Put(MessageConstants.ExchangeFrameStart);
            writer.Put(messageLength);
            writer.Put(ExchangeId);
            writer.Put((byte)Type);
            writer.Put(MessageConstants.ExchangeConstantY); // signify last frame
            writer.Put(MessageConstants.ExchangeFrameEnd);
        }
    }
}
