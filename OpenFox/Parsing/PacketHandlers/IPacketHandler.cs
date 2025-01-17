using OpenFox.DataAccess;
using OpenFox.Parsing.Packets;

namespace OpenFox.Parsing.PacketHandlers
{
    public interface IPacketHandler
    {
        void Handle(IPacket packet, IPacketCallback cb);
    }

    public interface IPacketCallback
    {
        void Process(DataMessagePacket packet);
        void SendAck(ushort exchnageId);
        void UpdateAck(ushort exchangeId);
        void SetConnectionResponseParams(ConnectionPacket pt);
        void SaveResponse(OFMLResponse message);
    }
}
