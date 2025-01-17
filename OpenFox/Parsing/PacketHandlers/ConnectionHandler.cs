using OpenFox.Parsing.Packets;

namespace OpenFox.Parsing.PacketHandlers
{
    public class ConnectionHandler : IPacketHandler
    {
        public void Handle(IPacket packet, IPacketCallback cb)
        {
            ConnectionPacket pt = (ConnectionPacket)packet;
            cb.SetConnectionResponseParams(pt);
        }
    }
}
