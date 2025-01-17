using OpenFox.Parsing.Packets;
using OpenFox.Parsing.Utility;

namespace OpenFox.Parsing.PacketHandlers
{
    public class DataMessageHandler : IPacketHandler
    {
        public void Handle(IPacket packet, IPacketCallback cb)
        {
            DataMessagePacket pt = (DataMessagePacket)packet;
            // only ack end of frame per documentation
            // 4.2.3 Type A
            if (pt.endIndicator == MessageConstants.ExchangeConstantY)
            {
                cb.SendAck(pt.ExchangeId);
            }
            cb.Process(pt);
        }
    }
}
