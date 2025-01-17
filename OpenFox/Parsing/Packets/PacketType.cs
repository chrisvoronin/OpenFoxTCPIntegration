using OpenFox.Parsing.Utility;

namespace OpenFox.Parsing.Packets
{
    public enum PacketType: byte
    {
        PositiveAck = (byte)'A',
        Connection = (byte)'C',
        //Encryption = (byte)'E',
        HeartBeat = (byte)'H',
        //Identification = (byte)'I',
        //KeyNeg = (byte)'K',
        DataMessage = (byte)'M',
        NegativeAck = (byte)'N'
    }
    
    public interface IPacket: INetSerializable
    {
        PacketType Type { get; }

        uint SizeHint { get; }

        ushort ExchangeId { get; set; }
    }

    public interface INetSerializable
    {
        void Serialize(PacketWriter writer);
        void Deserialize(PacketReader reader);
    }
}
