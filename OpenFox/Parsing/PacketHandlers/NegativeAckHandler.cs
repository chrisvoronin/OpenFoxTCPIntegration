using OpenFox.DataAccess;
using OpenFox.Parsing.Packets;

namespace OpenFox.Parsing.PacketHandlers
{
    public class NegativeAckHandler : IPacketHandler
    {
        public void Handle(IPacket packet, IPacketCallback cb)
        {
            NegativeAckPacket pt = (NegativeAckPacket)packet;
            OFMLResponse response = new OFMLResponse();
            response.message = pt.ErrorMessage;
            if (pt.ExchangeId > 2)
            {
                cb.SaveResponse(response);
            }            
        }
    }
}
