using OpenFox.Parsing.Packets;

namespace OpenFox.Parsing.PacketHandlers
{
    public class PositiveAckHandler : IPacketHandler
    {
        public void Handle(IPacket packet, IPacketCallback cb)
        {
            int exchangeId = packet.ExchangeId;
            // 1 = connection
            // 2 = heartbeat
            if (exchangeId > 2)
            {
                cb.UpdateAck(packet.ExchangeId);
            }
        }
    }
}
